using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TMG.GameOfLifeV3
{
    public class SpawnGridSystem : SystemBase
    {
        private EntityArchetype _cellArchetype;
        private EndSimulationEntityCommandBufferSystem _endSimulationECBSystem;
        private Entity _gameController;
        private Entity _cellRenderPrefab;
        private GridOptionData _gridOptions;
        private int2 _newGridSize;
        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<NewGridSize>();
            _endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _gameController = GetSingletonEntity<GameControllerTag>();
            _cellRenderPrefab = GetSingleton<CellRendererPrefab>();
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
        
        protected override void OnUpdate()
        {
            SpawnGrid();
        }

        private void SpawnGrid()
        {
            var currentGridData = EntityManager.GetComponentData<CurrentGridData>(_gameController);
            _gridOptions = EntityManager.GetComponentData<GridOptionData>(_gameController);
            _newGridSize = GetSingleton<NewGridSize>();
            currentGridData.GridSize = _newGridSize;
            CameraController.Instance.SetToGridFullscreen(currentGridData.GridSize);
            EntityManager.SetComponentData(_gameController, currentGridData);
            
            switch (_gridOptions.GridOrganizationPattern)
            {
                case GridOrganizationPattern.RowColumn:
                    RowColumnSpawn();
                    break;
                case GridOrganizationPattern.ColumnRow:
                    ColumnRowSpawn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var cellEntitiesReference = GetBlobAssetReference();
            
            EntityManager.SetComponentData(_gameController, cellEntitiesReference);

            switch (_gridOptions.CellReferenceType)
            {
                case CellReferenceType.BlobAsset:
                    Entities.WithAll<CellPositionData>().ForEach((Entity e) =>
                    {
                        EntityManager.AddComponentData(e, cellEntitiesReference);
                    }).WithStructuralChanges().Run();
                    break;
                case CellReferenceType.DynamicBuffer:
                    var ecb = _endSimulationECBSystem.CreateCommandBuffer();
                    var newGridSize = _newGridSize;
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EntityManager.RemoveComponent<NewGridSize>(_gameController);
            GameOfLifeMonoController.Instance.ChangeProcessLifeType(_gridOptions.CellReferenceType);
        }

        private CellEntitiesReference GetBlobAssetReference()
        {
            var cellCount = _newGridSize.x * _newGridSize.y;
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var cellEntitiesBlob = ref blobBuilder.ConstructRoot<CellEntitiesBlob>();
            var blobData = blobBuilder.Allocate(ref cellEntitiesBlob.Value, cellCount);

            var eq = EntityManager.CreateEntityQuery(typeof(CellPositionData));
            var na = eq.ToEntityArray(Allocator.Temp);

            foreach (var entity in na)
            {
                var cellPosition = EntityManager.GetComponentData<CellPositionData>(entity).Value;
                var renderEntity = EntityManager.GetComponentData<RenderCellReference>(entity).Value;
                
                blobData[FlatIndex(cellPosition.x, cellPosition.y, _newGridSize.y)] = new CellEntities
                {
                    RenderEntity = renderEntity,
                    DataEntity = entity
                };
            }
            
            var blobAssetReference = blobBuilder.CreateBlobAssetReference<CellEntitiesBlob>(Allocator.Persistent);
            var cellEntitiesReference = new CellEntitiesReference
            {
                Value = blobAssetReference,
                GridHeight = _newGridSize.y
            };

            return cellEntitiesReference;
        }

        private Entity[] SpawnSeparateRenderAndDataCells(int x, int y)
        {
            var newRenderCell = EntityManager.Instantiate(_cellRenderPrefab);
            var cellRenderTranslation = new Translation {Value = new float3(x, y, 5f)};
            
            cellRenderTranslation.Value += _gridOptions.PositionOffset;
            EntityManager.SetComponentData(newRenderCell, cellRenderTranslation);
            
            var newCell = EntityManager.CreateEntity(_cellArchetype);
            var newCellPosition = new CellPositionData {Value = new int2(x, y)};
            var renderCellReference = new RenderCellReference {Value = newRenderCell};
            var cellVitalData = new CellVitalData {Value = false};
            
            EntityManager.SetComponentData(newCell, newCellPosition);
            EntityManager.SetComponentData(newCell, renderCellReference);
            EntityManager.SetComponentData(newCell, cellVitalData);
            
            var newEntities = new Entity[2];
            newEntities[0] = newRenderCell;
            newEntities[1] = newCell;
            return newEntities;
        }

        private void RowColumnSpawn()
        {
            for(var x = 0; x < _newGridSize.x; x++)
            {
                for (var y = 0; y < _newGridSize.y; y++)
                {
                    var newCells = SpawnSeparateRenderAndDataCells(x, y);
                }
            }
        }
        
        private static int FlatIndex(int x, int y, int h) => h * x + y;
        private static bool IsValidPosition(int2 coordinatesToTest, int2 maxCoordinates)
        {
            return coordinatesToTest.x >= 0 && 
                   coordinatesToTest.x < maxCoordinates.x && 
                   coordinatesToTest.y >= 0 && 
                   coordinatesToTest.y < maxCoordinates.y;
        }
        private void ColumnRowSpawn()
        {
            for(var y = 0; y < _newGridSize.y; y++)
            {
                for (var x = 0; x < _newGridSize.x; x++)
                {
                    var newCells = SpawnSeparateRenderAndDataCells(x, y);
                }
            }
        }
    }
}