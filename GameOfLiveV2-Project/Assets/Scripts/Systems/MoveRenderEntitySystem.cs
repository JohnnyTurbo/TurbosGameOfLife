using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(ChangeVitalStateSystem))]
    public class MoveRenderEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var allCellDataComponents = GetComponentDataFromEntity<CellData>(true);
            Entities
                .WithReadOnly(allCellDataComponents)
                .ForEach(
                    (Entity e, int entityInQueryIndex, ref Translation translation, in DataEntityReference aliveCellData) =>
                    {
                        if (allCellDataComponents[aliveCellData.Value].IsAlive)
                        {
                            translation.Value.z = -5f;
                        }
                        else
                        {
                            translation.Value.z = 5f;
                        }
                    }).ScheduleParallel();
        }
    }
}