using UnityEngine;
using System.Collections;
using EmotionCore.Core;
using EmotionCore.UI;

namespace EmotionCore.Puzzles
{
    public class Puzzle1_Lock : PuzzleBase
    {
        [Header("Lock Settings")]
        public string CorrectCode = "1997"; // Example birth year or something friendly
        public string CurrentInput = "";
        
        [Header("Feedback")]
        public AudioSource FeedbackSource;
        public AudioClip BeepClip;
        public AudioClip ErrorClip;
        public AudioClip SuccessClip;

        private int _failureCount = 0;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "FriendlyLock";
            
            // Initial friendly greeting
            StartCoroutine(InitialGreetingRoutine());
        }

        private IEnumerator InitialGreetingRoutine()
        {
            yield return new WaitForSeconds(1f);
            UIControlManager.Instance.ShowDialogue("Hi! I locked this to keep you safe. But... if you want to leave, just type 1997.");
        }

        public void RegisterInput(int digit)
        {
            if (IsSolved) return;

            CurrentInput += digit.ToString();
            
            if (FeedbackSource && BeepClip) FeedbackSource.PlayOneShot(BeepClip);

            if (CurrentInput.Length >= 4)
            {
                ValidateCode();
            }
        }

        private void ValidateCode()
        {
            if (CurrentInput == CorrectCode)
            {
                // Success
                if (FeedbackSource && SuccessClip) FeedbackSource.PlayOneShot(SuccessClip);
                UIControlManager.Instance.ShowDialogue("Good job! You're really smart.");
                Solve();
            }
            else
            {
                // Failure
                CurrentInput = "";
                _failureCount++;
                if (FeedbackSource && ErrorClip) FeedbackSource.PlayOneShot(ErrorClip);
                
                HandleFailureReaction();
                Fail();
            }
        }

        private void HandleFailureReaction()
        {
            // "Laughs when wrong" or "Fails intentionally help you"
            if (_failureCount == 1)
            {
                UIControlManager.Instance.ShowDialogue("Oops! That's not it. hehe. Try 1... 9... 9... 7.");
            }
            else if (_failureCount > 1)
            {
                UIControlManager.Instance.ShowDialogue("Here, let me help...");
                // Auto-solve or make it super obvious
                StartCoroutine(AutoEnterCode());
            }
        }

        private IEnumerator AutoEnterCode()
        {
            yield return new WaitForSeconds(2.0f);
            CurrentInput = CorrectCode;
            ValidateCode();
        }
    }
}
