using UnityEngine;
using UnityEngine.Audio;

namespace Project
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource sfxObject;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // Mixer parameters

        public void SetMasterVolume(float level)
        {
            _audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20);
        }

        public void ToggleMasterVolume()
        {
            _audioMixer.GetFloat("MasterVolume", out float level);
            _audioMixer.SetFloat("MasterVolume", level == 0 ? -80 : 0);
        }

        // SFX
        public void PlaySFX(AudioClip clip, Transform spawnTransform, float volume = 1f)
        {
            AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clip.length);
        }
    }
}
