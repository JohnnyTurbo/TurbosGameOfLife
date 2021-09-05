using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    public struct CellBlobAssetY
    {
        public BlobArray<CellData> Y;
    }

    public struct CellBlobAssetX
    {
        public BlobArray<CellBlobAssetY> X;
    }
}