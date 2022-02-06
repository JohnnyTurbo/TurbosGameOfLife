using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    public class SetColorSystem : SystemBase
    {
        private BeginPresentationEntityCommandBufferSystem _ecbSystem;

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var eq = EntityManager.CreateEntityQuery(typeof(CellPositionData));
            //var eq = EntityManager.CreateEntityQuery(typeof(CellColorData));
            var newColorJob = new ColorCellJob
            {
                RenderCellHandle = GetComponentTypeHandle<RenderCellReference>(),
                CellVitalDataTypeHandle = GetComponentTypeHandle<CellVitalData>(true),
                ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter()
            };

            Dependency = newColorJob.ScheduleParallel(eq, 1, Dependency);
            Dependency.Complete();
        }
    }

    public struct ColorCellJob : IJobEntityBatch
    {
        public ComponentTypeHandle<RenderCellReference> RenderCellHandle;
        [ReadOnly] public ComponentTypeHandle<CellVitalData> CellVitalDataTypeHandle;
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var renderCells = batchInChunk.GetNativeArray(RenderCellHandle);
            var cellVitalDatas = batchInChunk.GetNativeArray(CellVitalDataTypeHandle);

            for (var i = 0; i < batchInChunk.Count; i++)
            {
                var newCol = cellVitalDatas[i] ? Color.green : Color.gray;
                var newColor = new CellColorData {Value = new float4(newCol.r, newCol.g, newCol.b, newCol.a)};
                var renderEntity = renderCells[i].Value;
                ecb.SetComponent(batchIndex, renderEntity, newColor);
            }

        }
    }

}