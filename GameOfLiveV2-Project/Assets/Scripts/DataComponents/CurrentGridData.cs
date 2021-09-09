using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CurrentGridData : IComponentData
    {
        public int2 GridSize;
        public int CellCount => GridSize.x * GridSize.y;
        public readonly bool IsValidCoordinate(int2 coordinate)
        {
            return coordinate.x >= 0 && 
                   coordinate.x < GridSize.x && 
                   coordinate.y >= 0 && 
                   coordinate.y < GridSize.y;
        }
    }

}