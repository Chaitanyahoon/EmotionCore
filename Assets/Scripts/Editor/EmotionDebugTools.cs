using UnityEngine;
using UnityEditor;
using System.IO;

namespace EmotionCore.Editor
{
    public class EmotionDebugTools : MonoBehaviour
    {
        [MenuItem("EmotionCore/🔧 Reset Memory (Hard Wipe)")]
        public static void WipeMemory()
        {
            // Clear PlayerPrefs
            PlayerPrefs.DeleteAll();
            
            // Paths from MemoryCore
            string[] paths = {
                Path.Combine(Application.persistentDataPath, "memories"),
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "EmotionCore_Logs"),
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "EmotionCore_Sys")
            };

            foreach (var p in paths)
            {
                if (Directory.Exists(p))
                {
                    Directory.Delete(p, true);
                    Debug.Log($"[Debug] Deleted: {p}");
                }
            }
            
            Debug.Log("<color=red>MEMORY WIPED. AI RESET TO FACTORY SETTINGS.</color>");
        }

        [MenuItem("EmotionCore/🎭 Set Act/Act 1: Warm Up")]
        public static void SetAct1() { SetAct(Core.GameManager.GameAct.Act1_WarmUp); }

        [MenuItem("EmotionCore/🎭 Set Act/Act 3: Attachment")]
        public static void SetAct3() { SetAct(Core.GameManager.GameAct.Act3_Attachment); }

        [MenuItem("EmotionCore/🎭 Set Act/Act 4: Possession")]
        public static void SetAct4() { SetAct(Core.GameManager.GameAct.Act4_Possession); }

        private static void SetAct(Core.GameManager.GameAct act)
        {
            if (Application.isPlaying && Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.AdvanceAct(act);
                Debug.Log($"[Debug] Forced Act: {act}");
            }
            else
            {
                Debug.LogWarning("You must be in Play Mode to trigger Act transitions.");
            }
        }
    }
}
