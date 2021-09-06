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

        public readonly bool IsValidCoordinate(int2 coordinate)
        {
            return coordinate.x >= 0 && 
                   coordinate.x < GridDimensions.x && 
                   coordinate.y >= 0 && 
                   coordinate.y < GridDimensions.y;
        }
    }

}