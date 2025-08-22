using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Project
{
    public class ClickManager : MonoBehaviour
    {
        public static ClickManager Instance { get; private set; }

        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _clickableLayer = ~0; // default: all layers
        [SerializeField] private bool _includeTriggerColliders = true;

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
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (Mouse.current == null)
            {
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var screenPos = Mouse.current.position.ReadValue();
            var worldPos3 = _camera != null ? _camera.ScreenToWorldPoint(screenPos) : (Vector3)screenPos;
            Vector2 worldPos = new(worldPos3.x, worldPos3.y);

            var overlaps = Physics2D.OverlapPointAll(worldPos, _clickableLayer);
            for (int i = 0; i < overlaps.Length; i++)
            {
                var col = overlaps[i];
                if (col == null)
                {
                    continue;
                }
                if (!_includeTriggerColliders && col.isTrigger)
                {
                    continue;
                }
                if (col.TryGetComponent<IClickable>(out var clickable))
                {
                    clickable.OnClick(worldPos);
                    break; // click the top-most match only
                }
            }
        }
    }
}


