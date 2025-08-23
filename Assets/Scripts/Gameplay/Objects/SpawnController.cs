using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _botPrefabs = new();
        [SerializeField] private SpawnRateConfig _rateConfig = new();

        [SerializeField, Range(0f, 5f)] private float _timeMultiplier = 1f;
        [SerializeField, Range(0f, 10f)] private float _rateMultiplier = 1f;
        [SerializeField] private float _maxSpawnDurationSeconds = 60f;
        [SerializeField] private float _spawnStartDelaySeconds = 0f;

        private float _spawnAccumulator;
        private readonly BotPoolManager _botPoolManager = new();

        private void Awake()
        {
            _botPoolManager.Initialize(_botPrefabs, transform);
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
            {
                return;
            }

            float rawElapsed = GameManager.Instance.GameTime - _spawnStartDelaySeconds;
            if (rawElapsed <= 0f)
            {
                return;
            }
            float elapsed = Mathf.Min(rawElapsed, _maxSpawnDurationSeconds);

            float normalizedTime = Mathf.Clamp01(elapsed / _maxSpawnDurationSeconds * _timeMultiplier);
            float rateNormalized = SpawnRateUtility.EvaluateNormalizedRate(normalizedTime, _rateConfig);
            float ratePerSecond = Mathf.Max(0f, rateNormalized * _rateMultiplier);

            _spawnAccumulator += ratePerSecond * Time.deltaTime;

            while (_spawnAccumulator >= 1f)
            {
                _botPoolManager.SpawnRandom();
                _spawnAccumulator -= 1f;
            }
        }

        public void DespawnAllBots()
        {
            _botPoolManager.DespawnAllBots();
            _spawnAccumulator = 0f;
        }
    }
}