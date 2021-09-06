using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellGridReference : IComponentData
    {
        public BlobAssetReference<CellBlobAssetX> Value;
        //public BlobAssetReference<CellBlobAssetX> VisualValue;
        
        public readonly Entity GetDataEntityAtCoordinate(int2 coordinate)
        {
            return Value.Value.X[coordinate.x].Y[coordinate.y].Value;
        }
        
        public readonly Entity GetVisualEntityAtCoordinate(int2 coordinate)
        {
            return Value.Value.X[coordinate.x].Y[coordinate.y].VisualValue;
        }
    }
}