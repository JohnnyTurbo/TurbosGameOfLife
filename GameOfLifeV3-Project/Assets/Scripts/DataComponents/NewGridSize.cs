using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct NewGridSize : IComponentData
    {
        public int2 Value;

        public static implicit operator int2(NewGridSize newGridSize)
        {
            return newGridSize.Value;
        }

        public static implicit operator NewGridSize(int2 value)
        {
            return new NewGridSize {Value = value};
        }
    }
}