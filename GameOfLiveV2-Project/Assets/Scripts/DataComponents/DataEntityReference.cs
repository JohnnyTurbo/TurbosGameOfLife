using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct DataEntityReference : IComponentData
    {
        public Entity Value;
    }
}