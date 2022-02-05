using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    public class ColorCellsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationECBSystem;

        protected override void OnStartRunning()
        {
            _endSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var eq = EntityManager.CreateEntityQuery(typeof(CellPositionData));
            //var eq = EntityManager.CreateEntityQuery(typeof(CellColorData));
            var newColorJob = new ColorBatchesJob
            {
                RenderCellHandle = GetComponentTypeHandle<RenderCellReference>(),
                ecb = _endSimulationECBSystem.CreateCommandBuffer().AsParallelWriter()
            };

            Dependency = newColorJob.ScheduleParallel(eq, 1, Dependency);
            Dependency.Complete();
        }
    }
    
    public struct ColorBatchesJob : IJobEntityBatch
    {
        public ComponentTypeHandle<RenderCellReference> RenderCellHandle;
        public EntityCommandBuffer.ParallelWriter ecb;
        
        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            //Debug.Log($"id: {batchIndex} count: {batchInChunk.Count}");
            NativeArray<RenderCellReference> renderCells = batchInChunk.GetNativeArray(RenderCellHandle);

            var newCol = Color.HSVToRGB(batchIndex * 0.1f % 1f, 1f, 1f);
            
            for (var i = 0; i < batchInChunk.Count; i++)
            {
                var renderEntity = renderCells[i].Value;
                var newColor = new CellColorData {Value = new float4(newCol.r, newCol.g, newCol.b, newCol.a)};
                ecb.SetComponent(batchIndex, renderEntity, newColor);
                //colorDatas[i] = new CellColorData {Value = new float4(newCol.r, newCol.g, newCol.b, newCol.a)};
            }
        }
    }
    
    public struct ColorCellsJob : IJobEntityBatch
    {
        public ComponentTypeHandle<CellColorData> CellColorTypeHandle;
        public double ElapsedTime;
        
        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            Debug.Log($"id: {batchIndex} count: {batchInChunk.Count}");
            NativeArray<CellColorData> colorDatas = batchInChunk.GetNativeArray(CellColorTypeHandle);

            var hue = (float)(ElapsedTime + (batchIndex * 2f)) * 0.1f % 1;
            //var newCol = Color.HSVToRGB(hue, 1f, 1f);
            var newCol = Color.HSVToRGB(batchIndex * 0.25f, 1f, 1f);
            //newCol = Color.red;
            //Debug.Log(newCol.ToString());
            for (var i = 0; i < batchInChunk.Count; i++)
            {
                //Debug.Log($"Set{i}");
                colorDatas[i] = new CellColorData {Value = new float4(newCol.r, newCol.g, newCol.b, newCol.a)};
            }
        }
    }
}