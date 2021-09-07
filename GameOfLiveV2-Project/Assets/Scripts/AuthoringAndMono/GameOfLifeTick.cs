using System;
using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class GameOfLifeTick : MonoBehaviour
    {
        [Range(float.Epsilon, 5)] public float _tickRate;
        [SerializeField] private KeyCode _pauseKey;
        [SerializeField] private KeyCode _stepKey;

        private bool isPaused = false;
        private float timer;
        private ProcessLifeSystem _processLifeSystem;

        public bool IsPaused => isPaused;

        private void Start()
        {
            timer = _tickRate;
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
        }

        public void PlayPauseLife()
        {
            isPaused = !isPaused;
        }

        public void AdvanceLife()
        {
            _processLifeSystem.Update();
        }
    }
}