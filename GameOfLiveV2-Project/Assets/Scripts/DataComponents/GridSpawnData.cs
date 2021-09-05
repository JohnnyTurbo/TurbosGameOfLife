using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct GridSpawnData : IComponentData
    {
        public int2 GridDimensions;
        public Entity TilePrefab;
        public Entity TileDataPrefab;
        public float3 PositionOffset;
    }

}