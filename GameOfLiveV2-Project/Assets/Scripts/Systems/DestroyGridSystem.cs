using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    public class DestroyGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<DestroyGridTag>();
        }
        
        protected override void OnUpdate()
        {
            var gameController = GetSingletonEntity<GameControllerTag>();

            var cellGridReference = EntityManager.GetComponentData<CellGridReference>(gameController);

            for (var x = 0; x < cellGridReference.xCount; x++)
            {
                for (var y = 0; y < cellGridReference.yCount; y++)
                {
                    var curCellData = cellGridReference[x, y];
                    EntityManager.DestroyEntity(curCellData.Value);
                    EntityManager.DestroyEntity(curCellData.VisualValue);
                }
            }
            
            cellGridReference.Value.Dispose();

            EntityManager.RemoveComponent<DestroyGridTag>(gameController);
        }
    }
}