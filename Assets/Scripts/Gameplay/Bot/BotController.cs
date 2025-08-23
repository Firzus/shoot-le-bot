using UnityEngine;
using UnityEngine.AI;

namespace Project
{
    public class BotController : MonoBehaviour, IDamageable, IClickable
    {
        private GameObject _target;
        public System.Action<BotController> OnDespawnRequested { get; set; }

        [Header("Components")]
        [SerializeField] private BotData _botData;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [Header("Data")]
        [SerializeField] private int _currentHealth;

        private void Awake()
        {
            _target = GameObject.FindGameObjectWithTag("TargetZone");
        }

        private void Start()
        {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
            _navMeshAgent.speed = _botData.MoveSpeed;
            _currentHealth = _botData.MaxHealth;
        }

        private void Update()
        {
            Move();
        }

        public void Move()
        {
            _navMeshAgent.SetDestination(_target.transform.position);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("TargetZone"))
            {
                GameManager.Instance.PlayerTakeDamage(_botData.Damage);
                Die();
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (OnDespawnRequested != null)
            {
                OnDespawnRequested(this);
                return;
            }

            Destroy(gameObject);
        }

        public void OnClick(Vector2 position)
        {
            GameManager.Instance.AddScore(_botData.ScoreReward);

            TakeDamage(GameManager.Instance.PlayerDamage);
        }

        public void OnSpawned()
        {
            _currentHealth = _botData.MaxHealth;
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = _botData.MoveSpeed;
                _navMeshAgent.ResetPath();
            }
        }
    }
}