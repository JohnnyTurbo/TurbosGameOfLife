using Unity.Entities;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct AliveCellData : IComponentData
    {
        public Entity DataEntity;
    }
}