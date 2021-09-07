using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CurrentGridData : IComponentData
    {
        public int2 GridDimensions;
        public Entity TilePrefab;
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