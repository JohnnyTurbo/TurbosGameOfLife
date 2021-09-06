using System;
using Unity.Entities;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class GameOfLifeTick : MonoBehaviour
    {
        [SerializeField][Range(float.Epsilon, 5)] private float _tickRate;
        [SerializeField] private KeyCode _pauseKey;
        [SerializeField] private KeyCode _stepKey;

        private bool isPaused = false;
        private float timer;
        private ProcessLifeSystem _processLifeSystem;
        
        private void Start()
        {
            timer = _tickRate;
            _processLifeSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ProcessLifeSystem>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_pauseKey))
            {
                isPaused = !isPaused;
            }

            if (isPaused && Input.GetKeyDown(_stepKey))
            {
                _processLifeSystem.Update();
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
    }
}