using UnityEngine;

namespace Project
{
    public class CameraManager : MonoBehaviour
    {
        // Singleton pattern
        public static CameraManager Instance { get; private set; }

        public enum CameraPosition
        {
            Left,
            Center,
            Right
        }

        public CameraPosition CurrentCameraPosition { get; private set; }

        private Transform _camera;
        [SerializeField] private float _cameraStepOffset = 10f;
        [SerializeField] private AnimationCurve _cameraAnimationTransition;

        private void Awake()
        {
            // Singleton pattern implementation
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _camera = Camera.main.transform;

            ResetCameraPosition();
        }

        public void UpdateCameraPosition(CameraPosition newPosition)
        {
            switch (newPosition)
            {
                case CameraPosition.Left:
                    _camera.position = new Vector3(0, _camera.position.y, _camera.position.z);
                    CurrentCameraPosition = CameraPosition.Left;
                    break;
                case CameraPosition.Center:
                    _camera.position = new Vector3(_cameraStepOffset, _camera.position.y, _camera.position.z);
                    CurrentCameraPosition = CameraPosition.Center;
                    break;
                case CameraPosition.Right:
                    _camera.position = new Vector3(_cameraStepOffset * 2, _camera.position.y, _camera.position.z);
                    CurrentCameraPosition = CameraPosition.Right;
                    break;
            }
        }

        public void ResetCameraPosition()
        {
            CurrentCameraPosition = CameraPosition.Left;
        }
    }
}