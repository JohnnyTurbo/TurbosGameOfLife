using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV3
{
    public struct CellPositionData : IComponentData
    {
        public float2 Value;

        public static implicit operator float2(CellPositionData cellPositionData)
        {
            return cellPositionData.Value;
        }

        public static implicit operator CellPositionData(float2 newPosition)
        {
            return new CellPositionData {Value = newPosition};
        }
    }
}