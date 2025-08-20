using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    // Game states
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    [Header("Game Settings")]
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int highScore = 0;
    [SerializeField] private GameState currentGameState = GameState.MainMenu;

    [Header("UI References")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;

    // Events
    public static event Action<int> OnScoreChanged;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStarted;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnGameOver;
    public static event Action OnGameVictory;

    // Properties
    public float GameTime => gameTime;
    public int CurrentScore => currentScore;
    public int HighScore => highScore;
    public GameState CurrentGameState => currentGameState;
    public bool IsGamePlaying => currentGameState == GameState.Playing;
    public bool IsGamePaused => currentGameState == GameState.Paused;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
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
        HandleInput();

        if (IsGamePlaying)
        {
            UpdateGameTime();
        }
    }

    private void InitializeGame()
    {
        SetGameState(GameState.MainMenu);
        UpdateUI();
    }

    private void HandleInput()
    {
        // Pause/Resume with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePlaying)
            {
                PauseGame();
            }
            else if (IsGamePaused)
            {
                ResumeGame();
            }
        }

        // Restart game with R key
        if (Input.GetKeyDown(KeyCode.R) && (IsGamePlaying || IsGamePaused))
        {
            RestartGame();
        }
    }

    private void UpdateGameTime()
    {
        gameTime += Time.deltaTime;
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        gameTime = 0f;
        currentScore = 0;
        OnGameStarted?.Invoke();
        UpdateUI();
    }

    public void PauseGame()
    {
        if (IsGamePlaying)
        {
            SetGameState(GameState.Paused);
            Time.timeScale = 0f;
            OnGamePaused?.Invoke();
            UpdateUI();
        }
    }

    public void ResumeGame()
    {
        if (IsGamePaused)
        {
            SetGameState(GameState.Playing);
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
            UpdateUI();
        }
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        Time.timeScale = 0f;
        CheckHighScore();
        OnGameOver?.Invoke();
        UpdateUI();
    }

    public void Victory()
    {
        SetGameState(GameState.Victory);
        Time.timeScale = 0f;
        CheckHighScore();
        OnGameVictory?.Invoke();
        UpdateUI();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SetGameState(GameState.MainMenu);
        UpdateUI();
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
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void SetScore(int score)
    {
        currentScore = score;
        OnScoreChanged?.Invoke(currentScore);
    }

    private void CheckHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }

    private void SetGameState(GameState newState)
    {
        if (currentGameState != newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }

    private void UpdateUI()
    {
        // Hide all UI elements first
        if (mainMenuUI) mainMenuUI.SetActive(false);
        if (gameUI) gameUI.SetActive(false);
        if (pauseUI) pauseUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (victoryUI) victoryUI.SetActive(false);

        // Show appropriate UI based on game state
        switch (currentGameState)
        {
            case GameState.MainMenu:
                if (mainMenuUI) mainMenuUI.SetActive(true);
                break;
            case GameState.Playing:
                if (gameUI) gameUI.SetActive(true);
                break;
            case GameState.Paused:
                if (pauseUI) pauseUI.SetActive(true);
                break;
            case GameState.GameOver:
                if (gameOverUI) gameOverUI.SetActive(true);
                break;
            case GameState.Victory:
                if (victoryUI) victoryUI.SetActive(true);
                break;
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void OnDestroy()
    {
        // Reset time scale when destroying
        Time.timeScale = 1f;
    }

    // Public methods for external access
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.DeleteKey("HighScore");
    }

    public string GetFormattedGameTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
