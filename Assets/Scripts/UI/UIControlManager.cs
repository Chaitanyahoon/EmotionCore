using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace EmotionCore.UI
{
    public class UIControlManager : MonoBehaviour
    {
        public static UIControlManager Instance { get; private set; }

        [Header("HUD Elements")]
        public TextMeshProUGUI DialogueText;
        public CanvasGroup FakeOSLayer;
        
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
            // TODO: Implement UI glitch shader parameters here
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
    }
}
