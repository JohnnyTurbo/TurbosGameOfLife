using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct TilePositionData : IComponentData
    {
        public int2 Value;
        public bool IsAlive;
        //public Entity Visuals;
    }
}