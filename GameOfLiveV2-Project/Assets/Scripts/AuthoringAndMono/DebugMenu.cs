using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class DebugMenu : MonoBehaviour
    {
        public GameOfLifeTick GameOfLifeTick;
        
        public float boxWidth;
        public float boxHeight;
        public float fpsPollRate;
        public float padding;
        
        private float timer;
        private float curFPSAverage;
        private List<float> fpsThisSecond;
        
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

            var playPauseString = GameOfLifeTick.IsPaused ? "Play" : "Pause";
            
            if(GUI.Button(new Rect(Screen.width - boxWidth, 110, 160, 50), playPauseString))
            {
                GameOfLifeTick.PlayPauseLife();
            }
            
            if(GUI.Button(new Rect(Screen.width - boxWidth + 170, 110, 160, 50), "Step"))
            {
                GameOfLifeTick.AdvanceLife();
            }

            GUI.Label(new Rect(Screen.width - boxWidth, 200, 160, 50), $"Tick Rate: {GameOfLifeTick._tickRate:N2}");
            GameOfLifeTick._tickRate = GUI.HorizontalSlider(new Rect(Screen.width - boxWidth, 230, 330, 50),
                GameOfLifeTick._tickRate, 0, 0.5f);
        }
    }
}
























