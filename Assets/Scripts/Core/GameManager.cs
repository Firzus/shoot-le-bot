using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Score
        }

        [Header("Game Settings")]
        [SerializeField] private float _gameTime = 0f;
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private GameState _currentGameState;

        // Events
        public event Action<int> OnScoreChanged;
        public event Action<int> OnHealthChanged;
        public event Action<GameState> OnGameStateChanged;

        // Gameplay
        private int _currentScore;
        private int _currentHealth;

        // API
        public GameState CurrentGameState => _currentGameState;
        public int PlayerDamage => _playerData.Damage;
        public float GameTime => _gameTime;

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

            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            ChangeToMenuState();
        }

        private void Update()
        {
            if (_currentGameState == GameState.Playing)
            {
                UpdateGameTime();
            }
        }

        private void UpdateGameTime()
        {
            _gameTime += Time.deltaTime;
        }

        public void ChangeToMenuState()
        {
            SetGameState(GameState.Menu);
        }

        public void ChangeToGameState()
        {
            SetGameState(GameState.Playing);
            Time.timeScale = 1f;
            _gameTime = 0f;
            ResetScore();
            ResetHealth();
        }

        public void ChangeToScoreState()
        {
            SetGameState(GameState.Score);
            Time.timeScale = 0f;
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
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void SetScore(int score)
        {
            _currentScore = score;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void ResetScore()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void ResetHealth()
        {
            _currentHealth = _playerData.MaxHealth;
            OnHealthChanged?.Invoke(_currentHealth);
        }

        public void SetHealth(int health)
        {
            _currentHealth = health;
            OnHealthChanged?.Invoke(_currentHealth);
        }

        private void SetGameState(GameState newState)
        {
            if (_currentGameState != newState)
            {
                _currentGameState = newState;
                OnGameStateChanged?.Invoke(newState);
            }
        }

        public void PlayerTakeDamage(int damage)
        {
            _currentHealth -= damage;

            OnHealthChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0)
            {
                ChangeToScoreState();
            }
        }
    }
}