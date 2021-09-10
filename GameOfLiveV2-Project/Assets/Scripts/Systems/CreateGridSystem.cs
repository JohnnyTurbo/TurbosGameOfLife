using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [UpdateAfter(typeof(DestroyGridSystem))]
    public class CreateGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NewGridData>();
        }

        protected override void OnUpdate()
        {
            var gameController = GetSingletonEntity<GameControllerTag>();
            var currentGridData = EntityManager.GetComponentData<CurrentGridData>(gameController);
            var persistentGridData = EntityManager.GetComponentData<PersistentGridData>(gameController);
            var persistentGridManagedData = EntityManager.GetComponentData<PersistentGridManagedData>(gameController);
            var newGridData = EntityManager.GetComponentData<NewGridData>(gameController);
            
            currentGridData.GridSize = newGridData.NewGridSize;
            EntityManager.SetComponentData(gameController, currentGridData);
            CameraController.Instance.SetToGridFullscreen(currentGridData.GridSize);

            var backgroundEntity = persistentGridData.GridRenderer;
            var backgroundTranslation = new Translation
                {Value = new float3(currentGridData.GridSize.x / 2f, currentGridData.GridSize.y / 2f, 0f)};
            var backgroundScale = new NonUniformScale
                {Value = new float3(currentGridData.GridSize.x, currentGridData.GridSize.y, 1f)};
            var backgroundMat = persistentGridManagedData.GridMaterial;
            backgroundMat.mainTextureScale = new Vector2(currentGridData.GridSize.x, currentGridData.GridSize.y);
            
            EntityManager.SetComponentData(backgroundEntity, backgroundTranslation);
            EntityManager.SetComponentData(backgroundEntity, backgroundScale);
            
            var cellCount = currentGridData.CellCount;
           
            var newAliveCellEntities =
                EntityManager.Instantiate(persistentGridData.AliveCellPrefab, cellCount, Allocator.Temp);
            
            var cellDataArchetype = EntityManager.CreateArchetype(typeof(CellData), typeof(CellEntitiesReference),
                typeof(ChangeVitalState));
            var newCellDataEntities = EntityManager.CreateEntity(cellDataArchetype, cellCount, Allocator.Temp);

            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var cellGridBlobAsset = ref blobBuilder.ConstructRoot<CellEntitiesX>();
            
            var xArray = blobBuilder.Allocate(ref cellGridBlobAsset.X, currentGridData.GridSize.x);
            
            var tileIndex = 0;
            for (var x = 0; x < currentGridData.GridSize.x; x++)
            {
                var yArray = blobBuilder.Allocate(ref xArray[x].Y, currentGridData.GridSize.y);
                
                for (var y = 0; y < currentGridData.GridSize.y; y++)
                {
                    var cellRenderTranslation = new Translation {Value = new float3(x, y, 5f)};
                    cellRenderTranslation.Value += persistentGridData.CellOffset;
                    EntityManager.SetComponentData(newAliveCellEntities[tileIndex], cellRenderTranslation);

                    var newAliveCellData = new DataEntityReference {Value = newCellDataEntities[tileIndex]};
                    EntityManager.SetComponentData(newAliveCellEntities[tileIndex], newAliveCellData);
                    
                    var newCellData = new CellData
                    {
                        GridPosition = new int2(x, y),
                        IsAlive = false,
                    };
                    EntityManager.SetComponentData(newCellDataEntities[tileIndex], newCellData);

                    yArray[y] = new CellEntities
                    {
                        DataEntity = newCellDataEntities[tileIndex], 
                        RendererEntity = newAliveCellEntities[tileIndex]
                    };
                    tileIndex++;
                }
            }

            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CellEntitiesX>(Allocator.Persistent);
            
            var cellEntitiesReference = new CellEntitiesReference {Value = blobAssetReference};
            EntityManager.SetComponentData(gameController, cellEntitiesReference);

            foreach (var cellDataEntity in newCellDataEntities)
            {
                EntityManager.SetComponentData(cellDataEntity, cellEntitiesReference);
            }

            EntityManager.RemoveComponent<NewGridData>(gameController);

            GameOfLifeMonoController.Instance.TotalEntityCount = EntityManager.GetAllEntities(Allocator.Temp).Length;
        }
    }
}