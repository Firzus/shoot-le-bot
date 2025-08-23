using UnityEngine;
using TMPro;

namespace Project
{
    public class GameCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _healthText;

        private void OnEnable()
        {
            GameManager.OnScoreChanged += UpdateScoreText;
            GameManager.OnHealthChanged += UpdateHealthText;
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
            GameManager.OnScoreChanged -= UpdateScoreText;
            GameManager.OnHealthChanged -= UpdateHealthText;
        }
    }
}