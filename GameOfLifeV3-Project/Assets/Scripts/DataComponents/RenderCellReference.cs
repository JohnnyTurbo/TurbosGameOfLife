using Unity.Entities;

namespace TMG.GameOfLiveV3
{
    public struct RenderCellReference : IComponentData
    {
        public Entity Value;
    }
}