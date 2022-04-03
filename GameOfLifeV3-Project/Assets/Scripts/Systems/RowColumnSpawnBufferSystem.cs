using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLifeV3
{
    [DisableAutoCreation]
    public partial class RowColumnSpawnBufferSystem : SystemBase
    {
        private EntityArchetype _cellArchetype;
        private EndSimulationEntityCommandBufferSystem _endSimulationECBSystem;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<NewGridSize>();
            _endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _cellArchetype = EntityManager.CreateArchetype(typeof(CellPositionData), typeof(RenderCellReference),
                typeof(CellVitalData));
        }

        private static readonly int2[] _relativeCoordinates = 
        {
            new int2(-1, -1),
            new int2(-1, 0),
            new int2(-1, 1),
            new int2(0, 1),
            new int2(1, 1),
            new int2(1, 0),
            new int2(1, -1),
            new int2(0, -1)
        };
        
        protected override void OnStartRunning()
        {
            var ecb = _endSimulationECBSystem.CreateCommandBuffer();
            var gameController = GetSingletonEntity<GameControllerTag>();
            var currentGridData = EntityManager.GetComponentData<CurrentGridData>(gameController);
            int2 newGridSize = GetSingleton<NewGridSize>();
            currentGridData.GridSize = newGridSize;
            CameraController.Instance.SetToGridFullscreen(currentGridData.GridSize);
            EntityManager.SetComponentData(gameController, currentGridData);
            Entity cellRenderPrefab = GetSingleton<CellRendererPrefab>();
            var cellCount = newGridSize.x * newGridSize.y;

            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var cellEntitiesBlob = ref blobBuilder.ConstructRoot<CellEntitiesBlob>();
            var blobData = blobBuilder.Allocate(ref cellEntitiesBlob.Value, cellCount);

            for(var x = 0; x < newGridSize.x; x++)
            {
                for (var y = 0; y < newGridSize.y; y++)
                {
                    var newRenderCell = EntityManager.Instantiate(cellRenderPrefab);
                    var cellRenderTranslation = new Translation {Value = new float3(x, y, 5f)};
                    EntityManager.SetComponentData(newRenderCell, cellRenderTranslation);
                    
                    var newCell = EntityManager.CreateEntity(_cellArchetype);
                    var newCellPosition = new CellPositionData {Value = new int2(x, y)};
                    var renderCellReference = new RenderCellReference {Value = newRenderCell};
                    var cellVitalData = new CellVitalData {Value = false};
                    
                    EntityManager.SetComponentData(newCell, newCellPosition);
                    EntityManager.SetComponentData(newCell, renderCellReference);
                    EntityManager.SetComponentData(newCell, cellVitalData);
                    blobData[FlatIndex(x, y, newGridSize.y)] = new CellEntities
                    {
                        DataEntity = newCell,
                        RenderEntity = newRenderCell
                    };
                }
            }
            
            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CellEntitiesBlob>(Allocator.Persistent);
            var cellEntitiesReference = new CellEntitiesReference
            {
                Value = blobAssetReference,
                GridHeight = newGridSize.y
            };
            
            EntityManager.SetComponentData(gameController, cellEntitiesReference);

            Entities.ForEach((Entity e, in CellPositionData positionData) =>
            {
                var neighbors = ecb.AddBuffer<NeighborBufferElementData>(e);
                foreach (var relativeCoordinate in _relativeCoordinates)
                {
                    var neighborPosition = positionData.Value + relativeCoordinate;
                    if (!IsValidPosition(neighborPosition, newGridSize)) {continue;}

                    var neighborEntity = cellEntitiesReference[neighborPosition].DataEntity;
                    neighbors.Add(neighborEntity);
                }
            }).Run();
            
            EntityManager.RemoveComponent<NewGridSize>(gameController);
        }
        
        private static bool IsValidPosition(int2 coordinatesToTest, int2 maxCoordinates)
        {
            return coordinatesToTest.x >= 0 && 
                   coordinatesToTest.x < maxCoordinates.x && 
                   coordinatesToTest.y >= 0 && 
                   coordinatesToTest.y < maxCoordinates.y;
        }
        
        private static int FlatIndex(int x, int y, int h) => h * x + y;
        
        protected override void OnUpdate()
        {
            
        }
    }
}