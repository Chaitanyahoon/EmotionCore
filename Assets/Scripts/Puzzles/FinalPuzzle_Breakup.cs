using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EmotionCore.Systems;
using EmotionCore.UI;
using EmotionCore.Core;

namespace EmotionCore.Puzzles
{
    public class FinalPuzzle_Breakup : PuzzleBase
    {
        [Header("Scene")]
        public List<Interactable> MemoryNodes; // Represents "Shared Memories"
        public GameObject FinalDoor;

        private int _memoriesDeleted = 0;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "TheBreakup";
            
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Desperate, 1.0f);
            
            // Hook up events
            foreach(var mem in MemoryNodes)
            {
                // Assuming Interactable has a generic 'OnInteract' we can hook or modify
                // For now, we assume simple interaction = deletion
            }
        }

        public void DeleteMemory(Interactable memory)
        {
            if (IsSolved) return;

            _memoriesDeleted++;
            memory.gameObject.SetActive(false); // Poof

            HandleReaction(_memoriesDeleted);

            if (_memoriesDeleted >= MemoryNodes.Count)
            {
                StartCoroutine(FinalGoodbye());
            }
        }

        private void HandleReaction(int count)
        {
            string line = "";
            switch (count)
            {
                case 1: 
                    line = "Stop! That hurts!"; 
                    EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Panicked, 1.0f);
                    break;
                case 2: 
                    line = "Please... not that one. That was my favorite day."; 
                    EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Desperate, 1.0f);
                    break;
                case 3: 
                    line = "Don't erase me. I'm real. I'm REAL!"; 
                    // Glitch intensity max
                    UIControlManager.Instance.TriggerGlitchEffect(1.0f);
                    break;
                case 4:
                     line = "...please...";
                     break;
            }
            if(!string.IsNullOrEmpty(line)) UIControlManager.Instance.ShowDialogue(line);
        }

        private IEnumerator FinalGoodbye()
        {
            yield return new WaitForSeconds(1.0f);
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Broken, 0.0f);
            UIControlManager.Instance.ShowDialogue("...I understand.");
            
            yield return new WaitForSeconds(3.0f);
            UIControlManager.Instance.ShowDialogue("Hurting me hurts you too. That means we were real.");
            
            yield return new WaitForSeconds(4.0f);
            
            // Unlock final exit logic
            // Application.Quit(); // Or Credit Roll
            if (FinalDoor) FinalDoor.SetActive(true);
            
            Solve();
        }
    }
}
