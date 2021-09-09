using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class GameOfLifeMonoController : MonoBehaviour
    {
        public static GameOfLifeMonoController Instance; 
        
        public Entity GameControllerEntity;
        
        [Range(float.Epsilon, 5)] public float _tickRate;
        [SerializeField] private KeyCode _pauseKey;
        [SerializeField] private KeyCode _stepKey;
        [SerializeField] private int2 _initialGridSize;
        
        private bool isPaused = false;
        private float timer;
        private EntityManager _entityManager;
        private ProcessLifeSystem _processLifeSystem;
        private int2 _gridSize;
        
        public int2 GridSize => _gridSize;

        public bool IsPaused => isPaused;

        public int TotalEntityCount;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            timer = _tickRate;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _processLifeSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ProcessLifeSystem>();
            
            InitializeGrid(_initialGridSize);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_pauseKey))
            {
                PlayPauseLife();
            }

            if (isPaused && Input.GetKeyDown(_stepKey))
            {
                AdvanceLife();
            }

            if (!isPaused)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    _processLifeSystem.Update();
                    timer = _tickRate;
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                DestroyGrid();
            }
        }

        public void PlayPauseLife()
        {
            isPaused = !isPaused;
        }

        public void AdvanceLife()
        {
            _processLifeSystem.Update();
        }

        public void ResizeGrid(int2 newGridSize)
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
        }
    }
}