using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLiveV2
{
    //[DisableAutoCreation]
    [UpdateAfter(typeof(ProcessLifeSystem))]
    public class ChangeVitalStateSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            //var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.ForEach((Entity e, int entityInQueryIndex,ref CellData cellData, ref ChangeVitalState changeVitalState/*, in CellEntitiesReference cellEntitiesReference*/) =>
            {
                if (changeVitalState.Value)
                {
                    cellData.IsAlive = !cellData.IsAlive;
                    changeVitalState.Value = false;

                    /*var aliveEntity = cellEntitiesReference[cellData.GridPosition].AliveEntity;

                    Translation aliveTranslation = new Translation();
                    
                    if (cellData.IsAlive)
                    {
                        aliveTranslation.Value = new float3(cellData.GridPosition.x+ 0.5f, cellData.GridPosition.y+ 0.5f, -5f);
                    }
                    else
                    {
                        aliveTranslation.Value = new float3(cellData.GridPosition.x+ 0.5f, cellData.GridPosition.y+ 0.5f, 5f);
                    }*/
                    
                    //ecb.SetComponent(entityInQueryIndex, aliveEntity, aliveTranslation);
                }
            }).ScheduleParallel();
            Dependency.Complete();
        }
    }
}