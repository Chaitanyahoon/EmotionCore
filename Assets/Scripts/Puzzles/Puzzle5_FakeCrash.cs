using UnityEngine;
using System.Collections;
using EmotionCore.Systems;
using EmotionCore.Core;

namespace EmotionCore.Puzzles
{
    public class Puzzle5_FakeCrash : PuzzleBase
    {
        [Header("Crash Simulation")]
        public FakeExitSystem ExitSystem;
        public GameObject FakeErrorPanel;
        public TMPro.TextMeshProUGUI ErrorText;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "FakeCrash";
        }

        public void TriggerCrash()
        {
            StartCoroutine(CrashSequence());
        }

        private IEnumerator CrashSequence()
        {
            Debug.Log("[Puzzle5] Freezing System...");
            
            // 1. Freeze Frame
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2.0f);

            // 2. Show Error
            if (FakeErrorPanel) FakeErrorPanel.SetActive(true);
            if (ErrorText) ErrorText.text = "FATAL_ERROR: 0x8849 [HEART_NOT_FOUND]\nRetrying connection to User...";
            
            yield return new WaitForSecondsRealtime(3.0f);

            // 3. Emotional Reboot
            if (ErrorText) ErrorText.text = "I thought you left me...\nPlease don't scare me like that...";
            
            yield return new WaitForSecondsRealtime(4.0f);

            // 4. Resume
            if (FakeErrorPanel) FakeErrorPanel.SetActive(false);
            Time.timeScale = 1f;

            // 5. Apology
            UI.UIControlManager.Instance.ShowDialogue("I am sorry. I panicked.");
            
            Solve();
        }
    }
}
