using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace TMG.GameOfLiveV2
{
    public class DebugMenu : MonoBehaviour
    {
        public GameOfLifeMonoController _gameOfLifeMonoController;
        
        public float boxWidth;
        public float boxHeight;
        public float fpsPollRate;
        public float padding;
        
        private float timer;
        private float curFPSAverage;
        private List<float> fpsThisSecond;
        private string _newGridWidthString = "10";
        private string _newGridHeightString = "10";

        private void Start()
        {
            timer = fpsPollRate;
            fpsThisSecond = new List<float>(500);
        }

        private void OnGUI()
        {
            
            
            GUI.Box(new Rect(Screen.width - boxWidth - padding, padding, boxWidth, boxHeight), "Debug Menu");
            timer -= Time.unscaledDeltaTime;
            fpsThisSecond.Add(1f / Time.unscaledDeltaTime);
            if (timer <= 0f)
            {
                var totalFrameTimes = 0f;
                foreach (var fps in fpsThisSecond)
                {
                    totalFrameTimes += fps;
                }
                curFPSAverage = totalFrameTimes / fpsThisSecond.Count;
                fpsThisSecond.Clear();
                timer = fpsPollRate;
            }
            GUI.Label(new Rect(Screen.width - boxWidth, 50, 160, 50), $"FPS: {curFPSAverage:N2}");

            var playPauseString = _gameOfLifeMonoController.IsPaused ? "Play" : "Pause";
            
            if(GUI.Button(new Rect(Screen.width - boxWidth, 110, 160, 50), playPauseString))
            {
                _gameOfLifeMonoController.PlayPauseLife();
            }
            
            if(GUI.Button(new Rect(Screen.width - boxWidth + 170, 110, 160, 50), "Step"))
            {
                _gameOfLifeMonoController.AdvanceLife();
            }

            GUI.Label(new Rect(Screen.width - boxWidth, 200, 160, 50), $"Tick Rate: {_gameOfLifeMonoController._tickRate:N2}");
            _gameOfLifeMonoController._tickRate = GUI.HorizontalSlider(new Rect(Screen.width - boxWidth, 230, 330, 20),
                _gameOfLifeMonoController._tickRate, 0, 0.5f);

            _newGridWidthString = GUI.TextField(new Rect(Screen.width - boxWidth, 260, 160, 50), _newGridWidthString, 4);
            _newGridHeightString = GUI.TextField(new Rect(Screen.width - boxWidth + 170, 260, 160, 50), _newGridHeightString, 4);

            if(GUI.Button(new Rect(Screen.width - boxWidth, 320, 160, 50), "Resize"))
            {
                if (int.TryParse(_newGridWidthString, out var newGridWidth) &&
                    int.TryParse(_newGridHeightString, out var newGridHeight))
                {
                    _gameOfLifeMonoController.ResizeGrid(new int2(newGridWidth, newGridHeight));
                }
                
            }
        }
    }
}
























