using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLifeV3
{
    public class GameOfLifeMonoController : MonoBehaviour
    {
        public static GameOfLifeMonoController Instance; 
        
        public Entity GameControllerEntity;
        
        [Range(float.Epsilon, 5)] public float _tickRate;
        [SerializeField] private KeyCode _pauseKey;
        [SerializeField] private KeyCode _stepKey;
        [SerializeField] private int2 _initialGridSize;
        
        private bool _isPaused = true;
        private float _timer;
        private EntityManager _entityManager;
        private SystemBase _processLifeSystem;
        private ProcessLifeSystem _regularLifeSystem;
        private ProcessLifeBufferSystem _bufferLifeSystem;
        private ProcessLifeBufferSystem _processLifeBufferSystem;
        private ChangeCellsSystem _changeCellsSystem;
        private SystemBase _currentVisualizationSystem;
        private SetColorSystem _setColorSystem;
        private VisualizeChunkSystem _visualizeChunkSystem;
        
        private int2 _gridSize;
        
        public int2 GridSize => _gridSize;

        public bool IsPaused => _isPaused;

        public int TotalEntityCount;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _timer = _tickRate;
            var goWorld = World.DefaultGameObjectInjectionWorld;
            _entityManager = goWorld.EntityManager;
            _regularLifeSystem = goWorld.GetOrCreateSystem<ProcessLifeSystem>();
            _bufferLifeSystem = goWorld.GetOrCreateSystem<ProcessLifeBufferSystem>();
            _processLifeSystem = _regularLifeSystem;
            _processLifeBufferSystem = goWorld.GetOrCreateSystem<ProcessLifeBufferSystem>();
            _changeCellsSystem = goWorld.GetOrCreateSystem<ChangeCellsSystem>();
            _setColorSystem = goWorld.GetOrCreateSystem<SetColorSystem>();
            _visualizeChunkSystem = goWorld.GetOrCreateSystem<VisualizeChunkSystem>();
            _visualizeChunkSystem.Enabled = false;
            _currentVisualizationSystem = _setColorSystem;
            _currentVisualizationSystem.Enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_pauseKey))
            {
                PlayPauseLife();
            }

            if (_isPaused && Input.GetKeyDown(_stepKey))
            {
                AdvanceLife();
            }

            if (!_isPaused)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    AdvanceLife();
                    _timer = _tickRate;
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                //DestroyGrid();
            }
        }

        public void PlayPauseLife()
        {
            _isPaused = !_isPaused;
        }

        public void AdvanceLife()
        {
            _processLifeSystem.Update();
            //_processLifeBufferSystem.Update();
        }

        public void RandomizeGrid()
        {
            _changeCellsSystem.RandomizeAllCells();
        }

        public void ChangeVisualizationType(GridVisualizationType visualizationType)
        {
            _currentVisualizationSystem.Enabled = false;
            _currentVisualizationSystem = visualizationType switch
            {
                GridVisualizationType.GameOfLife => _setColorSystem,
                GridVisualizationType.ChunkViewer => _visualizeChunkSystem,
                _ => throw new ArgumentOutOfRangeException(nameof(visualizationType), visualizationType, null)
            };
            _currentVisualizationSystem.Enabled = true;
        }

        public void ChangeProcessLifeType(CellReferenceType referenceType)
        {
            _processLifeSystem = referenceType switch
            {
                CellReferenceType.BlobAsset => _regularLifeSystem,
                CellReferenceType.DynamicBuffer => _bufferLifeSystem,
                _ => throw new ArgumentOutOfRangeException(nameof(referenceType), referenceType, null)
            };
        }
        
        /*public void ResizeGrid(int2 newGridSize)
        {
            DestroyGrid();
            InitializeGrid(newGridSize);
        }

        private void InitializeGrid(int2 newGridSize)
        {
            _gridSize = newGridSize;
            var newGridData = new NewGridData {NewGridSize = newGridSize};
            _entityManager.AddComponentData(GameControllerEntity, newGridData);
        }

        private void DestroyGrid()
        {
            _entityManager.AddComponent<DestroyGridTag>(GameControllerEntity);
        }*/
    }
}