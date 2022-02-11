using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct CellOffsetData : IComponentData
    {
        public float3 Value;
    }
}