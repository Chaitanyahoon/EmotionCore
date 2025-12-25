using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace EmotionCore.UI
{
    public class UIControlManager : MonoBehaviour
    {
        public static UIControlManager Instance { get; private set; }

        public TextMeshProUGUI DialogueText;
        public TextMeshProUGUI InteractionPromptText;

        public CanvasGroup FakeOSLayer;
        public EmotionCore.Visuals.GlitchController GlitchFX;
        
        [Header("Settings")]
        public float TextTypingSpeed = 0.05f;

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

        public void ShowDialogue(string content)
        {
            StartCoroutine(TypewriterEffect(content));
        }

        private IEnumerator TypewriterEffect(string content)
        {
            DialogueText.text = "";
            foreach (char c in content)
            {
                DialogueText.text += c;
                
                // Varied typing speed for "organic" feel
                float randomDelay = Random.Range(TextTypingSpeed * 0.8f, TextTypingSpeed * 1.2f);
                yield return new WaitForSeconds(randomDelay);
            }
        }

        public void TriggerGlitchEffect(float intensity)
        {
            if (GlitchFX != null)
            {
                GlitchFX.TriggerBurst(intensity, 0.5f);
            }
            Debug.Log($"[UI] Glitch Intensity: {intensity}");
        }

        public void ShowFakeOSOverlay(bool show)
        {
            if (FakeOSLayer != null)
            {
                FakeOSLayer.alpha = show ? 1f : 0f;
                FakeOSLayer.interactable = show;
                FakeOSLayer.blocksRaycasts = show;
            }
        }

        public void ShowInteractionPrompt(string message)
        {
            if (InteractionPromptText != null)
            {
                InteractionPromptText.text = message;
                InteractionPromptText.gameObject.SetActive(true);
            }
        }

        public void HideInteractionPrompt()
        {
            if (InteractionPromptText != null)
            {
                InteractionPromptText.gameObject.SetActive(false);
            }
        }
    }
}
