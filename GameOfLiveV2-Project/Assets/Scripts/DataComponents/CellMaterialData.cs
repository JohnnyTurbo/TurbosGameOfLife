using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public class CellMaterialData : IComponentData
    {
        public Material Alive;
        public Material Dead;
    }
}