using UnityEngine;

namespace EmotionCore.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public class ProceduralAudio : MonoBehaviour
    {
        [Header("Settings")]
        public float BaseFrequency = 120f;
        public float ModulationSpeed = 0.5f;
        public float Intensity = 0.1f;
        
        private double phase;
        private double increment;
        private float sampleRate;

        private void OnEnable()
        {
            sampleRate = AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            // Modulate pitch based on emotional intensity if possible
            if (EmotionCore.Core.EmotionEngine.Instance != null)
            {
                float emotionalFactor = EmotionCore.Core.EmotionEngine.Instance.EmotionalIntensity;
                BaseFrequency = Mathf.Lerp(120f, 60f, emotionalFactor); // Lower pitch = scarier
                Intensity = Mathf.Lerp(0.05f, 0.2f, emotionalFactor);
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            increment = BaseFrequency * 2.0 * Mathf.PI / sampleRate;

            for (int i = 0; i < data.Length; i += channels)
            {
                phase += increment;
                
                // Super simple FM Synthesis (Sine wave modulated by another sine)
                float modulator = Mathf.Sin((float)phase * 0.5f);
                float sample = Mathf.Sin((float)phase + modulator * 2.0f);

                // Add some noise for texture
                float noise = (float)(new System.Random().NextDouble() * 2.0 - 1.0) * 0.1f;
                
                sample = (sample + noise) * Intensity;

                if (phase > (Mathf.PI * 2)) phase = 0;

                data[i] = sample;
                if (channels == 2) data[i + 1] = sample;
            }
        }
    }
}
