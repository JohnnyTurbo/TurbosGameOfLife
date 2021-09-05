using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.GameOfLife
{
    //[DisableAutoCreation]
    public class InitializeGameOfLife : SystemBase
    {
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
        
        private GameOfLifeData _gameOfLifeData;
        //private GameOfLifeManagedData _gameOfLifeManagedData;
        private EndSimulationEntityCommandBufferSystem _commandBufferSystem;
        private List<Entity> allEntities;
        
        protected override void OnCreate()
        {
            _commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            var ecb = _commandBufferSystem.CreateCommandBuffer();
            var _gameOfLifeEntity = GetSingletonEntity<GameOfLifeData>();
            _gameOfLifeData = EntityManager.GetComponentData<GameOfLifeData>(_gameOfLifeEntity);
            allEntities = new List<Entity>();
            //_gameOfLifeManagedData = EntityManager.GetComponentData<GameOfLifeManagedData>(_gameOfLifeEntity);
            
            //_gameOfLifeManagedData.allCells = new UnitCellData[_gameOfLifeData.InitialGridSize.x, _gameOfLifeData.InitialGridSize.y];
            
            for (var x = 0; x < _gameOfLifeData.InitialGridSize.x; x++)
            {
                for (var y = 0; y < _gameOfLifeData.InitialGridSize.y; y++)
                {
                    var newCell = EntityManager.Instantiate(_gameOfLifeData.UnitCellPrefab);
                    var newCellData = EntityManager.GetComponentData<UnitCellData>(newCell);
                    newCellData.Coordinates = new int2(x, y);
                    newCellData.IsAlive = false;
                    EntityManager.SetComponentData(newCell, newCellData);
                    allEntities.Add(newCell);
                    //_gameOfLifeManagedData.allCells[x, y] = newCellData;
                }
            }
            
            Entities.ForEach((Entity e, ref UnitCellData unitCellData, ref Translation trans, ref DynamicBuffer<UnitCellBufferElement> bufferElements) =>
            {
                trans.Value = CalculatePosition(unitCellData.Coordinates);
                //var testBuffer = ecb.SetBuffer<UnitCellBufferElement>(e);
                SetNeighbors(unitCellData, bufferElements);
            }).WithoutBurst().Run();
        }

        private float3 CalculatePosition(int2 coordinates)
        {
            var newPos = new float3
            {
                x = coordinates.x * (_gameOfLifeData.CellSize.x + _gameOfLifeData.CellPadding.x) + _gameOfLifeData.CellOffset.x,
                y = 0f,
                z = coordinates.y * (-1 * _gameOfLifeData.CellSize.y - _gameOfLifeData.CellPadding.y) - _gameOfLifeData.CellOffset.y
            };
            //Debug.Log(coordinates);
            return newPos;
        }

        private void SetNeighbors(UnitCellData cellData, DynamicBuffer<UnitCellBufferElement> buffer)
        {
            var coordinates = cellData.Coordinates;
            foreach (var relativeCoordinate in _relativeCoordinates)
            {
                var actualCoordinate = coordinates + relativeCoordinate;
                if (IsRealCoordinate(actualCoordinate))
                {
                    
                    foreach (var entity in allEntities)
                    {
                        var unitCellData = GetComponent<UnitCellData>(entity);
                        if (unitCellData.Coordinates.Equals(actualCoordinate))
                        {
                            //Debug.Log($"Adding bufferer");
                            buffer.Add(entity);
                        }
                    }
                    //buffer.Add(_gameOfLifeManagedData.allCells[actualCoordinate.x, actualCoordinate.y]);
                }
            }
        }

        private bool IsRealCoordinate(int2 coordinate)
        {
            return coordinate.x >= 0 && 
                   coordinate.y >= 0 && 
                   coordinate.x < _gameOfLifeData.InitialGridSize.x && 
                   coordinate.y < _gameOfLifeData.InitialGridSize.y;
        }
        
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                //Debug.Log("live!");
                Entities.ForEach((Entity e, ref UnitCellData cellData) =>
                {
                    if (cellData.Coordinates.x == 1)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                }).WithoutBurst().Run();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                //Debug.Log("live!");
                Entities.ForEach((Entity e, ref UnitCellData cellData) =>
                {
                    if (cellData.Coordinates.x == 14 && cellData.Coordinates.y == 15)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                    else if (cellData.Coordinates.x == 15 && cellData.Coordinates.y == 14)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                    else if (cellData.Coordinates.x == 15 && cellData.Coordinates.y == 15)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                    else if (cellData.Coordinates.x == 15 && cellData.Coordinates.y == 16)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                    else if (cellData.Coordinates.x == 16 && cellData.Coordinates.y == 14)
                    {
                        cellData.ChangeThisFrame = true;
                    }
                }).WithoutBurst().Run();
            }
        }
    }
}