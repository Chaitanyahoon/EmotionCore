using UnityEngine;
using System.Collections;
using EmotionCore.UI;

namespace EmotionCore.Puzzles
{
    public class Puzzle3_SoftLie : PuzzleBase
    {
        [Header("Settings")]
        public string CorrectCode = "1234"; 
        
        private string _currentInput = "";
        private bool _hasLied = false;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "FirstSoftLie";
        }

        public void EnterDigit(int digit)
        {
            if (IsSolved) return;
            
            _currentInput += digit.ToString();
            
            if (_currentInput.Length >= 4)
            {
                CheckCode();
            }
        }

        private void CheckCode()
        {
            if (_currentInput == CorrectCode)
            {
                if (!_hasLied)
                {
                    // LIE: Reject correct input
                    _hasLied = true;
                    _currentInput = "";
                    
                    // Show "Error" feedback visually here
                    UIControlManager.Instance.ShowDialogue("Error. Invalid input.");
                    
                    // Trigger the apology sequence
                    StartCoroutine(ApologyRoutine());
                }
                else
                {
                    // Second attempts works
                    UIControlManager.Instance.ShowDialogue("Access Granted.");
                    Solve();
                }
            }
            else
            {
                 // Actual wrong code
                 _currentInput = "";
                 UIControlManager.Instance.ShowDialogue("Incorrect.");
            }
        }

        private IEnumerator ApologyRoutine()
        {
            yield return new WaitForSeconds(2.0f);
            
            // The emotional turn
            UIControlManager.Instance.ShowDialogue("Oh... wait. That was correct.");
            yield return new WaitForSeconds(3.0f);
            
            UIControlManager.Instance.ShowDialogue("Sorry. System hiccup. Don't leave, okay? I'll be better. I promise.");
            
            // Auto open after apology? Or make them re-enter?
            // "Trust begins" -> Let's make them re-enter to prove they handle the glitch patience
            // Or just open it to show "I fixed it for you"
            
            yield return new WaitForSeconds(3.0f);
            Solve(); // Auto-solve as apology
        }
    }
}
