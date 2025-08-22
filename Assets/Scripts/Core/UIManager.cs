using UnityEngine;

namespace Project
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameObject _menuCanvas;
        [SerializeField] private GameObject _gameCanvas;
        [SerializeField] private GameObject _victoryCanvas;

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
            GameManager.Instance.OnGameStateChanged += UpdateUI;

            UpdateUI(GameManager.GameState.Menu);
        }

        public void UpdateUI(GameManager.GameState gameState)
        {
            switch (gameState)
            {
                case GameManager.GameState.Menu:
                    _menuCanvas.SetActive(true);
                    _gameCanvas.SetActive(false);
                    _victoryCanvas.SetActive(false);
                    break;
                case GameManager.GameState.Playing:
                    _gameCanvas.SetActive(true);
                    _menuCanvas.SetActive(false);
                    _victoryCanvas.SetActive(false);
                    break;
                case GameManager.GameState.Victory:
                    _victoryCanvas.SetActive(true);
                    _menuCanvas.SetActive(false);
                    _gameCanvas.SetActive(false);
                    break;
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
        }
    }
}