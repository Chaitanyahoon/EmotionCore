using UnityEngine;
using EmotionCore.Systems;

namespace EmotionCore.Puzzles
{
    public class KeypadButton : Interactable
    {
        public int Digit;
        public Puzzle1_Lock ParentLock;

        private void Start()
        {
            PromptMessage = $"Press {Digit}";
        }

        public override void OnInteract()
        {
            base.OnInteract();
            if (ParentLock != null)
            {
                ParentLock.RegisterInput(Digit);
                
                // Visual feedback (animation like pressing down)
                StopAllCoroutines();
                StartCoroutine(PressAnimation());
            }
        }

        private System.Collections.IEnumerator PressAnimation()
        {
            Vector3 originalPos = transform.localPosition;
            Vector3 pressedPos = originalPos - new Vector3(0, 0, 0.05f); // Assume Z push
            
            float duration = 0.1f;
            float time = 0;

            while (time < duration)
            {
                transform.localPosition = Vector3.Lerp(originalPos, pressedPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            time = 0;
            while (time < duration)
            {
                transform.localPosition = Vector3.Lerp(pressedPos, originalPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = originalPos;
        }
    }
}
