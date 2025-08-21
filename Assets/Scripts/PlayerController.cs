using UnityEngine;

namespace Project
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField] private PlayerData _playerData;


        // 
        [SerializeField] private int _currentHealth;

        private void Awake()
        {
            _playerData.maxHealth = 100;
        }

        public void TakeDamage(int damage)
        {
            throw new System.NotImplementedException();
        }
    }
}