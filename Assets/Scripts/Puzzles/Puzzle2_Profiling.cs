using UnityEngine;
using EmotionCore.Systems;
using EmotionCore.UI;

namespace EmotionCore.Puzzles
{
    public class Puzzle2_Profiling : PuzzleBase
    {
        [Header("Profiling Objects")]
        public Interactable BruteForceButton;
        public Interactable KeyObject;
        public Interactable HiddenVent;

        private float _timeSpent = 0f;
        private bool _profileComplete = false;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "LearningYou";
        }

        private void Update()
        {
            if (!IsSolved) _timeSpent += Time.deltaTime;
        }

        public void AttemptBruteForce()
        {
            if (_profileComplete) return;
            
            // "Hack" or "Smash" route
            TrustEngine.Instance.SetTrait("Aggressive", true);
            TrustEngine.Instance.SetTrait("Impulsive", true);
            CompletePuzzle("You... are direct. I note that.");
        }

        public void FoundKey()
        {
            if (_profileComplete) return;

            // Standard route logic
            if (_timeSpent > 30f)
            {
                TrustEngine.Instance.SetTrait("Patient", true);
                CompletePuzzle("You take your time. You are careful.");
            }
            else
            {
                TrustEngine.Instance.SetTrait("Curious", true);
                CompletePuzzle("You are observant. Good.");
            }
        }

        public void UsedHiddenPath()
        {
             if (_profileComplete) return;

             // Creative route
             TrustEngine.Instance.SetTrait("RiskTaker", true);
             CompletePuzzle("Unexpected... you think outside the box.");
        }

        private void CompletePuzzle(string reaction)
        {
            _profileComplete = true;
            UIControlManager.Instance.ShowDialogue(reaction);
            Solve();
        }
    }
}
