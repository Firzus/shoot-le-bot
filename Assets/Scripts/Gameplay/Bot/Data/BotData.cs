using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "BotData", menuName = "Data/Bot/BotData")]
    public class BotData : ScriptableObject
    {
        [Range(1, 10)] public float MoveSpeed;
        [Range(1, 10)] public int Damage;
        [Range(1, 10)] public int MaxHealth = 1;
        [Range(1, 10)] public int ScoreReward = 1;
    }
}