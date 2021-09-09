using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ChangeSelectedCellSystem : SystemBase
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
                var currentGridData = EntityManager.GetComponentData<CurrentGridData>(_gameController);
                var cellEntitiesReference = EntityManager.GetComponentData<CellEntitiesReference>(_gameController);
                
                var mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var mouseGridPosition = new int2
                {
                    x = (int) math.floor(mouseWorldPosition.x),
                    y = (int) math.floor(mouseWorldPosition.y)
                };
                if(!currentGridData.IsValidCoordinate(mouseGridPosition)){return;}
                var selectedCellEntity = cellEntitiesReference[mouseGridPosition].DataEntity;
                var selectedCellData = EntityManager.GetComponentData<CellData>(selectedCellEntity);
                selectedCellData.IsAlive = !selectedCellData.IsAlive;
                EntityManager.SetComponentData(selectedCellEntity, selectedCellData);
                var selectedRenderEntity = cellEntitiesReference[mouseGridPosition].RenderEntity;
                EntityManager.AddComponent<ChangeCellColorTag>(selectedRenderEntity);
            }
        }
    }
}