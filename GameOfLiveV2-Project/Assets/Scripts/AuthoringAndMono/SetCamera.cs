using System;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class SetCamera : MonoBehaviour
    {
        public static SetCamera Instance;
        
        private Camera mainCamera;
        
        private void Awake()
        {
            Instance = this;
            mainCamera = Camera.main;
        }

        public void Set(int2 gridSize)
        {
            mainCamera.transform.position = new Vector3(gridSize.x / 2f, gridSize.y / 2f, -10);
            mainCamera.orthographicSize = gridSize.x / 2f;
        }
    }
}