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
    public class WorldGenerator : EditorWindow
    {
        [MenuItem("EmotionCore/GENERATE FULL GAME WORLD")]
        public static void ShowWindow()
        {
            GetWindow<WorldGenerator>("World Gen");
        }

        private void OnGUI()
        {
            GUILayout.Label("Procedural World Creator", EditorStyles.boldLabel);
            if (GUILayout.Button("EXECUTE: Construct World"))
            {
                ConstructWorld();
            }
        }

        private static void ConstructWorld()
        {
            // 0. Generate Assets
            var wallMat = AssetGenerator.CreateWallMaterial();
            var floorMat = AssetGenerator.CreateFloorMaterial();
            var concreteMat = AssetGenerator.CreateConcreteMaterial();
            var glitchMat = AssetGenerator.CreateGlitchMaterial();

            // 1. Setup Scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // 2. Systems
            SetupSystems(glitchMat);

            // 3. Player
            var player = SetupPlayer(new Vector3(0, 1, 0));

            // 4. Geometry & Puzzles
            // Act 1: The Cell
            BuildRoom(new Vector3(0, 0, 0), new Vector3(10, 5, 10), "Room_Act1");
            SetupPuzzle1(new Vector3(0, 1.5f, 4.5f));

            // Hallway (Puzzle 9 Area)
            BuildHallway(new Vector3(5, 0, 0), 20f, "Hallway_Loop");
            
            // Act 2: The Office (L-Shaped Layout)
            // Main Room (North Open)
            BuildRoom(new Vector3(30, 0, 0), new Vector3(8, 4, 8), "Room_Act2_Main", new bool[] { true, false, false, false }); 
            // Annex (South Open to connect)
            BuildRoom(new Vector3(30, 0, 8), new Vector3(8, 4, 8), "Room_Act2_Annex", new bool[] { false, true, false, false }); 
            
            SetupPuzzle4(new Vector3(30, 1.5f, 3f));
            SetupPuzzle5_FakeCrash(); // UI based

            // Act 3: The Bedroom (Attachment)
            BuildRoom(new Vector3(30, 0, 15), new Vector3(10, 4, 10), "Room_Act3");
            SetupPuzzle7(new Vector3(28, 1, 15));
            SetupPuzzle8_Door(new Vector3(30, 1, 19.5f));

            // Finale Chamber
            BuildRoom(new Vector3(30, -10, 0), new Vector3(20, 10, 20), "Chamber_Finale");
            SetupFinalPuzzle(new Vector3(30, -5, 0));

            string scenePath = "Assets/Scenes/EmotionCore_FullWorld.unity";
            System.IO.Directory.CreateDirectory("Assets/Scenes");
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log("Full World Generated successfully!");
            
            // 5. Generate Main Menu
            ConstructMainMenu();
        }

        private static void ConstructMainMenu()
        {
            Scene menuScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Controller
            GameObject ctrl = new GameObject("MenuController");
            var menuScript = ctrl.AddComponent<MainMenuController>();

            // Setup Camera
            GameObject camObj = new GameObject("MainCamera");
            Camera cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f); // Darker Deep Blue
            camObj.AddComponent<AudioListener>();

            // Setup EventSystem
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // --- VISUAL UPGRADE: Background Grid ---
            GameObject bgObj = new GameObject("BackgroundGrid");
            bgObj.transform.SetParent(canvasObj.transform, false);
            UnityEngine.UI.RawImage bgImg = bgObj.AddComponent<UnityEngine.UI.RawImage>();
            bgImg.raycastTarget = false; // CRITICAL: Don't block clicks
            bgImg.color = new Color(0.3f, 0.0f, 0.0f, 0.2f); // Blood Red Faint Grid
            bgImg.rectTransform.anchorMin = Vector2.zero;
            bgImg.rectTransform.anchorMax = Vector2.one;
            bgImg.rectTransform.sizeDelta = Vector2.zero;
            bgImg.texture = AssetGenerator.CreateWallMaterial().mainTexture; 
            bgImg.uvRect = new Rect(0, 0, 8, 4.5f); 

            // --- VISUAL UPGRADE: Glitch Title (Corrupted Look) ---
            // Layer 1: Offset Dark
            CreateGlitchText(canvasObj.transform, "EMOTION CORE", new Vector2(5, 95), new Color(0.2f, 0, 0, 0.8f));
            // Layer 2: Offset Bright
            CreateGlitchText(canvasObj.transform, "EMOTION CORE", new Vector2(-4, 104), new Color(1f, 0, 0, 0.3f));

            // Title Main
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.raycastTarget = false; // CRITICAL: No Blocking
            
            // Attach Subtle Glitch
            titleObj.tag = "GlitchText"; // Tag for Controller to find
            titleText.text = "EMOTION CORE";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.color = new Color(1f, 0.1f, 0.1f);  // Bright Red
            titleText.fontSize = 95; // Large, intimidating
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.horizontalOverflow = HorizontalWrapMode.Overflow;
            titleText.rectTransform.anchoredPosition = new Vector2(0, 100);
            titleText.rectTransform.sizeDelta = new Vector2(800, 200);
            // Add native shadow for glow feel
            var shadow = titleObj.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = new Color(1f, 0f, 0f, 0.4f); // Red Glow
            shadow.effectDistance = new Vector2(0, 0); 
            // Add Outline for thickness
            var outline = titleObj.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);
            
            // Start Button
            CreateButton(canvasObj.transform, "StartButton", "INITIALIZE", new Vector2(0, -50), menuScript, "OnStartGame");
            
            // Exit Button
            CreateButton(canvasObj.transform, "ExitButton", "TERMINATE", new Vector2(0, -150), menuScript, "OnExitGame");

            // Save
            string path = "Assets/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(menuScene, path); 
            Debug.Log("Main Menu Generated!");
        }

        private static void CreateButton(Transform parent, string name, string label, Vector2 pos, MainMenuController ctrl, string method)
        {
            // Background
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            UnityEngine.UI.Image img = btnObj.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0.1f, 0.0f, 0.0f, 0.9f); // Dark Red tint
            
            // Outline
            var outl = btnObj.AddComponent<UnityEngine.UI.Outline>();
            outl.effectColor = new Color(0.8f, 0f, 0f, 0.6f); // Red Outline
            outl.effectDistance = new Vector2(1, -1);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f); // Center
            rect.anchorMax = new Vector2(0.5f, 0.5f); // Center
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(350, 70); // Wider, taller

            // Button Component - INTERACTIVE COLORS
            UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0.8f, 0.8f, 0.8f, 1f); 
            colors.highlightedColor = new Color(1f, 0.5f, 0.5f, 1f); // Reddish hover
            colors.pressedColor = new Color(0.6f, 0.0f, 0.0f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;
            
            // NOTE: Interaction logic is now handled in MainMenuController.Start()
            // This prevents serialization issues where runtime listeners are lost on build.
            
            // Text
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            var tmp = txtObj.AddComponent<UnityEngine.UI.Text>();
            tmp.raycastTarget = false; // CRITICAL: Don't block clicks
            tmp.text = label;
            tmp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tmp.color = new Color(0.8f, 0.9f, 0.9f); // Off-white
            tmp.fontSize = 28; // Slightly smaller, crisper
            tmp.alignment = TextAnchor.MiddleCenter;
            tmp.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;

            // Events: Handled by binding in MainMenuController.Start() via Name
            if (method == "OnStartGame") btnObj.name = "StartButton"; // Critical for finding
            else if (method == "OnExitGame") btnObj.name = "ExitButton";
        }

        // Helper for Glitch Text
        private static void CreateGlitchText(Transform parent, string text, Vector2 pos, Color col)
        {
            GameObject obj = new GameObject("GlitchText");
            obj.tag = "GlitchText"; // For Controller
            obj.transform.SetParent(parent, false);
            var txt = obj.AddComponent<UnityEngine.UI.Text>();
            txt.raycastTarget = false; // CRITICAL
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.color = col;
            txt.fontSize = 95;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.rectTransform.anchoredPosition = pos;
            txt.rectTransform.sizeDelta = new Vector2(800, 200);
        }

        private static void SetupSystems(Material glitchMat)
        {
            GameObject sys = new GameObject("___SYSTEMS___");
            sys.AddComponent<GameManager>();
            sys.AddComponent<MemoryCore>();
            sys.AddComponent<EmotionEngine>();
            sys.AddComponent<TrustEngine>();
            sys.AddComponent<DialogueBrain>();
            sys.AddComponent<ProceduralAudio>(); // Ambient Drone
            var gc = sys.AddComponent<EmotionCore.Visuals.GlitchController>();
            gc.GlitchMaterial = glitchMat;
            
            // Setup UI
            GameObject canvas = new GameObject("Canvas");
            Canvas c = canvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            var uiMgr = sys.AddComponent<UIControlManager>();
            
            // Text
            GameObject txt = new GameObject("DialogueText");
            txt.transform.SetParent(canvas.transform, false);
            var tmp = txt.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.fontSize = 24;
            tmp.alignment = TMPro.TextAlignmentOptions.Bottom;
            tmp.rectTransform.anchoredPosition = new Vector2(0, 100);
            tmp.rectTransform.sizeDelta = new Vector2(800, 100);
            uiMgr.DialogueText = tmp;
        }

        private static GameObject SetupPlayer(Vector3 pos)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            p.name = "Player";
            p.transform.position = pos;
            DestroyImmediate(p.GetComponent<CapsuleCollider>());
            p.AddComponent<CharacterController>();
            var pc = p.AddComponent<PlayerController>();
            
            GameObject cam = new GameObject("MainCamera");
            cam.transform.SetParent(p.transform, false);
            cam.transform.localPosition = new Vector3(0, 0.6f, 0);
            var c = cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.AddComponent<AudioListener>();
            
            // FX
            cam.AddComponent<FX.GlitchEffect>();
            
            pc.CameraTransform = cam.transform;
            
            var interact = cam.AddComponent<InteractionController>();
            interact.PlayerCamera = c;
            interact.InteractableLayer = LayerMask.GetMask("Default", "Interactable");
            
            return p;
        }

        // --- Geometry Builders ---

        private static void BuildRoom(Vector3 center, Vector3 size, string name, bool[] openWalls = null)
        {
            GameObject root = new GameObject(name);
            root.transform.position = center;
            
            Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Concrete.mat"); // Use improved mat
            if(wallMat == null) wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_CyberWall.mat");
            
            Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_DarkFloor.mat");

            // Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(root.transform);
            floor.transform.localPosition = new Vector3(0, -0.5f, 0);
            floor.transform.localScale = new Vector3(size.x, 1, size.z);
            if(floorMat) 
            {
                var r = floor.GetComponent<Renderer>();
                r.material = floorMat;
                r.material.mainTextureScale = new Vector2(size.x / 4f, size.z / 4f);
            }

            // Ceiling
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(root.transform);
            ceiling.transform.localPosition = new Vector3(0, size.y + 0.5f, 0);
            ceiling.transform.localScale = new Vector3(size.x, 1, size.z);
            if(wallMat) 
            {
                var r = ceiling.GetComponent<Renderer>();
                r.material = wallMat;
                r.material.mainTextureScale = new Vector2(size.x / 4f, size.z / 4f);
            }

            // Walls (Simplistic box) - Order: North (+Z), South (-Z), East (+X), West (-X)
            // openWalls: [North, South, East, West]
            if (openWalls == null || !openWalls[0]) 
                CreateWall(root.transform, new Vector3(0, size.y/2, size.z/2), new Vector3(size.x, size.y, 1), wallMat);
            
            if (openWalls == null || !openWalls[1]) 
                CreateWall(root.transform, new Vector3(0, size.y/2, -size.z/2), new Vector3(size.x, size.y, 1), wallMat);
            
            if (openWalls == null || !openWalls[2])
                CreateWall(root.transform, new Vector3(size.x/2, size.y/2, 0), new Vector3(1, size.y, size.z), wallMat);
            
            if (openWalls == null || !openWalls[3])
                CreateWall(root.transform, new Vector3(-size.x/2, size.y/2, 0), new Vector3(1, size.y, size.z), wallMat);
            
            // Lighting
            GameObject lightObj = new GameObject("Light_Source");
            lightObj.transform.SetParent(root.transform);
            lightObj.transform.localPosition = new Vector3(0, size.y - 1, 0);
            var l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = Mathf.Max(size.x, size.z) * 1.5f;
            l.intensity = 1.0f;
            l.color = name.Contains("Act4") || name.Contains("Finale") ? Color.red : new Color(0.6f, 1f, 1f); // Cyan vs Red

            // --- FURNITURE POPULATION ---
            if (name.Contains("Act1")) // Cell
            {
                // Bed
                PropGenerator.CreateDesk(root.transform, new Vector3(-3, 0.5f, -3), 0); // Reusing desk as "bed slab"
            }
            else if (name.Contains("Act2")) // Office
            {
                if (name.Contains("Main"))
                {
                    // Work Area
                    PropGenerator.CreateDesk(root.transform, new Vector3(2, 0, 2), 45);
                    PropGenerator.CreateChair(root.transform, new Vector3(1.5f, 0, 1.5f), 225);
                    
                    PropGenerator.CreateDesk(root.transform, new Vector3(-2, 0, -2), -45);
                    PropGenerator.CreateChair(root.transform, new Vector3(-1.5f, 0, -1.5f), 135);
                }
                else if (name.Contains("Annex"))
                {
                    // Server / Storage Area
                    PropGenerator.CreateServerRack(root.transform, new Vector3(3, 0, 3), 0);
                    PropGenerator.CreateServerRack(root.transform, new Vector3(-3, 0, 3), 0);
                    PropGenerator.CreateServerRack(root.transform, new Vector3(3, 0, -3), 0);
                    PropGenerator.CreateServerRack(root.transform, new Vector3(-3, 0, -3), 0);
                }
            }
            else if (name.Contains("Act3")) // Bedroom
            {
                PropGenerator.CreateLamp(root.transform, new Vector3(4, 0, 4));
                PropGenerator.CreateChair(root.transform, new Vector3(0, 0, 0), 0);
            }
            else if (name.Contains("Finale"))
            {
                PropGenerator.CreateServerRack(root.transform, new Vector3(-8, 0, 8), 45);
                PropGenerator.CreateServerRack(root.transform, new Vector3(8, 0, 8), -45);
            }
        }

        private static void BuildHallway(Vector3 start, float length, string name)
        {
            // Just a long corridor
            BuildRoom(start + new Vector3(length/2, 0, 0), new Vector3(length, 4, 4), name);
        }

        private static void CreateWall(Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject w = GameObject.CreatePrimitive(PrimitiveType.Cube);
            w.transform.SetParent(parent);
            w.transform.localPosition = pos;
            w.transform.localScale = scale;
            if(mat) 
            {
                var r = w.GetComponent<Renderer>();
                r.material = mat;
                // UV Tiling: Material instance copy
                r.material.mainTextureScale = new Vector2(scale.x / 4f, scale.y / 4f);
            }
        }

        // --- Puzzle Setup ---

        private static void SetupPuzzle1(Vector3 pos)
        {
            GameObject p1 = new GameObject("Puzzle1");
            p1.transform.position = pos;
            var lockScript = p1.AddComponent<Puzzle1_Lock>();
            
            // Console
            GameObject console = GameObject.CreatePrimitive(PrimitiveType.Cube);
            console.transform.SetParent(p1.transform);
            console.transform.localPosition = Vector3.zero;
            console.transform.localScale = new Vector3(1, 1.5f, 0.5f);
            
            // Keypad
            GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btn.name = "KeypadButton";
            btn.transform.SetParent(p1.transform);
            btn.transform.localPosition = new Vector3(0, 0.2f, -0.3f);
            btn.transform.localScale = new Vector3(0.2f, 0.2f, 0.1f);
            var k = btn.AddComponent<KeypadButton>();
            k.Digit = 1;
            k.ParentLock = lockScript;
        }

        private static void SetupPuzzle4(Vector3 pos)
        {
             // Placeholder for Settings Monitor
             GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Cube);
             monitor.name = "SettingsMonitor";
             monitor.transform.position = pos;
             // Ideally we'd spawn a WorldSpace Canvas here
        }

        private static void SetupPuzzle5_FakeCrash()
        {
            GameObject p5 = new GameObject("Puzzle5_FakeCrash");
            p5.AddComponent<Puzzle5_FakeCrash>();
        }

        private static void SetupPuzzle7(Vector3 pos)
        {
            GameObject root = new GameObject("Puzzle7_Memories");
            root.AddComponent<Puzzle7_SharedMemory>();
            
            GameObject mem1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mem1.transform.position = pos;
            mem1.name = "MemoryOrb_1";
            mem1.AddComponent<SphereCollider>();
            // Add Interactor script logic
        }

        private static void SetupPuzzle8_Door(Vector3 pos)
        {
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "ForbiddenDoor";
            door.transform.position = pos;
            door.transform.localScale = new Vector3(2, 3, 0.2f);
            var p8 = new GameObject("Puzzle8").AddComponent<Puzzle8_PleaseDont>();
            p8.ForbiddenDoor = door.AddComponent<Interactable>(); // Needs concrete implementation
        }

        private static void SetupFinalPuzzle(Vector3 pos)
        {
            new GameObject("FinalPuzzle").AddComponent<FinalPuzzle_Breakup>();
        }
    }
}
#endif
