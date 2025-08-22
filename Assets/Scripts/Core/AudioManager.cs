using UnityEngine;

namespace Project
{
    public class AudioManager : MonoBehaviour
    {
        // Singleton pattern
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern implementation
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
    }
}
