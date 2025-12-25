using UnityEngine;
using EmotionCore.Systems;

namespace EmotionCore.Puzzles
{
    public class MicPuzzle : PuzzleBase
    {
        [Header("Mic Settings")]
        public float Sensitivity = 0.1f;
        public float QuietThreshold = 0.05f;
        public float LoudThreshold = 0.3f;
        
        private AudioSource _micInput;
        private string _device;

        [Header("State")]
        public bool IsListening = false;
        public float AverageVolume = 0f;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "MicCheck";
            InitMic();
        }

        private void InitMic()
        {
            if (Microphone.devices.Length > 0)
            {
                _device = Microphone.devices[0];
                _micInput = gameObject.AddComponent<AudioSource>();
                _micInput.clip = Microphone.Start(_device, true, 10, 44100);
                _micInput.loop = true;
                
                // Mute playback so we don't hear ourselves
                _micInput.mute = true; 
                
                while (!(Microphone.GetPosition(_device) > 0)) { } // Wait
                _micInput.Play();
                IsListening = true;
                Debug.Log($"[MicPuzzle] Listening on {_device}");
            }
            else
            {
                Debug.LogWarning("[MicPuzzle] No Microphone found!");
                // Auto-solve or bypass
            }
        }

        private void Update()
        {
            if (!IsListening) return;

            float vol = GetVolume();
            AverageVolume = Mathf.Lerp(AverageVolume, vol, Time.deltaTime * 2f);

            // Logic: High stress (breathing) vs Silence
            if (AverageVolume > LoudThreshold)
            {
                // Player is noisy/stressed
                OnHighNoise();
            }
            else if (AverageVolume < QuietThreshold)
            {
                // Player is silent
                OnSilence();
            }
        }

        private void OnHighNoise()
        {
            if (IsSolved) return;
            // "Hey... you're stressed. It's ok."
            // UI.UIControlManager.Instance.ShowDialogue("Breathe... I'm here.");
        }

        private void OnSilence()
        {
             // Puzzles get harder or atmosphere gets creepier
        }

        private float GetVolume()
        {
            float[] data = new float[256];
            float a = 0;
            _micInput.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 256;
        }

        public void StopMic()
        {
            Microphone.End(_device);
            IsListening = false;
        }
    }
}
