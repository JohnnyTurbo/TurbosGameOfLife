using Unity.Entities;

namespace TMG.GameOfLife
{
    [GenerateAuthoringComponent]
    public class GameOfLifeManagedData : IComponentData
    {
        public UnitCellData[,] allCells;
    }
}