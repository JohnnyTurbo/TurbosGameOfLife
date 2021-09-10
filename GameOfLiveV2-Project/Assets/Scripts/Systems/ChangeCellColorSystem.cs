using Unity.Entities;
using Unity.Rendering;

namespace TMG.GameOfLiveV2
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ChangeCellColorSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _beginInitializationEntityCommandBufferSystem;
        private CellMaterialData _cellMaterialData;
        
        protected override void OnStartRunning()
        {
            _beginInitializationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var gameController = GetSingletonEntity<GameControllerTag>();
            _cellMaterialData = EntityManager.GetComponentData<CellMaterialData>(gameController);
        }

        protected override void OnUpdate()
        {
            var ecb = _beginInitializationEntityCommandBufferSystem.CreateCommandBuffer();
            var cellMaterialData = _cellMaterialData;
            Entities.WithAll<ChangeCellColorTag>().ForEach((Entity e, RenderMesh renderMesh) =>
            {
                renderMesh.material = renderMesh.material == cellMaterialData.Alive
                    ? cellMaterialData.Dead
                    : cellMaterialData.Alive;
                ecb.SetSharedComponent(e, renderMesh);
                ecb.RemoveComponent<ChangeCellColorTag>(e);
            }).WithoutBurst().Run();
            
            Entities.WithAll<SetAliveColorTag>().ForEach((Entity e, RenderMesh renderMesh) =>
            {
                renderMesh.material = cellMaterialData.Alive;
                ecb.SetSharedComponent(e, renderMesh);
                ecb.RemoveComponent<SetAliveColorTag>(e);
            }).WithoutBurst().Run();
            
            Entities.WithAll<SetDeadColorTag>().ForEach((Entity e, RenderMesh renderMesh) =>
            {
                renderMesh.material = cellMaterialData.Dead;
                ecb.SetSharedComponent(e, renderMesh);
                ecb.RemoveComponent<SetDeadColorTag>(e);
            }).WithoutBurst().Run();
        }
    }
    
    
}