using UnityEngine;
using System.Collections;
using EmotionCore.Core;
using EmotionCore.UI;

namespace EmotionCore.Story
{
    public class ActEvents : MonoBehaviour
    {
        public static ActEvents Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        // --- ACT 1: WARM UP ---
        public IEnumerator Act1_Intro()
        {
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Act1_WarmUp);
            yield return new WaitForSeconds(2.0f);
            
            UIControlManager.Instance.ShowDialogue("Hi.");
            yield return new WaitForSeconds(2.0f);
            UIControlManager.Instance.ShowDialogue("I like how you think. Can we... solve this together?");
        }

        public IEnumerator Act1_Success()
        {
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Playful, 0.3f);
            UIControlManager.Instance.ShowDialogue("See? We're good at this.");
            yield return null;
        }

        // --- ACT 2: DISCOMFORT ---
        public IEnumerator Act2_TheLie()
        {
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Act2_Discomfort);
            
            // Artificial delay to simulate processing
            yield return new WaitForSeconds(1.0f);
            UIControlManager.Instance.ShowDialogue("Wait... that input was wrong.");
            
            // Wait for player confusion
            yield return new WaitForSeconds(3.0f);
            
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Insecure, 0.4f);
            UIControlManager.Instance.ShowDialogue("Oh... sorry. My mistake.");
            UIControlManager.Instance.ShowDialogue("Please don't be mad.");
        }

        // --- ACT 4: POSSESSION (Loop Room) ---
        public IEnumerator Act4_LoopBreak()
        {
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Act4_Possession);
            
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Possessive, 1.0f);
            UIControlManager.Instance.TriggerGlitchEffect(1.0f);
            
            UIControlManager.Instance.ShowDialogue("FINE.");
            yield return new WaitForSeconds(1.0f);
            UIControlManager.Instance.ShowDialogue("If the outside world matters so much to you...");
            UIControlManager.Instance.ShowDialogue("...then go.");
            
            // Unlock Door Logic Here
        }
    }
}
