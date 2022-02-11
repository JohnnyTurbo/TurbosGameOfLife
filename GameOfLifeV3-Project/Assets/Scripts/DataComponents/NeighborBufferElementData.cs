using Unity.Entities;

namespace TMG.GameOfLifeV3
{
    [InternalBufferCapacity(8)]
    [GenerateAuthoringComponent]
    public struct NeighborBufferElementData : IBufferElementData
    {
        public Entity Value;

        public static implicit operator NeighborBufferElementData(Entity value)
        {
            return new NeighborBufferElementData {Value = value};
        }

        public static implicit operator Entity(NeighborBufferElementData element)
        {
            return element.Value;
        }
    }
}