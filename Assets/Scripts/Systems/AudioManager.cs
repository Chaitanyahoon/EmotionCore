using UnityEngine;

namespace EmotionCore.Systems
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioSource VoiceSource;
        public AudioSource AmbienceSource;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void PlayVoice(AudioClip clip)
        {
            if (VoiceSource != null && clip != null)
            {
                VoiceSource.pitch = 1.0f + Random.Range(-0.05f, 0.05f); // Natural variation
                VoiceSource.PlayOneShot(clip);
            }
        }

        public void PlayGlitchSound()
        {
            // Trigger static noise
        }

        public void SetAmbienceIntensity(float intensity)
        {
            if (AmbienceSource != null)
            {
                // Lerp volume or pitch based on intensity
                AmbienceSource.pitch = 1.0f - (intensity * 0.5f); // Lower pitch = scarier
            }
        }
    }
}
