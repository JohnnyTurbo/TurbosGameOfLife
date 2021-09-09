using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct PersistentGridData : IComponentData
    {
        public Entity CellPrefab;
        public float3 CellOffset;
    }
}