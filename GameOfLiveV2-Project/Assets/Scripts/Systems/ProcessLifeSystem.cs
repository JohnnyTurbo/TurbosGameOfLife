using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [DisableAutoCreation]
    [UpdateAfter(typeof(ChangeSelectedCellSystem))]
    public class ProcessLifeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        private static readonly int2[] _relativeCoordinates = 
        {
            new int2(-1, -1),
            new int2(-1, 0),
            new int2(-1, 1),
            new int2(0, 1),
            new int2(1, 1),
            new int2(1, 0),
            new int2(1, -1),
            new int2(0, -1)
        };
        
        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var currentGridData = GetSingleton<CurrentGridData>();
            var allCellDataComponents = GetComponentDataFromEntity<CellData>(true);
            var gridSize = currentGridData.GridSize;
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities
                .WithReadOnly(allCellDataComponents)
                .ForEach((Entity e, int entityInQueryIndex, ref ChangeVitalState changeVitalState, in CellData cellData,
                    in CellEntitiesReference cellEntitiesReference) =>
            {
                var aliveNeighbors = 0;
                foreach (var relativeCoordinate in _relativeCoordinates)
                {
                    var neighborPosition = cellData.GridPosition + relativeCoordinate;
                    if (!IsValidPosition(neighborPosition, gridSize)) {continue;}

                    var neighborEntity = cellEntitiesReference[neighborPosition].DataEntity;
                    var neighborPositionData = allCellDataComponents[neighborEntity];
                    if (neighborPositionData.IsAlive)
                    {
                        aliveNeighbors++;
                    }
                }

                if (cellData.IsAlive)
                {
                    if (aliveNeighbors < 2)
                    {
                        // Die from underpopulation
                        var renderEntity = cellEntitiesReference[cellData.GridPosition].RenderEntity;
                        ecb.AddComponent<ChangeCellColorTag>(entityInQueryIndex, renderEntity);
                        changeVitalState.Value = true;
                    }
                    else if (aliveNeighbors > 3)
                    {
                        // Die from overpopulation
                        var renderEntity = cellEntitiesReference[cellData.GridPosition].RenderEntity;
                        ecb.AddComponent<ChangeCellColorTag>(entityInQueryIndex, renderEntity);
                        changeVitalState.Value = true;
                    }
                }
                else if (aliveNeighbors == 3)
                {
                    // Birth by reproduction
                    var renderEntity = cellEntitiesReference[cellData.GridPosition].RenderEntity;
                    ecb.AddComponent<ChangeCellColorTag>(entityInQueryIndex, renderEntity);
                    changeVitalState.Value = true;
                }
                
            }).ScheduleParallel();
            Dependency.Complete();
        }

        private static bool IsValidPosition(int2 coordinatesToTest, int2 maxCoordinates)
        {
            return coordinatesToTest.x >= 0 && 
                   coordinatesToTest.x < maxCoordinates.x && 
                   coordinatesToTest.y >= 0 && 
                   coordinatesToTest.y < maxCoordinates.y;
        }
    }
}