using UnityEngine;
using System.Collections;
using EmotionCore.Core;

namespace EmotionCore.Story
{
    public class NarrativeDirector : MonoBehaviour
    {
        public static NarrativeDirector Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            // Start the game loop automatically for now
            StartCoroutine(Flowcheck());
        }

        private IEnumerator Flowcheck()
        {
            yield return new WaitForSeconds(1.0f);
            
            // Check Save Data to see where we left off
            // For now, assume fresh start
            StartCoroutine(ActEvents.Instance.Act1_Intro());
        }

        public void TriggerEvent(string eventID)
        {
            Debug.Log($"[Director] Event Triggered: {eventID}");
            
            switch(eventID)
            {
                case "Puzzle1_Solved":
                    StartCoroutine(ActEvents.Instance.Act1_Success());
                    break;
                case "FirstLie_Trigger":
                    StartCoroutine(ActEvents.Instance.Act2_TheLie());
                    break;
                case "Player_Broken_Loop":
                    StartCoroutine(ActEvents.Instance.Act4_LoopBreak());
                    break;
            }
        }
    }
}
