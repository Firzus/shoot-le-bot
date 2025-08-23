using TMPro;
using UnityEngine;

namespace Project
{
    public class ScoreCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _summaryText;

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            if (gameState == GameManager.GameState.Score)
            {
                _summaryText.text = UIManager.Instance.GetKilledBotsMessage();
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}