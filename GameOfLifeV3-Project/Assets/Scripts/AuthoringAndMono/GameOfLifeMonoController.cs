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
        private ProcessLifeSystem _processLifeSystem;
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
            _processLifeSystem = goWorld.GetOrCreateSystem<ProcessLifeSystem>();
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
            Debug.Log($"Changing visualization type to: {visualizationType.ToString()}");
            _currentVisualizationSystem.Enabled = false;
            Debug.Log($"Disabled {_currentVisualizationSystem.ToString()}");
            switch (visualizationType)
            {
                case GridVisualizationType.GameOfLife:
                    _currentVisualizationSystem = _setColorSystem;
                    break;
                case GridVisualizationType.ChunkViewer:
                    _currentVisualizationSystem = _visualizeChunkSystem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(visualizationType), visualizationType, null);
            }

            _currentVisualizationSystem.Enabled = true;
            Debug.Log($"Enabled {_currentVisualizationSystem.ToString()}");

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