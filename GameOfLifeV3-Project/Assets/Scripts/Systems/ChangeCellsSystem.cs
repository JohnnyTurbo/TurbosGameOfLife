using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    [AlwaysUpdateSystem]
    public partial class ChangeCellsSystem : SystemBase
    {
        private Camera _mainCamera;
        private Entity _gameController;

        public void AfterReloadOps()
        {
            _gameController = GetSingletonEntity<GameControllerTag>();
        }
        
        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            _gameController = GetSingletonEntity<GameControllerTag>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ChangeSingleCell();
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                RandomizeAllCells();
            }
        }

        public void RandomizeAllCells()
        {
            var currentGridData = EntityManager.GetComponentData<CurrentGridData>(_gameController);
            var cellEntitiesReference = EntityManager.GetComponentData<CellEntitiesReference>(_gameController);

            var gridSize = currentGridData.GridSize;

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var curEntity = cellEntitiesReference[x, y].DataEntity;
                    CellVitalData curCellData;
                    
                    if (currentGridData.ShouldSpawnRandomCell)
                    {
                        curCellData = true;
                    }
                    else
                    {
                        curCellData = false;
                    }
                    EntityManager.SetComponentData(curEntity, curCellData);
                }
            }
            EntityManager.SetComponentData(_gameController, currentGridData);
        }

        private void ChangeSingleCell()
        {
            var currentGridData = EntityManager.GetComponentData<CurrentGridData>(_gameController);
            var cellEntitiesReference = EntityManager.GetComponentData<CellEntitiesReference>(_gameController);

            var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPosition = new int2
            {
                x = (int) math.floor(mouseWorldPosition.x),
                y = (int) math.floor(mouseWorldPosition.y)
            };
            if (!currentGridData.IsValidCoordinate(mouseGridPosition))
            {
                return;
            }

            var selectedCellEntity = cellEntitiesReference[mouseGridPosition].DataEntity;
            var selectedCellData = EntityManager.GetComponentData<CellVitalData>(selectedCellEntity);
            selectedCellData.Value = !selectedCellData.Value;
            EntityManager.SetComponentData(selectedCellEntity, selectedCellData);
        }
    }
}