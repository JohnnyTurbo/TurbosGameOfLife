using Unity.Entities;

namespace TMG.GameOfLifeV3
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class DestroyGridSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<DestroyGridTag>();
        }

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            /*var gameController = GetSingletonEntity<GameControllerTag>();
            var gridSize = EntityManager.GetComponentData<CurrentGridData>(gameController).GridSize;
            var cellEntitiesReference = EntityManager.GetComponentData<CellEntitiesReference>(gameController);

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var curCellEntities = cellEntitiesReference[x, y];
                    EntityManager.DestroyEntity(curCellEntities.DataEntity);
                    EntityManager.DestroyEntity(curCellEntities.RenderEntity);
                }
            }
            
            cellEntitiesReference.Value.Dispose();*/
            var ecb = _ecbSystem.CreateCommandBuffer();
            
            var cellQueryDesc = new EntityQueryDesc
            {
                Any = new ComponentType[] { typeof(CellPositionData), typeof(CellColorData) }
            };
            
            var cellQuery = EntityManager.CreateEntityQuery(cellQueryDesc);
            //EntityManager.DestroyEntity(cellQuery);
            ecb.DestroyEntitiesForEntityQuery(cellQuery);
            
            var gameController = GetSingletonEntity<GameControllerTag>();
            ecb.RemoveComponent<DestroyGridTag>(gameController);
        }
    }
}