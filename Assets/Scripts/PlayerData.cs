using UnityEngine;

namespace Project
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data / PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int maxHealth = 3;
    }
}