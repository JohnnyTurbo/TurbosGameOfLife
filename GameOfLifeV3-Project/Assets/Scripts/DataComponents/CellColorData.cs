using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    [MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
    public struct CellColorData : IComponentData
    {
        public float4 Value;
    }
}