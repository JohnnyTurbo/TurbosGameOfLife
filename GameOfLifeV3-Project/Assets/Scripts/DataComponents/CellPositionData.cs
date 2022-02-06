using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLifeV3
{
    public struct CellPositionData : IComponentData
    {
        public int2 Value;

        public static implicit operator int2(CellPositionData cellPositionData)
        {
            return cellPositionData.Value;
        }

        public static implicit operator CellPositionData(int2 newPosition)
        {
            return new CellPositionData {Value = newPosition};
        }
    }
}