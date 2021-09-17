using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ChangeCellsSystem : SystemBase
    {
        private Camera _mainCamera;
        private Entity _gameController;
        
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
                    var curCellData = EntityManager.GetComponentData<CellData>(curEntity);
                    var aliveEntity = cellEntitiesReference[x, y].RendererEntity;

                    var aliveTranslation = new Translation();

                    if (currentGridData.ShouldSpawnRandomCell)
                    {
                        curCellData.IsAlive = true;
                        aliveTranslation.Value = new float3(curCellData.GridPosition.x + 0.5f, curCellData.GridPosition.y + 0.5f, -5f);
                    }
                    else
                    {
                        curCellData.IsAlive = false;
                        aliveTranslation.Value = new float3(curCellData.GridPosition.x + 0.5f, curCellData.GridPosition.y + 0.5f, 5f);
                    }
                    EntityManager.SetComponentData(curEntity, curCellData);
                    EntityManager.SetComponentData(aliveEntity, aliveTranslation);

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
            var selectedCellData = EntityManager.GetComponentData<CellData>(selectedCellEntity);
            selectedCellData.IsAlive = !selectedCellData.IsAlive;
            EntityManager.SetComponentData(selectedCellEntity, selectedCellData);
            var aliveEntity = cellEntitiesReference[selectedCellData.GridPosition].RendererEntity;
            var aliveTranslation = new Translation();
            
            if (selectedCellData.IsAlive)
            {
                aliveTranslation.Value = new float3(selectedCellData.GridPosition.x + 0.5f, selectedCellData.GridPosition.y + 0.5f, -5f);
            }
            else
            {
                aliveTranslation.Value = new float3(selectedCellData.GridPosition.x + 0.5f, selectedCellData.GridPosition.y + 0.5f, 5f);
            }
            
            EntityManager.SetComponentData(aliveEntity, aliveTranslation);
        }
    }
}