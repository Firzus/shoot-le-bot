using UnityEngine;

namespace Project
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameObject _menuCanvas;
        [SerializeField] private GameObject _gameCanvas;
        [SerializeField] private GameObject _scoreCanvas;

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
            GameManager.OnGameStateChanged += UpdateUI;

            UpdateUI(GameManager.GameState.Menu);
        }

        public void UpdateUI(GameManager.GameState gameState)
        {
            switch (gameState)
            {
                case GameManager.GameState.Menu:
                    _menuCanvas.SetActive(true);
                    _gameCanvas.SetActive(false);
                    _scoreCanvas.SetActive(false);
                    break;
                case GameManager.GameState.Playing:
                    _gameCanvas.SetActive(true);
                    _menuCanvas.SetActive(false);
                    _scoreCanvas.SetActive(false);
                    break;
                case GameManager.GameState.Score:
                    _scoreCanvas.SetActive(true);
                    _menuCanvas.SetActive(false);
                    _gameCanvas.SetActive(false);
                    break;
            }
        }

        public void ShowHitEffect(Vector2 worldPosition)
        {
            var gameCanvas = _gameCanvas.GetComponent<Canvas>();
            var effectSprite = GameManager.Instance.HitEffect;

            var go = new GameObject("HitEffect");
            go.transform.SetParent(_gameCanvas.transform, false);
            go.transform.SetAsLastSibling(); // ensure on top

            var image = go.AddComponent<UnityEngine.UI.Image>();
            image.sprite = effectSprite;
            image.raycastTarget = false;
            image.SetNativeSize();

            // Convert world to screen to canvas space
            bool isOverlay = gameCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
            var cam = isOverlay ? null : (gameCanvas.worldCamera != null ? gameCanvas.worldCamera : Camera.main);
            var referenceCam = cam == null ? Camera.main : cam;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(referenceCam, worldPosition);

            var canvasRect = _gameCanvas.GetComponent<RectTransform>();
            var rt = image.rectTransform;
            if (gameCanvas.renderMode == RenderMode.WorldSpace)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPos, cam, out var worldPoint))
                {
                    rt.position = worldPoint;
                }
            }
            else
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out var localPoint))
                {
                    rt.anchoredPosition = localPoint;
                }
            }

            var fade = go.AddComponent<UIAutoFadeAndDestroy>();
            fade.Duration = 0.2f;
            fade.ScaleFrom = 0.75f;
            fade.ScaleTo = 1.25f;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= UpdateUI;
        }
    }
}