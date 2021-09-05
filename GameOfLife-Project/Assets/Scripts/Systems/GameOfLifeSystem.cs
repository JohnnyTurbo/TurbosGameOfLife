using TMG.GameOfLife;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace TMG.GameOfLife
{
    [UpdateAfter(typeof(InitializeGameOfLife))]
    public class GameOfLifeSystem : SystemBase
    {
        private GameOfLifeData _gameOfLifeData;
        private GameOfLifeManagedData _gameOfLifeManagedData;
        //private UnitCellData[,] _allCells;
        private EndSimulationEntityCommandBufferSystem _buffer;
        
        private int2[] _relativeCoordinates = new[]
        {
            new int2(-1, -1),
            new int2(-1, 0),
            new int2(-1, 1),
            new int2(0, 1),
            new int2(1, 1),
            new int2(1, 0),
            new int2(1, -1),
            new int2(0, -1)
        };

        protected override void OnCreate()
        {
            _buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            var _gameOfLifeEntity = GetSingletonEntity<GameOfLifeData>();
            _gameOfLifeData = EntityManager.GetComponentData<GameOfLifeData>(_gameOfLifeEntity);
            _gameOfLifeManagedData = EntityManager.GetComponentData<GameOfLifeManagedData>(_gameOfLifeEntity);
            //_allCells = _gameOfLifeManagedData.allCells;
        }

        protected override void OnUpdate()
        {
            /*if (Input.GetKeyDown(KeyCode.A))
            {*/
                Entities.ForEach((ref UnitCellData cellData, ref DynamicBuffer<UnitCellBufferElement> bufferElement) =>
                {
                    var aliveNeighborCount = AliveNeighborCount(bufferElement);
                    if (cellData.IsAlive)
                    {
                        if (aliveNeighborCount < 2)
                        {
                            //Debug.Log($"Cell: {cellData.Coordinates} died from under population");
                            cellData.ChangeThisFrame = true;
                        }
                        else if (aliveNeighborCount > 3)
                        {
                            //Debug.Log($"Cell: {cellData.Coordinates} died from over population");
                            cellData.ChangeThisFrame = true;
                        }
                    }
                    else if (aliveNeighborCount == 3)
                    {
                        //Debug.Log($"Cell: {cellData.Coordinates} is bourne!");
                        cellData.ChangeThisFrame = true;
                    }
                }).WithoutBurst().Run();
            //}
        

            var ecb = _buffer.CreateCommandBuffer();
            
            Entities.ForEach((Entity e, UnitCellRenderData renderData, RenderMesh renderMesh, ref UnitCellData cellData) =>
            {
                //Debug.Log(cellData.Coordinates);
                if (cellData.ChangeThisFrame)
                {
                    //Debug.Log("changin!");
                    cellData.ChangeThisFrame = false;
                    if (cellData.IsAlive)
                    {
                        cellData.IsAlive = false;
                        renderMesh.material = renderData.DeadMaterial;
                        ecb.SetSharedComponent(e, renderMesh);
                        //EntityManager.SetSharedComponentData(e, renderMesh);
                    }
                    else
                    {
                        cellData.IsAlive = true;
                        renderMesh.material = renderData.LiveMaterial;
                        ecb.SetSharedComponent(e, renderMesh);
                        //EntityManager.SetSharedComponentData(e, renderMesh);
                    }
                }
            }).WithoutBurst().Run();
        }

        private int AliveNeighborCount(DynamicBuffer<UnitCellBufferElement> buffer)
        {
            int neighborCount = 0;
            foreach (var bufferElement in buffer)
            {
                var cmd = GetComponent<UnitCellData>(bufferElement);
                if (cmd.IsAlive)
                {
                    neighborCount++;
                }
            }

            //if(neighborCount > 0){Debug.Log($"Neigbor count: {neighborCount}");}
            return neighborCount;
        }
        
        /*private int AliveNeighborCount(int2 coordinates)
        {
            //return GetNeighborCount(coordinates, true);
            int neighborCount = 0;
            foreach (var relativeCoordinate in _relativeCoordinates)
            {
                var actualCoordinate = coordinates + relativeCoordinate;
                if(!IsRealCoordinate(actualCoordinate)){continue;}
                //Debug.Log($"Issa vibe at {actualCoordinate.ToString()}");
                //var thisCell = _allCells[actualCoordinate.x, actualCoordinate.y];
                //Debug.Log($"Celly at: {thisCell.Coordinates} isAlive? {thisCell.IsAlive}");
                if (thisCell.IsAlive)
                {
                    neighborCount++;
                    Debug.Log("Aliver nebery");
                }
            }

            return neighborCount;
        }*/

        /*private int DeadNeighborCount(int2 coordinates)
        {
            return GetNeighborCount(coordinates, false);
        }*/

        /*private int GetNeighborCount(int2 coordinates, bool countAlive)
        {
            int neighborCount = 0;
            foreach (var relativeCoordinate in _relativeCoordinates)
            {
                var actualCoordinate = coordinates + relativeCoordinate;
                if(!IsRealCoordinate(actualCoordinate)){continue;}

                if ((countAlive == true && _allCells[actualCoordinate.x, actualCoordinate.y].IsAlive) ||
                    (countAlive == false && !_allCells[actualCoordinate.x, actualCoordinate.y].IsAlive))
                    neighborCount++;
            }

            return neighborCount;
        }*/
        
        private bool IsRealCoordinate(int2 coordinate)
        {
            return coordinate.x >= 0 && 
                   coordinate.y >= 0 && 
                   coordinate.x < _gameOfLifeData.InitialGridSize.x && 
                   coordinate.y < _gameOfLifeData.InitialGridSize.y;
        }
    }
}