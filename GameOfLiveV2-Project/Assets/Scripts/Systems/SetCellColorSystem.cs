using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(ChangeVitalStateSystem))]
    public class SetCellColorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e) =>
            {
                
            }).Run();
        }
    }
}