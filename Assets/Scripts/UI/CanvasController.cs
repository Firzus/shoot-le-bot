using UnityEngine;
using TMPro;

namespace Project
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _healthText;

        private void OnEnable()
        {
            GameManager.Instance.OnScoreChanged += UpdateScoreText;
            GameManager.Instance.OnHealthChanged += UpdateHealthText;
        }

        private void UpdateScoreText(int newScore)
        {
            _scoreText.text = newScore.ToString();
        }

        private void UpdateHealthText(int newHealth)
        {
            _healthText.text = newHealth.ToString();
        }

        private void OnDisable()
        {
            GameManager.Instance.OnScoreChanged -= UpdateScoreText;
            GameManager.Instance.OnHealthChanged -= UpdateHealthText;
        }
    }
}