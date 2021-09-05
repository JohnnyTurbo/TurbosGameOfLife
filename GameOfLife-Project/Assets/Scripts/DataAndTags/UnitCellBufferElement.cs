using Unity.Entities;

namespace TMG.GameOfLife
{
    [InternalBufferCapacity(8)]
    [GenerateAuthoringComponent]
    public struct UnitCellBufferElement : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(UnitCellBufferElement e)
        {
            return e.Value;
        }

        public static implicit operator UnitCellBufferElement(Entity e)
        {
            return new UnitCellBufferElement {Value = e};
        }
    }
}