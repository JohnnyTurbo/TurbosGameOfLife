using Unity.Entities;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct CellVitalData : IComponentData
    {
        public bool Value;

        public static implicit operator bool(CellVitalData cellVitalData)
        {
            return cellVitalData.Value;
        }

        public static implicit operator CellVitalData(bool value)
        {
            return new CellVitalData {Value = value};
        }
    }
}