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

        public string GetKilledBotsMessage()
        {
            return $"Bravo, vous avez banni {GameManager.Instance.CurrentScore} bots !";
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
        }
    }
}