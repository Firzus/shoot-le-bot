using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Collections.Generic;


namespace Project
{


    public enum SpawnRateFormula
    {
        Linear,
        Exponential,
        Logistic
    }

    [System.Serializable]
    public class SpawnRateConfig
    {
        public SpawnRateFormula formula = SpawnRateFormula.Linear;
        [Min(1.0001f)] public float exponentialGrowth = 4f;
        [Range(0.1f, 20f)] public float logisticSteepness = 6f;
        [Range(0f, 1f)] public float logisticMidpoint = 0.5f;
    }

    public static class SpawnRateUtility
    {
        public static float EvaluateNormalizedRate(float normalizedTime, SpawnRateConfig config)
        {
            float t = Mathf.Clamp01(normalizedTime);
            SpawnRateFormula formula = config != null ? config.formula : SpawnRateFormula.Linear;

            switch (formula)
            {
                case SpawnRateFormula.Exponential:
                    {
                        float g = config != null ? Mathf.Max(1.0001f, config.exponentialGrowth) : 4f;
                        return (Mathf.Pow(g, t) - 1f) / (g - 1f);
                    }
                case SpawnRateFormula.Logistic:
                    {
                        float steep = config != null ? config.logisticSteepness : 6f;
                        float mid = config != null ? config.logisticMidpoint : 0.5f;
                        float f = 1f / (1f + Mathf.Exp(-steep * (t - mid)));
                        float f0 = 1f / (1f + Mathf.Exp(-steep * (0f - mid)));
                        float f1 = 1f / (1f + Mathf.Exp(-steep * (1f - mid)));
                        return (f - f0) / Mathf.Max(1e-5f, (f1 - f0));
                    }
                case SpawnRateFormula.Linear:
                default:
                    return t;
            }
        }
    }

    public class BotPoolManager
    {
        private readonly List<IObjectPool<BotController>> _pools = new();
        private readonly List<System.Action<BotController>> _releaseHandlers = new();
        private Transform _spawnTransform;

        public void Initialize(List<GameObject> botPrefabs, Transform spawnTransform)
        {
            _pools.Clear();
            _releaseHandlers.Clear();
            _spawnTransform = spawnTransform;

            if (botPrefabs == null || botPrefabs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < botPrefabs.Count; i++)
            {
                GameObject prefab = botPrefabs[i];
                if (prefab == null)
                {
                    _pools.Add(null);
                    continue;
                }

                int capturedIndex = i;
                IObjectPool<BotController> pool = null;
                pool = new ObjectPool<BotController>(
                    createFunc: () =>
                    {
                        GameObject go = UnityEngine.Object.Instantiate(prefab);
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
                        bot.transform.SetPositionAndRotation(_spawnTransform.position, Quaternion.identity);
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
                            UnityEngine.Object.Destroy(bot.gameObject);
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

        public void SpawnRandom()
        {
            if (_pools.Count == 0)
            {
                return;
            }

            int index = UnityEngine.Random.Range(0, _pools.Count);
            IObjectPool<BotController> pool = (index >= 0 && index < _pools.Count) ? _pools[index] : null;
            if (pool == null)
            {
                return;
            }

            pool.Get();
        }

        public void DespawnAllBots()
        {
            var bots = UnityEngine.Object.FindObjectsByType<BotController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var bot in bots)
            {
                if (bot != null)
                {
                    UnityEngine.Object.Destroy(bot.gameObject);
                }
            }
        }
    }
}


