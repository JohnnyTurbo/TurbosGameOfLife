using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class GameOfLifeMonoController : MonoBehaviour
    {
        public static GameOfLifeMonoController Instance; 
        
        public Entity _gameControllerEntity;
        
        [Range(float.Epsilon, 5)] public float _tickRate;
        [SerializeField] private KeyCode _pauseKey;
        [SerializeField] private KeyCode _stepKey;

        private bool isPaused = false;
        private float timer;
        private EntityManager _entityManager;
        private ProcessLifeSystem _processLifeSystem;

        public bool IsPaused => isPaused;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            timer = _tickRate;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _processLifeSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ProcessLifeSystem>();
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
            var newGridData = new NewGridData {NewGridSize = newGridSize};
            _entityManager.AddComponentData(_gameControllerEntity, newGridData);
        }

        private void DestroyGrid()
        {
            _entityManager.AddComponent<DestroyGridTag>(_gameControllerEntity);
        }
    }
}