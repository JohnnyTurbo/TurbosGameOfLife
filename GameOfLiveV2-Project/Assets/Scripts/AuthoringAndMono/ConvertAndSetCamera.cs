using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class ConvertAndSetCamera : MonoBehaviour, IConvertGameObjectToEntity
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var gridSize = dstManager.GetComponentData<GridSpawnData>(entity).GridDimensions;
            _mainCamera.transform.position = new Vector3(gridSize.x / 2f, gridSize.y / 2f, -10);
            _mainCamera.orthographicSize = gridSize.x / 2f;
        }
    }
}