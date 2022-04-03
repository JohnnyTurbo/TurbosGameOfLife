using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    [DisableAutoCreation]
    public partial class ProcessLifeBufferSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities
                .ForEach((Entity e, int entityInQueryIndex, DynamicBuffer<NeighborBufferElementData> neighbors,
                    in CellPositionData positionData, in CellVitalData vitalData) =>
                {
                    var aliveNeighbors = 0;
                    foreach (var neighbor in neighbors)
                    {
                        Entity neighborEntity = neighbor;
                        
                        var neighborVitalData = GetComponent<CellVitalData>(neighborEntity);
                        var neighborPosData = GetComponent<CellPositionData>(neighborEntity);


                        if (neighborVitalData.Value)
                        {
                            aliveNeighbors++;
                        }
                    }

                    var nextVitalData = vitalData;
                    
                    if (vitalData.Value)
                    {
                        if (aliveNeighbors < 2)
                        {
                            // Die from underpopulation
                            nextVitalData.Value = false;
                        }
                        else if (aliveNeighbors > 3)
                        {
                            // Die from overpopulation
                            nextVitalData.Value = false;
                        }
                    }
                    else if (aliveNeighbors == 3)
                    {
                        // Birth by reproduction
                        nextVitalData.Value = true;
                    }

                    ecb.SetComponent(entityInQueryIndex, e, nextVitalData);
                }).ScheduleParallel();
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}