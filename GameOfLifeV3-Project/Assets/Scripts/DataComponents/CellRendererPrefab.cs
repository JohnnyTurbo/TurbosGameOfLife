using Unity.Entities;

namespace TMG.GameOfLiveV3
{
    [GenerateAuthoringComponent]
    public struct CellRendererPrefab : IComponentData
    {
        public Entity Value;

        public static implicit operator Entity(CellRendererPrefab cellRendererPrefab)
        {
            return cellRendererPrefab.Value;
        }

        public static implicit operator CellRendererPrefab(Entity value)
        {
            return new CellRendererPrefab {Value = value};
        }
    }
}