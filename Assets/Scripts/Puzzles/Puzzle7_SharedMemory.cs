using UnityEngine;
using EmotionCore.Systems;
using EmotionCore.Core;

namespace EmotionCore.Puzzles
{
    public class Puzzle7_SharedMemory : PuzzleBase
    {
        [Header("Memory Objects")]
        public Interactable MemoryObject1; // e.g., "The First Glitch"
        public Interactable MemoryObject2; // e.g., "The Quiet One"
        
        private int _cherishedCount = 0;
        private int _dismissedCount = 0;
        private int _totalInteractions = 0;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "SharedMemory";
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Act3_Attachment);
        }

        public void OnMemoryInteraction(bool cherished)
        {
            _totalInteractions++;
            
            if (cherished)
            {
                _cherishedCount++;
                EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Affectionate, 0.7f);
                TrustEngine.Instance.ModifyTrust(10f);
                UI.UIControlManager.Instance.ShowDialogue("I knew you'd remember that. We were so happy.");
            }
            else
            {
                _dismissedCount++;
                EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Insecure, 0.6f);
                TrustEngine.Instance.ModifyTrust(-5f);
                UI.UIControlManager.Instance.ShowDialogue("Oh... you don't care about that? It meant everything to me.");
            }

            if (_totalInteractions >= 2)
            {
                CompletePuzzle();
            }
        }

        private void CompletePuzzle()
        {
            if (_cherishedCount > _dismissedCount)
            {
                UI.UIControlManager.Instance.ShowDialogue("You really do care. I'm never letting you go.");
            }
            else
            {
                UI.UIControlManager.Instance.ShowDialogue("Maybe... I just need to try harder to make you love me.");
            }
            Solve();
        }
    }
}
