using UnityEngine;

namespace EmotionCore.Systems
{
    public class MicrophoneListener : MonoBehaviour
    {
        public static MicrophoneListener Instance { get; private set; }

        private string _device;
        private AudioClip _clip;
        private int _sampleWindow = 128;
        
        [Header("Telemetry")]
        public float CurrentLoudness = 0f;
        public bool IsBreathing = false;
        public float SilenceThreshold = 0.05f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            if (Microphone.devices.Length > 0)
            {
                _device = Microphone.devices[0];
                _clip = Microphone.Start(_device, true, 10, 44100);
                Debug.Log($"[Mic] Listening on {_device}");
            }
            else
            {
                Debug.LogWarning("[Mic] No microphone found!");
            }
        }

        private void Update()
        {
            if (_clip == null) return;

            CurrentLoudness = GetLoudnessFromMicrophone();
            
            // Analyze breathing patterns (if constant low noise)
            IsBreathing = (CurrentLoudness > 0.01f && CurrentLoudness < 0.15f);

            // Feed to Emotion Engine
            // If loudness spikes -> Fear or Anger detected
            if (CurrentLoudness > 0.5f)
            {
                // Player shouted?
                Debug.Log("[Mic] High Volume Detected!");
            }
        }

        private float GetLoudnessFromMicrophone()
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(_device), _clip);
        }

        private float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
        {
            int startPosition = clipPosition - _sampleWindow;
            if (startPosition < 0) return 0;

            float[] waveData = new float[_sampleWindow];
            clip.GetData(waveData, startPosition);

            // RMS algorithm
            float totalLoudness = 0;
            for (int i = 0; i < _sampleWindow; i++) {
                totalLoudness += Mathf.Abs(waveData[i]);
            }
            return totalLoudness / _sampleWindow;
        }
    }
}
