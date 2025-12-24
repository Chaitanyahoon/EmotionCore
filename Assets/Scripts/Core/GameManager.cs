using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EmotionCore.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameAct
        {
            Act1_WarmUp,
            Act2_Discomfort,
            Act3_Attachment,
            Act4_Possession,
            Final_Breakup,
            Trapped
        }

        public GameAct CurrentAct { get; private set; } = GameAct.Act1_WarmUp;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSystems();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSystems()
        {
            Debug.Log("[GameManager] Initializing EmotionCore...");
            
            // Initialize Sub-Systems
            // MemoryCore.Instance.Init();
            // EmotionEngine.Instance.Init();
            // TrustEngine.Instance.Init();
        }

        public void AdvanceAct(GameAct nextAct)
        {
            CurrentAct = nextAct;
            Debug.Log($"[GameManager] Advancing to {CurrentAct}");
            // Trigger global events based on act change
            EmotionEngine.Instance.OnActChange(CurrentAct);
        }

        public void FakeCrashGame()
        {
            StartCoroutine(FakeCrashSequence());
        }

        private IEnumerator FakeCrashSequence()
        {
            // TODO: Implement realistic freeze and crash log generation
            Debug.LogWarning("[GameManager] INITIATING FAKE CRASH...");
            yield return new WaitForSeconds(1.0f);
            
            // For now, just quit (or fake UI overlay)
            // Application.Quit(); 
            // In editor: UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
