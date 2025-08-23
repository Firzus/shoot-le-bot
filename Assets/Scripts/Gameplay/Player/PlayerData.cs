using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int MaxHealth = 3;
        public int Damage = 1;
        public AudioClip HitSound;
        public Sprite HitEffect;
    }
}