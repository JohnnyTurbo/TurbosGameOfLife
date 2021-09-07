using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellGridReference : IComponentData
    {
        public BlobAssetReference<CellBlobAssetX> Value;

        public int xCount => Value.Value.X.Length;
        public int yCount => Value.Value[0].Length;
        
        public readonly CellData this[int x, int y] => Value.Value.X[x].Y[y];

        public readonly CellData this[int2 index] => Value.Value.X[index.x].Y[index.y];
    }
}