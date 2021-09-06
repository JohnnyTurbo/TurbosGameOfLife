using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(ProcessLifeSystem))]
    public class GameOfLifeCleanupSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref TilePositionData tilePositionData, ref ChangeNextFrame changeNextFrame) =>
            {
                if (changeNextFrame.Value)
                {
                    tilePositionData.IsAlive = !tilePositionData.IsAlive;
                    changeNextFrame.Value = false;
                }                
            }).Run();
        }
    }
}