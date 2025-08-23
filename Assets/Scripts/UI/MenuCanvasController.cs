using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class MenuCanvasController : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        // Volume
        [SerializeField] private Button _volumeButton;
        [SerializeField] private Image _volumeImage;
        [SerializeField] private Sprite _volumeOnSprite;
        [SerializeField] private Sprite _volumeOffSprite;

        private void Awake()
        {
            _volumeImage.sprite = _volumeImage.sprite == _volumeOnSprite ? _volumeOffSprite : _volumeOnSprite;
        }

        private void Start()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _volumeButton.onClick.AddListener(OnVolumeButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            GameManager.Instance.ChangeToGameState();
        }

        private void OnQuitButtonClicked()
        {
            GameManager.Instance.QuitGame();
        }

        private void OnVolumeButtonClicked()
        {
            AudioManager.Instance.ToggleMasterVolume();
            _volumeImage.sprite = _volumeImage.sprite == _volumeOnSprite ? _volumeOffSprite : _volumeOnSprite;
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClicked);
            _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            _volumeButton.onClick.RemoveListener(OnVolumeButtonClicked);
        }
    }
}