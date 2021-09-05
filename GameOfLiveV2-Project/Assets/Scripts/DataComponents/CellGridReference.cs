using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellGridReference : IComponentData
    {
        public BlobAssetReference<CellBlobAssetX> Value;
    }
}