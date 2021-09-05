using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLife
{
    [GenerateAuthoringComponent]
    public struct GameOfLifeData : IComponentData
    {
        public int2 InitialGridSize;
        public Entity UnitCellPrefab;
        public float2 CellSize;
        public float2 CellOffset;
        public float2 CellPadding;

        public int CellCount => InitialGridSize.x * InitialGridSize.y;
    }
}