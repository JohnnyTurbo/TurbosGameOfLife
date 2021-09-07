using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [DisableAutoCreation]
    [UpdateAfter(typeof(ClickToChangeSystem))]
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
            var allTilePositions = GetComponentDataFromEntity<TilePositionData>(true);
            var maxCoordinates = currentGridData.GridDimensions;
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.WithReadOnly(allTilePositions).ForEach((Entity e, int entityInQueryIndex, ref ChangeNextFrame changeNextFrame, in TilePositionData tilePositionData, in CellGridReference grid) =>
            {
                var aliveNeighbors = 0;
                foreach (var relativeCoordinate in _relativeCoordinates)
                {
                    var neighborPosition = tilePositionData.Value + relativeCoordinate;
                    if (!IsValidPosition(neighborPosition, maxCoordinates)) {continue;}

                    var neighborEntity = grid[neighborPosition].Value;
                    var neighborPositionData = allTilePositions[neighborEntity];
                    if (neighborPositionData.IsAlive)
                    {
                        aliveNeighbors++;
                    }
                }

                if (tilePositionData.IsAlive)
                {
                    if (aliveNeighbors < 2)
                    {
                        // Die from underpopulation
                        var visualEntity = grid[tilePositionData.Value].VisualValue;
                        ecb.AddComponent<ChangeVisualsTag>(entityInQueryIndex, visualEntity);
                        changeNextFrame.Value = true;
                    }
                    else if (aliveNeighbors > 3)
                    {
                        // Die from overpopulation
                        var visualEntity = grid[tilePositionData.Value].VisualValue;
                        ecb.AddComponent<ChangeVisualsTag>(entityInQueryIndex, visualEntity);
                        changeNextFrame.Value = true;
                    }
                }
                else if (aliveNeighbors == 3)
                {
                    // Birth by reproduction
                    var visualEntity = grid[tilePositionData.Value].VisualValue;
                    ecb.AddComponent<ChangeVisualsTag>(entityInQueryIndex, visualEntity);
                    changeNextFrame.Value = true;
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