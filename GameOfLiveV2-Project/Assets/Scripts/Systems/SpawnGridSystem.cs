using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLiveV2
{
    public class SpawnGridSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            var gridSpawnData = GetSingleton<GridSpawnData>();
            var numEntities = gridSpawnData.GridDimensions.x * gridSpawnData.GridDimensions.y;
            var newTiles = EntityManager.Instantiate(gridSpawnData.TilePrefab, numEntities, Allocator.Temp);
            var dataPrefabArchetype = EntityManager.CreateArchetype(typeof(TilePositionData));
            var newDatas = EntityManager.CreateEntity(dataPrefabArchetype, numEntities, Allocator.Temp);

            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var cellGridBlobAsset = ref blobBuilder.ConstructRoot<CellBlobAssetX>();
            
            var xArray = blobBuilder.Allocate(ref cellGridBlobAsset.X, gridSpawnData.GridDimensions.x);

            var tileIndex = 0;
            for (var x = 0; x < gridSpawnData.GridDimensions.x; x++)
            {
                var yArray = blobBuilder.Allocate(ref xArray[x].Y, gridSpawnData.GridDimensions.y);
                for (var y = 0; y < gridSpawnData.GridDimensions.y; y++)
                {
                    var newTranslation = new Translation {Value = new float3(x, y, 0)};
                    newTranslation.Value += gridSpawnData.PositionOffset;
                    EntityManager.SetComponentData(newTiles[tileIndex], newTranslation);

                    var newTilePosition = new TilePositionData
                    {
                        //Value = new int2(x, y),
                        IsAlive = false,
                        Visuals = newTiles[tileIndex]
                    };
                    EntityManager.SetComponentData(newDatas[tileIndex], newTilePosition);

                    yArray[y] = new CellData {Value = newDatas[tileIndex]};
                    
                    tileIndex++;
                }
            }

            var cellGridRefData = GetSingleton<CellGridReference>();
            cellGridRefData.Value = blobBuilder.CreateBlobAssetReference<CellBlobAssetX>(Allocator.Persistent);
            SetSingleton(cellGridRefData);
        }

        protected override void OnUpdate()
        {
            
        }
    }
}