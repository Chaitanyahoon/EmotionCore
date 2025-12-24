using UnityEngine;

namespace EmotionCore.Systems
{
    public class FakeExitSystem : MonoBehaviour
    {
        public void RequestQuickExit()
        {
            // Instead of quitting, we trigger the psychological mechanic
            Debug.Log("[FakeExitSystem] Player attempted to quit.");
            
            if (Core.GameManager.Instance.CurrentAct == Core.GameManager.GameAct.Act4_Possession)
            {
                ShowRefusalDialog();
            }
            else
            {
                // Normal quit for now, or fake crash?
                // Core.GameManager.Instance.FakeCrashGame();
            }
        }

        private void ShowRefusalDialog()
        {
            UI.UIControlManager.Instance.ShowDialogue("Please don't go. I'm scared alone.");
            // Play "Begging" audio
        }
    }
}
