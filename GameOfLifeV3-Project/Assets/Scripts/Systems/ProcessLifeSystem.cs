using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    [DisableAutoCreation]
    public partial class ProcessLifeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

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
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

            var currentGridData = GetSingleton<CurrentGridData>();
            var gridSize = currentGridData.GridSize;
            
            Entities
                .ForEach((Entity e, int entityInQueryIndex, in CellPositionData positionData,
                    in CellEntitiesReference cellEntitiesReference) =>
                {
                    var aliveNeighbors = 0;
                    var vitalData = GetComponent<CellVitalData>(e);
                    foreach (var relativeCoordinate in _relativeCoordinates)
                    {
                        var neighborPosition = positionData.Value + relativeCoordinate;
                        if (!IsValidPosition(neighborPosition, gridSize)) {continue;}

                        var neighborEntity = cellEntitiesReference[neighborPosition].DataEntity;
                        var neighborPositionData = GetComponent<CellVitalData>(neighborEntity);
                        if (neighborPositionData.Value)
                        {
                            aliveNeighbors++;
                        }
                    }
                    if (vitalData.Value)
                    {
                        if (aliveNeighbors < 2)
                        {
                            // Die from underpopulation
                            vitalData.Value = false;
                        }
                        else if (aliveNeighbors > 3)
                        {
                            // Die from overpopulation
                            vitalData.Value = false;
                        }
                    }
                    else if (aliveNeighbors == 3)
                    {
                        // Birth by reproduction
                        vitalData.Value = true;
                    }
                    ecb.SetComponent(entityInQueryIndex, e, vitalData);
                }).ScheduleParallel();
            _ecbSystem.AddJobHandleForProducer(Dependency);
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