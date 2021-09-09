using Unity.Mathematics;
using UnityEngine;

namespace TMG.GameOfLiveV2
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        [SerializeField] private float _normalScrollRate;
        [SerializeField] private float _fastScrollRate;
        [SerializeField] private float _minOrthographicSize;
        
        private float _maxOrthographicSize;
        private Camera mainCamera;
        private Vector3 _lastMousePosition;
        private Rect _cameraBoundary;
        
        private void Awake()
        {
            Instance = this;
            mainCamera = Camera.main;
        }

        public void SetToGridFullscreen(int2 gridSize)
        {
            mainCamera.transform.position = new Vector3(gridSize.x / 2f, gridSize.y / 2f, -10);
            
            mainCamera.orthographicSize = Mathf.Max(CalculateOrthographicSize(gridSize), _minOrthographicSize);

            _cameraBoundary = new Rect(0, 0, gridSize.x, gridSize.y);
        }

        private float CalculateOrthographicSize(int2 gridSize)
        {
            var longerSide = Mathf.Max(gridSize.x / mainCamera.aspect, gridSize.y);
            _maxOrthographicSize = Mathf.Max(_minOrthographicSize, longerSide / 2f * 1.1f);
            return longerSide / 2f;
        }


        public void Update()
        {
            ProcessCameraZoom();
            ProcessCameraMove();

            _lastMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void ProcessCameraMove()
        {
            if(!Input.GetMouseButton(2)){return;}

            var cameraMovement = _lastMousePosition - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var cameraPosition = mainCamera.transform.position + cameraMovement;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, _cameraBoundary.x,
                _cameraBoundary.x + _cameraBoundary.width);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, _cameraBoundary.y,
                _cameraBoundary.y + _cameraBoundary.height);

            mainCamera.transform.position = cameraPosition;
        }

        private void ProcessCameraZoom()
        {
            var mouseScrollDelta = Input.mouseScrollDelta;
            if(mouseScrollDelta == Vector2.zero){return;}
            var scrollThisStep = _normalScrollRate * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                scrollThisStep = _fastScrollRate * Time.deltaTime;
            }
            if (mouseScrollDelta.y > 0f)
            {
                //Debug.Log("Scroll up");
                mainCamera.orthographicSize -= scrollThisStep;
            }
            else if(mouseScrollDelta.y < 0f)
            {
                //Debug.Log("Scroll down");
                mainCamera.orthographicSize += scrollThisStep;
            }

            mainCamera.orthographicSize =
                Mathf.Clamp(mainCamera.orthographicSize, _minOrthographicSize, _maxOrthographicSize);
        }
    }
}