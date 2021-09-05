using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLife
{
    [GenerateAuthoringComponent]
    public class UnitCellRenderData : IComponentData
    {
        public Material LiveMaterial;
        public Material DeadMaterial;
    }
}