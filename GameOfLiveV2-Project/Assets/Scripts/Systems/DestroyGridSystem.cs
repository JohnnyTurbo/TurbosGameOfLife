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
            var gridSize = EntityManager.GetComponentData<CurrentGridData>(gameController).GridSize;
            var cellEntitiesReference = EntityManager.GetComponentData<CellEntitiesReference>(gameController);

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var curCellEntities = cellEntitiesReference[x, y];
                    EntityManager.DestroyEntity(curCellEntities.DataEntity);
                    EntityManager.DestroyEntity(curCellEntities.RenderEntity);
                }
            }
            
            cellEntitiesReference.Value.Dispose();

            EntityManager.RemoveComponent<DestroyGridTag>(gameController);
        }
    }
}