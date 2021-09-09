using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellEntitiesReference : IComponentData
    {
        public BlobAssetReference<CellEntitiesX> Value;

        public readonly CellEntities this[int x, int y] => Value.Value.X[x].Y[y];

        public readonly CellEntities this[int2 index] => Value.Value.X[index.x].Y[index.y];
    }
}