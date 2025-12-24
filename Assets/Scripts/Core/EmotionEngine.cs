using UnityEngine;
using System;

namespace EmotionCore.Core
{
    public class EmotionEngine : MonoBehaviour
    {
        public static EmotionEngine Instance { get; private set; }

        public enum EmotionState
        {
            Calm,       // Neutral, helpful
            Playful,    // Joking, friendly
            Affectionate, // "Warm" attachment
            Insecure,   // Doubting player loyalty
            Possessive, // Controlling, "safe" logic
            Desperate,  // Begging, panic
            Broken      // Glitched, corrupted
        }

        public EmotionState CurrentEmotion { get; private set; } = EmotionState.Calm;
        
        [Range(0f, 1f)]
        public float EmotionalIntensity = 0f;

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

        public void SetEmotion(EmotionState newState, float intensity = 0.5f)
        {
            CurrentEmotion = newState;
            EmotionalIntensity = intensity;
            Debug.Log($"[EmotionEngine] Mood Shift: {CurrentEmotion} (Intensity: {EmotionalIntensity})");
            
            ApplyEmotionalEffects();
        }

        public void OnActChange(GameManager.GameAct newAct)
        {
            switch (newAct)
            {
                case GameManager.GameAct.Act1_WarmUp:
                    SetEmotion(EmotionState.Calm, 0.2f);
                    break;
                case GameManager.GameAct.Act2_Discomfort:
                    SetEmotion(EmotionState.Insecure, 0.4f);
                    break;
                case GameManager.GameAct.Act3_Attachment:
                    SetEmotion(EmotionState.Affectionate, 0.7f);
                    break;
                case GameManager.GameAct.Act4_Possession:
                    SetEmotion(EmotionState.Possessive, 0.9f);
                    break;
                case GameManager.GameAct.Final_Breakup:
                    SetEmotion(EmotionState.Desperate, 1.0f);
                    break;
            }
        }

        private void Update()
        {
            // Reactive Logic (Phase 2)
            if (Systems.TrustEngine.Instance != null)
            {
                float trust = Systems.TrustEngine.Instance.PlayerProfile.TrustScore;
                
                // If Trust drops critically low, override emotion to Broken or Possessive
                if (trust < 20f && CurrentEmotion != EmotionState.Broken && CurrentEmotion != EmotionState.Possessive)
                {
                    SetEmotion(EmotionState.Possessive, 0.8f);
                    UI.UIControlManager.Instance.ShowDialogue("I don't trust you anymore.");
                }

                // If high trust in late game, become affectionate
                if (trust > 80f && GameManager.Instance.CurrentAct == GameManager.GameAct.Act3_Attachment 
                    && CurrentEmotion != EmotionState.Affectionate)
                {
                    SetEmotion(EmotionState.Affectionate, 0.6f);
                }
            }
        }

        private void ApplyEmotionalEffects()
        {
            // Hook into Audio and Visual systems here
            // e.g., AudioController.SetPitch(CurrentEmotion);
            // e.g., PostProcessingManager.SetDistortion(EmotionalIntensity);
            
            // Phase 2: Signal the UI Manager
            if (UI.UIControlManager.Instance != null)
            {
                // UI.UIControlManager.Instance.TriggerGlitchEffect(EmotionalIntensity);
            }
        }

        public string GetReactionDialogue(string trigger)
        {
            // Placeholder for adaptive dialogue lookup
            return "...";
        }
    }
}
