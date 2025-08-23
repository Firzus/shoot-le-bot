using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Project
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _botPrefabs = new();
        [SerializeField] private AnimationCurve _spawnCurve;
        [SerializeField, Range(0f, 5f)] private float _timeMultiplier = 1f;
        [SerializeField, Range(0f, 10f)] private float _rateMultiplier = 1f;
        [SerializeField] private float _maxSpawnDurationSeconds = 60f;
        [SerializeField] private float _spawnStartDelaySeconds = 0f;
        [SerializeField, Range(0f, 1f)] private float _rateJitter = 0f;
        [SerializeField, Range(0f, 2f)] private float _noiseAmplitude = 0f;
        [SerializeField, Min(0f)] private float _noiseFrequency = 0.5f;
        [SerializeField] private int _noiseSeed = 0;

        private float _spawnAccumulator;
        private readonly List<IObjectPool<BotController>> _pools = new();
        private readonly List<System.Action<BotController>> _releaseHandlers = new();

        private void Awake()
        {
            EnsurePools();
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
            float ratePerSecond = 0f;
            if (_spawnCurve != null)
            {
                ratePerSecond = Mathf.Max(0f, _spawnCurve.Evaluate(normalizedTime) * _rateMultiplier);
            }

            if (_rateJitter > 0f && ratePerSecond > 0f)
            {
                float factor = Random.Range(1f - _rateJitter, 1f + _rateJitter);
                ratePerSecond = Mathf.Max(0f, ratePerSecond * factor);
            }

            if (_noiseAmplitude > 0f && ratePerSecond > 0f)
            {
                float noiseTime = GameManager.Instance.GameTime * _noiseFrequency;
                float noise = Mathf.PerlinNoise(_noiseSeed, noiseTime) * 2f - 1f; // [-1, 1]
                float noiseFactor = 1f + noise * _noiseAmplitude;
                ratePerSecond = Mathf.Max(0f, ratePerSecond * Mathf.Max(0f, noiseFactor));
            }

            _spawnAccumulator += ratePerSecond * Time.deltaTime;

            while (_spawnAccumulator >= 1f)
            {
                SpawnFromPool();
                _spawnAccumulator -= 1f;
            }
        }

        private void EnsurePools()
        {
            _pools.Clear();
            _releaseHandlers.Clear();
            if (_botPrefabs == null || _botPrefabs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _botPrefabs.Count; i++)
            {
                GameObject prefab = _botPrefabs[i];
                if (prefab == null)
                {
                    _pools.Add(null);
                    continue;
                }

                // capture local index for closures
                int capturedIndex = i;
                IObjectPool<BotController> pool = null;
                pool = new ObjectPool<BotController>(
                    createFunc: () =>
                    {
                        GameObject go = Instantiate(prefab);
                        BotController bot = go.GetComponent<BotController>();
                        if (bot == null)
                        {
                            bot = go.AddComponent<BotController>();
                        }
                        go.SetActive(false);
                        return bot;
                    },
                    actionOnGet: (bot) =>
                    {
                        bot.OnDespawnRequested = _releaseHandlers[capturedIndex];
                        bot.gameObject.SetActive(true);
                        bot.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                        bot.OnSpawned();
                    },
                    actionOnRelease: (bot) =>
                    {
                        bot.gameObject.SetActive(false);
                        bot.OnDespawnRequested = null;
                    },
                    actionOnDestroy: (bot) =>
                    {
                        if (bot != null)
                        {
                            Destroy(bot.gameObject);
                        }
                    },
                    defaultCapacity: 10,
                    maxSize: 200
                );

                _pools.Add(pool);
                _releaseHandlers.Add((b) =>
                {
                    _pools[capturedIndex].Release(b);
                });
            }
        }

        private void SpawnFromPool()
        {
            if (_botPrefabs == null || _botPrefabs.Count == 0)
            {
                return;
            }

            int index = Random.Range(0, _botPrefabs.Count);
            IObjectPool<BotController> pool = (index >= 0 && index < _pools.Count) ? _pools[index] : null;
            if (pool == null)
            {
                return;
            }

            pool.Get();
        }

        public void DespawnAllBots()
        {
            var bots = FindObjectsByType<BotController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var bot in bots)
            {
                if (bot != null)
                {
                    Destroy(bot.gameObject);
                }
            }
            _spawnAccumulator = 0f;
        }
    }
}