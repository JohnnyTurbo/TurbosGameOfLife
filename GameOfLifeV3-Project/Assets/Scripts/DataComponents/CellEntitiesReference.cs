using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct CellEntitiesReference : IComponentData
    {
        public BlobAssetReference<CellEntitiesBlob> Value;
        public int GridHeight;

        public readonly CellEntities this[int x, int y] => Value.Value.Value[GridHeight * x + y];
        
        public readonly CellEntities this[int2 coords] => Value.Value.Value[GridHeight * coords.x + coords.y];
    }
}