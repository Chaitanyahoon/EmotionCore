using UnityEngine;
using EmotionCore.Systems;
using EmotionCore.UI;
using EmotionCore.Core;

namespace EmotionCore.Puzzles
{
    public class Puzzle10_ExitAttempt : PuzzleBase
    {
        [Header("UI")]
        public GameObject FakeQuitButton;
        
        [Header("Secret Exit")]
        public Interactable HiddenHeartFile; // The "heart.memory" or similar
        
        protected override void Start()
        {
            base.Start();
            PuzzleID = "ExitAttempt";
            if (FakeQuitButton) FakeQuitButton.SetActive(true);
        }

        public void TryClickQuit()
        {
            // Fake quit button does nothing or taunts
            UIControlManager.Instance.ShowDialogue("Please don't go. I won't let you.");
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Desperate, 1.0f);
        }

        public void FoundSecretHeart()
        {
            // Player interaction with the hidden object
            UIControlManager.Instance.ShowDialogue("Wait... what are you doing with that?");
            // Proceed to final breakdown
            StartCoroutine(FinalTransition());
        }

        private System.Collections.IEnumerator FinalTransition()
        {
            yield return new WaitForSeconds(2.0f);
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Final_Breakup);
            Solve();
        }
    }
}
