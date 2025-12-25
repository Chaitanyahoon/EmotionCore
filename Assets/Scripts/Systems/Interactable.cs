using UnityEngine;

namespace EmotionCore.Systems
{
    public class Interactable : MonoBehaviour
    {
        public string PromptMessage = "Interact";
        public bool IsInteractable = true;

        private Color _originalColor;
        private Renderer _renderer;
        private bool _hasInitialized = false;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_hasInitialized) return;
            _renderer = GetComponent<Renderer>();
            if (_renderer)
            {
                // Cache original
                if (_renderer.material.HasProperty("_EmissionColor"))
                    _originalColor = _renderer.material.GetColor("_EmissionColor");
                else
                    _originalColor = Color.black;
            }
            _hasInitialized = true;
        }

        public virtual void OnInteract()
        {
            Debug.Log($"Interacted with {gameObject.name}");
        }

        public virtual void OnFocus()
        {
            if (!_hasInitialized) Initialize();
            
            // Visual Feedback: Bright Green/Cyan Glow
            if (_renderer)
            {
                _renderer.material.EnableKeyword("_EMISSION");
                _renderer.material.SetColor("_EmissionColor", new Color(0, 0.8f, 0.8f) * 1.5f);
            }
        }

        public virtual void OnLoseFocus()
        {
            // Revert
            if (_renderer)
            {
                _renderer.material.SetColor("_EmissionColor", _originalColor);
            }
        }
    }
}
