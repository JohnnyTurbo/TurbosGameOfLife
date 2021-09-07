using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ClickToChangeSystem : SystemBase
    {
        private Camera mainCamera;
        private Entity _gameController;
        
        protected override void OnStartRunning()
        {
            mainCamera = Camera.main;
            _gameController = GetSingletonEntity<GameControllerTag>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var currentGridData = EntityManager.GetComponentData<CurrentGridData>(_gameController);
                var cellGridReference = EntityManager.GetComponentData<CellGridReference>(_gameController);
                
                var worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var tilePos = new int2
                {
                    x = (int) math.floor(worldPos.x),
                    y = (int) math.floor(worldPos.y)
                };
                if(!currentGridData.IsValidCoordinate(tilePos)){return;}
                var entity = cellGridReference[tilePos].Value;
                var posData = EntityManager.GetComponentData<TilePositionData>(entity);
                posData.IsAlive = !posData.IsAlive;
                EntityManager.SetComponentData(entity, posData);
                var visualEntity = cellGridReference[tilePos].VisualValue;
                EntityManager.AddComponent<ChangeVisualsTag>(visualEntity);
            }
        }
    }
}