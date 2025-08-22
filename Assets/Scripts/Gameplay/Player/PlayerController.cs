using UnityEngine;

namespace Project
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;

        private void Start()
        {
            _inputReader.EnablePlayerActions();
            _inputReader.Move += OnMove;
        }

        private void OnMove(float direction)
        {
            switch (direction)
            {
                case 1:
                    if (CameraManager.Instance.CurrentCameraPosition == CameraManager.CameraPosition.Right) return;
                    CameraManager.Instance.UpdateCameraPosition(CameraManager.Instance.CurrentCameraPosition + 1);
                    break;
                case -1:
                    if (CameraManager.Instance.CurrentCameraPosition == CameraManager.CameraPosition.Left) return;
                    CameraManager.Instance.UpdateCameraPosition(CameraManager.Instance.CurrentCameraPosition - 1);
                    break;
            }
        }

        private void OnDestroy()
        {
            _inputReader.Move -= OnMove;
        }
    }
}