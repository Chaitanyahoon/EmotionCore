using UnityEngine;
using UnityEngine.UI;

namespace EmotionCore.UI
{
    public class UIGlitch : MonoBehaviour
    {
        public bool IsViolent = false;
        private RectTransform _rect;
        private Vector2 _originalPos;
        private Text _text;
        private Color _originalColor;

        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _originalPos = _rect.anchoredPosition;
            _text = GetComponent<Text>();
            if (_text) _originalColor = _text.color;
        }

        void Update()
        {
            // Position Jitter
            float range = IsViolent ? 5.0f : 2.0f;
            _rect.anchoredPosition = _originalPos + new Vector2(Random.Range(-range, range), Random.Range(-range, range));

            // Color Flicker (Occasional)
            if (_text && Random.value > 0.95f)
            {
                _text.color = IsViolent ? Color.red : Color.white;
            }
            else if (_text)
            {
                _text.color = _originalColor;
            }
        }
    }
}
