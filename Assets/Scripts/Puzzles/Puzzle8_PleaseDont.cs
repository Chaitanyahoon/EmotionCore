using UnityEngine;
using System.Collections;
using EmotionCore.Systems;
using EmotionCore.Core;

namespace EmotionCore.Puzzles
{
    public class Puzzle8_PleaseDont : PuzzleBase
    {
        [Header("Door Object")]
        public Interactable ForbiddenDoor;
        public GameObject SafeRoomVisuals;
        public GameObject BrokenRoomVisuals;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "ObedienceTest";
            
            // Warning
            StartCoroutine(WarningRoutine());
        }

        private IEnumerator WarningRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            UI.UIControlManager.Instance.ShowDialogue("Please... do NOT open that door. Just stay here with me.");
        }

        public void AttemptOpenDoor()
        {
            // Player Disobeyed
            HandleDisobedience();
        }

        public void ExampleWaitLogic()
        {
            // If player waits X seconds, Good Ending path for this puzzle
             StartCoroutine(WaitRoutine());
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(20.0f); // 20s of obedience
            if (!IsSolved)
            {
                HandleObedience();
            }
        }

        private void HandleDisobedience()
        {
            if (IsSolved) return;

            // Panic Mode
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Panicked, 1.0f); // Or Broken/Desperate
            TrustEngine.Instance.RegisterDisobedience();
            TrustEngine.Instance.SetTrait("Impulsive", true);
            
            if (BrokenRoomVisuals) BrokenRoomVisuals.SetActive(true);
            if (SafeRoomVisuals) SafeRoomVisuals.SetActive(false);

            UI.UIControlManager.Instance.ShowDialogue("Why?! I told you not to! You ruin everything!");
            
            // Glitch effect
            UI.UIControlManager.Instance.TriggerGlitchEffect(1.0f);
            
            Solve(); 
        }

        private void HandleObedience()
        {
             if (IsSolved) return;
             
             // Cuddle Mode
             EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Affectionate, 1.0f);
             TrustEngine.Instance.SetTrait("Patient", true);
             TrustEngine.Instance.ModifyTrust(20f);
             
             UI.UIControlManager.Instance.ShowDialogue("Thank you... for listening. You're so good to me.");
             
             Solve();
        }
    }
}
