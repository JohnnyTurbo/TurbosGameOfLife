using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct NewGridData : IComponentData
    {
        public int2 NewGridSize;
    }
}