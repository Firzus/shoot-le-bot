using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// controlle le drag de la map sur x

namespace Project
{
    public class GameManager : MonoBehaviour
    {
        // Singleton pattern
        public static GameManager Instance { get; private set; }

        // Game states
        public enum GameState
        {
            Menu,
            Playing,
            Victory
        }

        [Header("Game Settings")]
        [SerializeField] private float _gameTime = 0f;
        public int CurrentScore { get; private set; }
        [SerializeField] private GameState _currentGameState = GameState.Menu;

        // Events
        public static event Action<int> OnScoreChanged;
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGameOver;
        public static event Action OnGameVictory;

        // Properties
        public GameState CurrentGameState => _currentGameState;
        public bool IsGamePlaying => _currentGameState == GameState.Playing;
        public bool IsGameMenu => _currentGameState == GameState.Menu;
        public bool IsGameVictory => _currentGameState == GameState.Victory;

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
            InitializeGame();
        }

        private void Update()
        {
            // HandleInput();

            if (IsGamePlaying)
            {
                UpdateGameTime();
            }
        }

        private void InitializeGame()
        {
            SetGameState(GameState.Menu);
            // UpdateUI();
        }

        private void UpdateGameTime()
        {
            _gameTime += Time.deltaTime;
        }

        public void StartGame()
        {
            SetGameState(GameState.Playing);
            _gameTime = 0f;
            CurrentScore = 0;
            OnGameStarted?.Invoke();
            // UpdateUI();
        }

        public void GameOver()
        {
            SetGameState(GameState.Victory);
            Time.timeScale = 0f;
            OnGameOver?.Invoke();
            // UpdateUI();
        }

        public void Victory()
        {
            SetGameState(GameState.Victory);
            Time.timeScale = 0f;
            OnGameVictory?.Invoke();
            // UpdateUI();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SetGameState(GameState.Menu);
            // UpdateUI();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public void AddScore(int points)
        {
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        private void SetGameState(GameState newState)
        {
            if (_currentGameState != newState)
            {
                _currentGameState = newState;
                OnGameStateChanged?.Invoke(newState);
            }
        }

        // private void UpdateUI()
        // {
        //     // Hide all UI elements first
        //     if (mainMenuUI) mainMenuUI.SetActive(false);
        //     if (gameUI) gameUI.SetActive(false);
        //     if (pauseUI) pauseUI.SetActive(false);
        //     if (gameOverUI) gameOverUI.SetActive(false);
        //     if (victoryUI) victoryUI.SetActive(false);

        //     // Show appropriate UI based on game state
        //     switch (currentGameState)
        //     {
        //         case GameState.MainMenu:
        //             if (mainMenuUI) mainMenuUI.SetActive(true);
        //             break;
        //         case GameState.Playing:
        //             if (gameUI) gameUI.SetActive(true);
        //             break;
        //         case GameState.Paused:
        //             if (pauseUI) pauseUI.SetActive(true);
        //             break;
        //         case GameState.GameOver:
        //             if (gameOverUI) gameOverUI.SetActive(true);
        //             break;
        //         case GameState.Victory:
        //             if (victoryUI) victoryUI.SetActive(true);
        //             break;
        //     }
        // }

        private void OnDestroy()
        {
            // Reset time scale when destroying
            Time.timeScale = 1f;
        }

        // Public methods for external access
        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}