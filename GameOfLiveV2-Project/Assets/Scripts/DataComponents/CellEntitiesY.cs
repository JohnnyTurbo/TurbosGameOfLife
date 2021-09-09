using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    public struct CellEntitiesY
    {
        public BlobArray<CellEntities> Y;
    }

    public struct CellEntitiesX
    {
        public BlobArray<CellEntitiesY> X;
    }
}