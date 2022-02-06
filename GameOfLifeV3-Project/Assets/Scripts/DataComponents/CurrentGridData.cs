using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct CurrentGridData : IComponentData
    {
        public int2 GridSize;
        public int CellCount => GridSize.x * GridSize.y;
        [Range(0, 1)] public float RandomCellSpawnRate;
        public Random Random;
        public bool ShouldSpawnRandomCell => Random.NextFloat() <= RandomCellSpawnRate;
        public readonly bool IsValidCoordinate(int2 coordinate)
        {
            return coordinate.x >= 0 && 
                   coordinate.x < GridSize.x && 
                   coordinate.y >= 0 && 
                   coordinate.y < GridSize.y;
        }
    }

}