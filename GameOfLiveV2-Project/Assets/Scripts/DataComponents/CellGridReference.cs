﻿using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    [GenerateAuthoringComponent]
    public struct CellGridReference : IComponentData
    {
        public BlobAssetReference<CellBlobAssetX> Value;
    }
}