using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ClickToChangeSystem : SystemBase
    {
        private Camera mainCamera;
        private CellGridReference _cellGridReference;
        
        protected override void OnStartRunning()
        {
            mainCamera = Camera.main;
            _cellGridReference = GetSingleton<CellGridReference>();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var tilePos = new int2
                {
                    x = (int) math.floor(worldPos.x),
                    y = (int) math.floor(worldPos.y)
                };
                var entity = _cellGridReference.Value.Value.X[0].Y[0].Value;
                var posData = EntityManager.GetComponentData<TilePositionData>(entity);
                posData.IsAlive = true;
                EntityManager.SetComponentData(entity, posData);
            }
        }
    }
}