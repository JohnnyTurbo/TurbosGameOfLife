using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLife
{
    [GenerateAuthoringComponent]
    public struct UnitCellData : IComponentData
    {
        //public Material LiveMaterial;
        //public Material DeadMaterial;
        public bool IsAlive;
        public bool ChangeThisFrame;
        public int2 Coordinates;
        //public int2[] NeighborCells;
        //public NativeArray<UnitCellData> NeighborCells;
    }
}