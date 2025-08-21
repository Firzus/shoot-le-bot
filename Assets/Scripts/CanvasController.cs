using UnityEngine;
using TMPro;

namespace Project
{
    public class CanvasController : MonoBehaviour
    {

        // référence les gameobjects UI et met à jour les valeurs correspondantes
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Start()
        {
            GameManager.OnScoreChanged += UpdateScoreText;
        }

        private void UpdateScoreText(int newScore)
        {
            _scoreText.text = newScore.ToString();
        }

        private void OnDestroy()
        {
            GameManager.OnScoreChanged -= UpdateScoreText;
        }
    }
}