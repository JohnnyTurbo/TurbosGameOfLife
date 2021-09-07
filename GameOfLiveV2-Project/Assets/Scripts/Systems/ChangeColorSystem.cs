using Unity.Entities;
using Unity.Rendering;

namespace TMG.GameOfLiveV2
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ChangeColorSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private CellMaterialData _cellMaterialData;
        private CellGridReference _cellGridReference;
        
        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var gameController = GetSingletonEntity<GameControllerTag>();
            _cellGridReference = EntityManager.GetComponentData<CellGridReference>(gameController);
            _cellMaterialData = EntityManager.GetComponentData<CellMaterialData>(gameController);
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var cellMaterialData = _cellMaterialData;
            Entities.WithAll<ChangeVisualsTag>().ForEach((Entity e, RenderMesh renderMesh) =>
            {
                renderMesh.material = renderMesh.material == cellMaterialData.Alive
                    ? cellMaterialData.Dead
                    : cellMaterialData.Alive;
                ecb.SetSharedComponent(e, renderMesh);
                ecb.RemoveComponent<ChangeVisualsTag>(e);
            }).WithoutBurst().Run();
        }
    }
}