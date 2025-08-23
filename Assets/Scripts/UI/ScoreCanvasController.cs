using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class ScoreCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _summaryText;
        [SerializeField] private Button _continueButton;

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

            _continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        private void OnGameStateChanged(GameManager.GameState gameState)
        {
            if (gameState == GameManager.GameState.Score)
            {
                _summaryText.text = UIManager.Instance.GetKilledBotsMessage();
            }
        }

        private void OnContinueButtonClicked()
        {
            GameManager.Instance.ChangeToMenuState();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}