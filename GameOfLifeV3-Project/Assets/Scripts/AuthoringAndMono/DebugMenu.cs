using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV3
{
    public class DebugMenu : MonoBehaviour
    {
        public GameOfLifeMonoController _gameOfLifeMonoController;

        [SerializeField] private float _debugPanelWidth;
        [SerializeField] private float _debugPanelHeight;
        [SerializeField] private float _fpsPollRate;
        [SerializeField] private float _padding;
        
        private float _timer;
        private float _curFPSAverage;
        private List<float> _fpsThisSecond;
        private string _newGridWidthString = "10";
        private string _newGridHeightString = "10";

        private void Start()
        {
            _timer = _fpsPollRate;
            _fpsThisSecond = new List<float>(500);
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(Screen.width - _debugPanelWidth - _padding, _padding, _debugPanelWidth, _debugPanelHeight),
                "Debug Menu");
            
            _timer -= Time.unscaledDeltaTime;
            _fpsThisSecond.Add(1f / Time.unscaledDeltaTime);
            if (_timer <= 0f)
            {
                var totalFrameTimes = 0f;
                foreach (var fps in _fpsThisSecond)
                {
                    totalFrameTimes += fps;
                }
                _curFPSAverage = totalFrameTimes / _fpsThisSecond.Count;
                _fpsThisSecond.Clear();
                _timer = _fpsPollRate;
            }
            GUI.Label(new Rect(Screen.width - _debugPanelWidth, 50, 160, 50), $"FPS: {_curFPSAverage:N2}");

            var playPauseString = _gameOfLifeMonoController.IsPaused ? "Play" : "Pause";
            if(GUI.Button(new Rect(Screen.width - _debugPanelWidth, 110, 160, 50), playPauseString))
            {
                _gameOfLifeMonoController.PlayPauseLife();
            }
            
            if(GUI.Button(new Rect(Screen.width - _debugPanelWidth + 170, 110, 160, 50), "Step"))
            {
                _gameOfLifeMonoController.AdvanceLife();
            }

            GUI.Label(new Rect(Screen.width - _debugPanelWidth, 200, 160, 50),
                $"Tick Rate: {_gameOfLifeMonoController._tickRate:N2}");
            _gameOfLifeMonoController._tickRate = GUI.HorizontalSlider(
                new Rect(Screen.width - _debugPanelWidth, 230, 330, 20), _gameOfLifeMonoController._tickRate, 0, 0.5f);

            GUI.Label(new Rect(Screen.width - _debugPanelWidth, 260, 160, 20),
                $"Grid Size: {_gameOfLifeMonoController.GridSize.ToString()}");
            var numTiles = _gameOfLifeMonoController.GridSize.x * _gameOfLifeMonoController.GridSize.y;
            GUI.Label(new Rect(Screen.width - _debugPanelWidth, 280, _debugPanelWidth - _padding, 20),
                $"Total Number of Tiles: {numTiles:N0}");

            GUI.Label(new Rect(Screen.width - _debugPanelWidth, 300, _debugPanelWidth - _padding, 20),
                $"Total Number of Entities: {_gameOfLifeMonoController.TotalEntityCount:N0}");

            _newGridWidthString = GUI.TextField(new Rect(Screen.width - _debugPanelWidth, 320, 160, 50),
                _newGridWidthString, 4);
            _newGridHeightString = GUI.TextField(new Rect(Screen.width - _debugPanelWidth + 170, 320, 160, 50),
                _newGridHeightString, 4);

            /*if(GUI.Button(new Rect(Screen.width - _debugPanelWidth, 390, 160, 50), "Resize"))
            {
                if (int.TryParse(_newGridWidthString, out var newGridWidth) &&
                    int.TryParse(_newGridHeightString, out var newGridHeight))
                {
                    _gameOfLifeMonoController.ResizeGrid(new int2(newGridWidth, newGridHeight));
                }
            }
            if(GUI.Button(new Rect(Screen.width - _debugPanelWidth + 170, 390, 160, 50), "Randomize"))
            {
                if (int.TryParse(_newGridWidthString, out var newGridWidth) &&
                    int.TryParse(_newGridHeightString, out var newGridHeight))
                {
                    _gameOfLifeMonoController.RandomizeGrid();
                }
            }*/
        }
    }
}
























