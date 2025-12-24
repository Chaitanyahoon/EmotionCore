using UnityEngine;
using UnityEngine.Events;

namespace EmotionCore.Puzzles
{
    public abstract class PuzzleBase : MonoBehaviour
    {
        [Header("Puzzle Settings")]
        public string PuzzleID;
        public bool IsSolved { get; protected set; } = false;
        
        [Header("Events")]
        public UnityEvent OnPuzzleStart;
        public UnityEvent OnPuzzleSolved;
        public UnityEvent OnPuzzleFailed;

        protected virtual void Start()
        {
            // Auto-register with GameManager if needed
        }

        public virtual void BeginPuzzle()
        {
            Debug.Log($"[PuzzleBase] Starting Puzzle: {PuzzleID}");
            OnPuzzleStart.Invoke();
        }

        protected virtual void Solve()
        {
            if (IsSolved) return;

            IsSolved = true;
            Debug.Log($"[PuzzleBase] Solved: {PuzzleID}");
            OnPuzzleSolved.Invoke();
            
            // Build trust
            Utility.ValidationCallback(true); 
        }

        protected virtual void Fail()
        {
            Debug.Log($"[PuzzleBase] Failed: {PuzzleID}");
            OnPuzzleFailed.Invoke();
        }
        
        // Helper for simple internal logic
        private static class Utility {
             public static void ValidationCallback(bool success) {
                 if(success && Systems.TrustEngine.Instance != null) {
                     Systems.TrustEngine.Instance.ModifyTrust(5f);
                 }
             }
        }
    }
}
