using Unity.Entities;

namespace TMG.GameOfLifeV3
{
    public struct CellEntitiesBlob
    {
        public BlobArray<CellEntities> Value;
    }
}