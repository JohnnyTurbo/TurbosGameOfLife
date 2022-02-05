using Unity.Entities;

namespace TMG.GameOfLifeV3
{
    public struct RenderCellReference : IComponentData
    {
        public Entity Value;
    }
}