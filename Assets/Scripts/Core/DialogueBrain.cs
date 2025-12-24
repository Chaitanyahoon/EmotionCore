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
        }

        public string GetReaction(string triggerID)
        {
            var currentEmotion = EmotionEngine.Instance.CurrentEmotion;
            var currentTrust = Systems.TrustEngine.Instance.PlayerProfile.TrustScore;

            // Find best match
            var candidates = _dialogueDatabase.Where(line => 
                line.ID == triggerID && 
                line.RequiredEmotion == currentEmotion &&
                currentTrust >= line.MinTrust && 
                currentTrust <= (line.MaxTrust == 0 ? 100 : line.MaxTrust)
            ).ToList();

            if (candidates.Count > 0)
            {
                return candidates[Random.Range(0, candidates.Count)].Text;
            }

            // Fallback
            return "...";
        }
    }
}
