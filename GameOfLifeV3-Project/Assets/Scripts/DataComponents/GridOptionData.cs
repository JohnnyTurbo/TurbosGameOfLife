using System;
using Unity.Entities;
using Unity.Mathematics;

namespace TMG.GameOfLifeV3
{
    [GenerateAuthoringComponent]
    public struct GridOptionData : IComponentData
    {
        public float3 PositionOffset;
        public CellReferenceType CellReferenceType;
        public GridOrganizationPattern GridOrganizationPattern;
        public GridVisualizationType GridVisualizationType;
    }

    public enum CellReferenceType
    {
        BlobAsset,
        DynamicBuffer
    }

    public enum GridOrganizationPattern
    {
        RowColumn,
        ColumnRow
    }

    public enum GridVisualizationType
    {
        GameOfLife,
        ChunkViewer
    }
}