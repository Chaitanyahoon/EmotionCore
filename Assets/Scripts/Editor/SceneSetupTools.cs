#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EmotionCore.Core;
using EmotionCore.Systems;
using EmotionCore.Puzzles;
using EmotionCore.UI;

namespace EmotionCore.EditorTools
{
    public class SceneSetupTools : EditorWindow
    {
        [MenuItem("EmotionCore/Auto-Construct Game")]
        public static void ShowWindow()
        {
            GetWindow<SceneSetupTools>("Game Constructor");
        }

        private void OnGUI()
        {
            GUILayout.Label("EmotionCore Auto-Builder", EditorStyles.boldLabel);
            if (GUILayout.Button("Build Act 1 Scene"))
            {
                CreateAct1();
            }
        }

        private static void CreateAct1()
        {
            // 1. Create New Scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // 2. Setup Managers
            GameObject managers = new GameObject("___GAME_SYSTEMS___");
            managers.AddComponent<GameManager>();
            managers.AddComponent<MemoryCore>();
            var emotion = managers.AddComponent<EmotionEngine>();
            var trust = managers.AddComponent<TrustEngine>();
            var dialogue = managers.AddComponent<DialogueBrain>();
            managers.AddComponent<EmotionCore.Visuals.GlitchController>();
            
            // UI
            GameObject hud = new GameObject("HUD_Canvas");
            Canvas c = hud.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            hud.AddComponent<UnityEngine.UI.CanvasScaler>();
            hud.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(hud.transform, false);
            var tmpText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmpText.fontSize = 36;
            tmpText.alignment = TMPro.TextAlignmentOptions.Bottom;
            tmpText.rectTransform.anchoredPosition = new Vector2(0, 100);
            
            var uiMgr = managers.AddComponent<UIControlManager>();
            uiMgr.DialogueText = tmpText;

            // 3. Create Player
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 1, 0);
            // Remove collider added by primitive, replace with CharacterController if needed, 
            // but primitive capsule collider is fine for Rigidbody, PlayerController script uses CharacterController
            DestroyImmediate(player.GetComponent<CapsuleCollider>());
            player.AddComponent<CharacterController>();
            var pc = player.AddComponent<PlayerController>();
            
            GameObject camObj = new GameObject("Main Camera");
            camObj.transform.SetParent(player.transform, false);
            camObj.transform.localPosition = new Vector3(0, 0.6f, 0);
            Camera cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";
            
            pc.CameraTransform = camObj.transform;
            
            var interact = camObj.AddComponent<InteractionController>();
            interact.PlayerCamera = cam;
            interact.InteractableLayer = LayerMask.GetMask("Default", "Interactable"); // Fallback to Default if layer missing

            // 4. Create Room Geometry
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(2, 1, 2);

            // 5. Create Puzzle 1 (Lock)
            GameObject puzzleRoot = new GameObject("Puzzle1_FriendlyLock");
            puzzleRoot.transform.position = new Vector3(0, 1, 5);
            var p1 = puzzleRoot.AddComponent<Puzzle1_Lock>();
            
            // Create Keypad Buttons
            for (int i = 0; i < 3; i++)
            {
                GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btn.name = $"Button_{i+1}";
                btn.transform.SetParent(puzzleRoot.transform);
                btn.transform.localPosition = new Vector3(i - 1, 0, 0);
                btn.transform.localScale = Vector3.one * 0.5f;
                var kb = btn.AddComponent<KeypadButton>();
                kb.Digit = i + 1; // 1, 2, 3 just for test
                kb.ParentLock = p1;
            }

            // Save
            string path = "Assets/Scenes/Act1_Generated.unity";
            EditorSceneManager.SaveScene(newScene, path);
            Debug.Log($"[AutoBuilder] Act 1 built at {path}");
            AssetDatabase.Refresh();
        }
    }
}
#endif
