using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace EmotionCore.UI
{
    public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform _rect;
        private Vector3 _originalScale;

        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _originalScale = _rect.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _rect.localScale = _originalScale * 1.1f; // Grow
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _rect.localScale = _originalScale; // Reset
        }
    }
}
