using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLiveV3
{
    public class RowColumnSpawnSystem : SystemBase
    {
        private EntityArchetype _cellArchetype;
        private EndSimulationEntityCommandBufferSystem _endSimulationECBSystem;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NewGridSize>();
            _endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _cellArchetype = EntityManager.CreateArchetype(typeof(CellPositionData), typeof(RenderCellReference));
        }

        protected override void OnStartRunning()
        {
            var ecb = _endSimulationECBSystem.CreateCommandBuffer();
            var gameController = GetSingletonEntity<GameControllerTag>();
            int2 newGridSize = GetSingleton<NewGridSize>();
            Entity cellRenderPrefab = GetSingleton<CellRendererPrefab>();

            //var cellCount = newGridSize.x * newGridSize.y;

            for(var x = 0; x < newGridSize.x; x++)
            {
                for (var y = 0; y < newGridSize.y; y++)
                {
                    var newRenderCell = EntityManager.Instantiate(cellRenderPrefab);
                    var cellRenderTranslation = new Translation {Value = new float3(x, y, 5f)};
                    EntityManager.SetComponentData(newRenderCell, cellRenderTranslation);
                    //ecb.SetComponent(newRenderCell, cellRenderTranslation);
                    
                    var newCell = EntityManager.CreateEntity(_cellArchetype);
                    var newCellPosition = new CellPositionData {Value = new int2(x, y)};
                    var renderCellReference = new RenderCellReference {Value = newRenderCell};
                    EntityManager.SetComponentData(newCell, newCellPosition);
                    EntityManager.SetComponentData(newCell, renderCellReference);
                }
            }
        }

        protected override void OnUpdate()
        {
            
        }
    }
}