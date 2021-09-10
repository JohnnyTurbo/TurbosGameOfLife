using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public class PersistentGridManagedData : IComponentData
    {
        public Material GridMaterial;
    }
}