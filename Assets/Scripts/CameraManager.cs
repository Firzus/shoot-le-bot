using System;
using UnityEngine;

namespace Project
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraPosition _cameraPosition;
        [SerializeField] private float _cameraStepOffset = 10f;

        public static event Action<CameraPosition> OnCameraPositionChanged;

        // move camera according to target
        public enum CameraPosition
        {
            Left,
            Center,
            Right
        }

        private void Awake()
        {
            _camera = Camera.main;
        }


        private void Start()
        {
            //
        }

        private void Update()
        {
            // move camera according to target
            _camera.transform.position = _target.position;
        }
    }
}