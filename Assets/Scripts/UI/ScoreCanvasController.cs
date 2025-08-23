using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class ScoreCanvasController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _summaryText;
        [SerializeField] private Button _continueButton;

        private void OnEnable()
        {
            _continueButton.onClick.AddListener(OnContinueButtonClicked);

            _summaryText.text = $"Bravo, vous avez banni {GameManager.Instance.CurrentScore} bots !";
        }

        private void OnContinueButtonClicked()
        {
            GameManager.Instance.ChangeToMenuState();
        }

        private void OnDisable()
        {
            _continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }
    }
}