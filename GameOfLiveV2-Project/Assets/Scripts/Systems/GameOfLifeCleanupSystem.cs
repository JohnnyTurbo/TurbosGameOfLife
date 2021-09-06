using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(ProcessLifeSystem))]
    public class GameOfLifeCleanupSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref TilePositionData tilePositionData) =>
            {
                if (tilePositionData.ChangeNextFrame)
                {
                    tilePositionData.IsAlive = !tilePositionData.IsAlive;
                    tilePositionData.ChangeNextFrame = false;
                }                
            }).Run();
        }
    }
}