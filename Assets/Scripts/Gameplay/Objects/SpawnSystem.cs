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
        [Range(0f, 1f)] public float minNormalizedRate = 0.15f; // ensures non-zero start rate
        [Range(0.25f, 4f)] public float curvature = 0.8f; // <1 boosts early growth, >1 slows it
    }

    public static class SpawnRateUtility
    {
        public static float EvaluateNormalizedRate(float normalizedTime, SpawnRateConfig config)
        {
            float t = Mathf.Clamp01(normalizedTime);
            SpawnRateFormula formula = config != null ? config.formula : SpawnRateFormula.Linear;

            float baseRate;

            switch (formula)
            {
                case SpawnRateFormula.Exponential:
                    {
                        float g = config != null ? Mathf.Max(1.0001f, config.exponentialGrowth) : 4f;
                        baseRate = (Mathf.Pow(g, t) - 1f) / (g - 1f);
                        break;
                    }
                case SpawnRateFormula.Logistic:
                    {
                        float steep = config != null ? config.logisticSteepness : 6f;
                        float mid = config != null ? config.logisticMidpoint : 0.5f;
                        float f = 1f / (1f + Mathf.Exp(-steep * (t - mid)));
                        float f0 = 1f / (1f + Mathf.Exp(-steep * (0f - mid)));
                        float f1 = 1f / (1f + Mathf.Exp(-steep * (1f - mid)));
                        baseRate = (f - f0) / Mathf.Max(1e-5f, (f1 - f0));
                        break;
                    }
                case SpawnRateFormula.Linear:
                default:
                    baseRate = t;
                    break;
            }

            // Apply curvature to bias early or late growth
            float curvature = config != null ? Mathf.Clamp(config.curvature, 0.25f, 4f) : 1f;
            if (!Mathf.Approximately(curvature, 1f))
            {
                baseRate = Mathf.Pow(Mathf.Clamp01(baseRate), curvature);
            }

            // Apply a minimum normalized rate so spawning starts immediately
            float min = config != null ? Mathf.Clamp01(config.minNormalizedRate) : 0f;
            return Mathf.Lerp(min, 1f, Mathf.Clamp01(baseRate));
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


