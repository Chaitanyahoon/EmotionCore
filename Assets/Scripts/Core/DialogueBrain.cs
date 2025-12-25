using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EmotionCore.Core
{
    public class DialogueBrain : MonoBehaviour
    {
        public static DialogueBrain Instance { get; private set; }

        [System.Serializable]
        public struct DialogueLine
        {
            public string ID;
            public string Text;
            public EmotionEngine.EmotionState RequiredEmotion;
            public float MaxTrust; // Only show if trust is BELOW this
            public float MinTrust; // Only show if trust is ABOVE this
            public string MemoryCondition; // Optional: Require a memory flag key to be TRUE
            public bool NegateMemory; // If true, MemoryCondition must be FALSE
        }

        // In a real scenario, this would load from a JSON/XML
        // For now, hardcoded for prototype speed
        private List<DialogueLine> _dialogueDatabase = new List<DialogueLine>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            // EXAMPLES
            _dialogueDatabase.Add(new DialogueLine { 
                ID = "PuzzleSuccess", Text = "Good job. We make a great team.", 
                RequiredEmotion = EmotionEngine.EmotionState.Calm, MinTrust = 50 
            });
            _dialogueDatabase.Add(new DialogueLine { 
                ID = "PuzzleSuccess", Text = "You're moving too fast. Slow down.", 
                RequiredEmotion = EmotionEngine.EmotionState.Insecure, MaxTrust = 40 
            });
            _dialogueDatabase.Add(new DialogueLine { 
                ID = "PuzzleFail", Text = "It's okay. I'll wait.", 
                RequiredEmotion = EmotionEngine.EmotionState.Calm, MinTrust = 50 
            });
            _dialogueDatabase.Add(new DialogueLine { 
                ID = "PuzzleFail", Text = "Why can't you do this? It's simple.", 
                RequiredEmotion = EmotionEngine.EmotionState.Possessive, MaxTrust = 30 
            });

            // Phase 2 Content
            _dialogueDatabase.Add(new DialogueLine {
                 ID = "Greeting", Text = "Hello... {USER}. I see you.",
                 RequiredEmotion = EmotionEngine.EmotionState.Calm
            });
            _dialogueDatabase.Add(new DialogueLine {
                ID = "Idle", Text = "You are quiet. thinking about the {TIME}?",
                RequiredEmotion = EmotionEngine.EmotionState.Calm
            });
        }

        public string GetReaction(string triggerID)
        {
            var currentEmotion = EmotionEngine.Instance.CurrentEmotion;
            var currentTrust = Systems.TrustEngine.Instance.PlayerProfile.TrustScore;

            // Filter candidates
            var candidates = _dialogueDatabase.Where(line => 
                line.ID == triggerID && 
                (line.RequiredEmotion == currentEmotion || line.RequiredEmotion == EmotionEngine.EmotionState.Broken) && // Broken can inherit OR fail
                currentTrust >= line.MinTrust && 
                currentTrust <= (line.MaxTrust == 0 ? 100 : line.MaxTrust) &&
                CheckMemoryCondition(line)
            ).ToList();

            if (candidates.Count > 0)
            {
                string rawText = candidates[Random.Range(0, candidates.Count)].Text;
                return FormatText(rawText);
            }

            // Fallback
            return "...";
        }

        private bool CheckMemoryCondition(DialogueLine line)
        {
            if (string.IsNullOrEmpty(line.MemoryCondition)) return true;

            bool flagValue = Systems.MemoryCore.Instance.GetFlag(line.MemoryCondition);
            return line.NegateMemory ? !flagValue : flagValue;
        }

        private string FormatText(string template)
        {
            string output = template;
            output = output.Replace("{USER}", System.Environment.UserName);
            output = output.Replace("{TIME}", System.DateTime.Now.ToString("HH:mm"));
            
            // Add other placeholders like {DEATH_COUNT} here
            
            return output;
        }
    }
}
