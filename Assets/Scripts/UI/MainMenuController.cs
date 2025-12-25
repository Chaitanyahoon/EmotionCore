using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace EmotionCore.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public List<UnityEngine.UI.Text> GlitchTexts = new List<UnityEngine.UI.Text>();
        public List<RectTransform> GlitchTransforms = new List<RectTransform>();
        private List<Vector2> _originalPositions = new List<Vector2>();

        void Start()
        {
            // CRITICAL: Ensure Mouse is visible and unlocked
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 1. Runtime Binding of Buttons (Fixes Serialization Issue)
            BindButton("StartButton", OnStartGame);
            BindButton("ExitButton", OnExitGame);

            // 2. Find Glitch Texts (Safer than Tags)
            var allTexts = GetComponentsInChildren<UnityEngine.UI.Text>(true);
            foreach (var t in allTexts)
            {
                // Identification by name pattern or parent name
                if (t.gameObject.name == "GlitchText" || t.transform.name == "GlitchText")
                {
                    GlitchTexts.Add(t);
                    var r = t.rectTransform;
                    GlitchTransforms.Add(r);
                    _originalPositions.Add(r.anchoredPosition);
                }
            }
        }

        private void BindButton(string btnName, UnityEngine.Events.UnityAction action)
        {
            var btnObj = GameObject.Find(btnName);
            if (btnObj)
            {
                var btn = btnObj.GetComponent<UnityEngine.UI.Button>();
                if (btn)
                {
                    btn.onClick.RemoveAllListeners(); // Clean slate
                    btn.onClick.AddListener(action);

                    // Add Hover Events Manually
                    var trigger = btnObj.GetComponent<UnityEngine.EventSystems.EventTrigger>();
                    if (!trigger) trigger = btnObj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                    
                    var entryEnter = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
                    entryEnter.callback.AddListener((data) => OnHoverEnter(btn.GetComponent<RectTransform>()));
                    trigger.triggers.Add(entryEnter);

                    var entryExit = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
                    entryExit.callback.AddListener((data) => OnHoverExit(btn.GetComponent<RectTransform>()));
                    trigger.triggers.Add(entryExit);
                }
            }
        }

        void Update()
        {
            // Centralized Glitch Logic
            for (int i = 0; i < GlitchTransforms.Count; i++)
            {
                // Jitter Position
                Vector2 jitter = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                GlitchTransforms[i].anchoredPosition = _originalPositions[i] + jitter;

                // Color Flicker
                if (Random.value > 0.9f)
                {
                    GlitchTexts[i].color = Random.value > 0.5f ? Color.red : Color.black;
                }
                else
                {
                    GlitchTexts[i].color = new Color(0.5f, 0, 0, 0.5f); // Base Dark Red
                }
            }
        }

        public void OnHoverEnter(RectTransform btnRect)
        {
            btnRect.localScale = Vector3.one * 1.2f;
        }

        public void OnHoverExit(RectTransform btnRect)
        {
            btnRect.localScale = Vector3.one;
        }

        public void OnStartGame()
        {
            Debug.Log("Starting Game...");
            SceneManager.LoadScene(1);
        }

        public void OnExitGame()
        {
            Debug.Log("Exiting Game...");
            Application.Quit();
        }
    }
}
