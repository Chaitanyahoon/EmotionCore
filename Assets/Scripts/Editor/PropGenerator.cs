using UnityEngine;
using UnityEditor;

namespace EmotionCore.EditorTools
{
    public static class PropGenerator
    {
        // --- MATERIALS (Helper to get existing ones) ---
        private static Material GetMat(string name)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/{name}.mat");
            if (!mat) mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Concrete.mat"); // Fallback
            return mat;
        }

        // --- FURNITURE ---

        public static void CreateDesk(Transform parent, Vector3 pos, float angle)
        {
            GameObject root = new GameObject("Prop_HighPoly_Desk");
            root.transform.SetParent(parent);
            root.transform.localPosition = pos;
            root.transform.localRotation = Quaternion.Euler(0, angle, 0);

            Material tableMat = GetMat("Mat_DarkFloor");
            Material metalMat = GetMat("Mat_Concrete");

            // 1. Tabletop (Sleek, thin)
            CreateBox(root.transform, new Vector3(0, 0.75f, 0), new Vector3(1.8f, 0.04f, 0.9f), tableMat);

            // 2. Legs (Metal Frame Style)
            float legThick = 0.05f;
            CreateBox(root.transform, new Vector3(-0.85f, 0.375f, 0), new Vector3(legThick, 0.75f, 0.8f), metalMat); // Left Plate
            CreateBox(root.transform, new Vector3(0.85f, 0.375f, 0), new Vector3(legThick, 0.75f, 0.8f), metalMat);  // Right Plate
            CreateBox(root.transform, new Vector3(0, 0.3f, 0.4f), new Vector3(1.7f, 0.02f, 0.02f), metalMat); // Crossbar

            // 3. High-Detail Computer Setup
            CreateMonitor(root.transform, new Vector3(0, 0.77f, 0.2f));
            CreateKeyboard(root.transform, new Vector3(0, 0.77f, -0.2f));
            CreateMouse(root.transform, new Vector3(0.4f, 0.77f, -0.2f));
            CreatePCUnit(root.transform, new Vector3(0.6f, 1.1f, 0.2f)); // Mini-tower on desk
        }

        public static void CreateChair(Transform parent, Vector3 pos, float angle)
        {
            GameObject root = new GameObject("Prop_HighPoly_Chair");
            root.transform.SetParent(parent);
            root.transform.localPosition = pos;
            root.transform.localRotation = Quaternion.Euler(0, angle, 0);

            Material seatMat = GetMat("Mat_Concrete"); // Fabric-ish
            Material metalMat = GetMat("Mat_DarkFloor"); // Base

            // Seat
            CreateBox(root.transform, new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.08f, 0.5f), seatMat);
            
            // Backrest (Curved simulation via 3 segments)
            CreateBox(root.transform, new Vector3(0, 0.9f, -0.25f), new Vector3(0.4f, 0.6f, 0.05f), seatMat);
            CreateBox(root.transform, new Vector3(-0.15f, 0.9f, -0.22f), new Vector3(0.1f, 0.6f, 0.05f), seatMat, new Vector3(0, -20, 0));
            CreateBox(root.transform, new Vector3(0.15f, 0.9f, -0.22f), new Vector3(0.1f, 0.6f, 0.05f), seatMat, new Vector3(0, 20, 0));

            // Central Pillar
            CreateBox(root.transform, new Vector3(0, 0.25f, 0), new Vector3(0.08f, 0.5f, 0.08f), metalMat);

            // Wheel Base (Star shape)
            for(int i = 0; i < 5; i++)
            {
                GameObject leg = CreateBox(root.transform, new Vector3(0, 0.05f, 0), new Vector3(0.05f, 0.05f, 0.35f), metalMat);
                leg.transform.Rotate(0, i * 72, 0);
                leg.transform.Translate(0, 0, 0.15f);
            }
        }

        public static void CreateServerRack(Transform parent, Vector3 pos, float angle)
        {
            GameObject root = new GameObject("Prop_HighPoly_Racks");
            root.transform.SetParent(parent);
            root.transform.localPosition = pos;
            root.transform.localRotation = Quaternion.Euler(0, angle, 0);

            Material frameMat = GetMat("Mat_DarkFloor");
            Material bladeMat = GetMat("Mat_Concrete");

            // Frame
            CreateBox(root.transform, new Vector3(0, 1.2f, 0), new Vector3(1.0f, 2.4f, 1.0f), frameMat);

            // Server Blades (Stacked)
            int bladeCount = 12;
            for(int i = 0; i < bladeCount; i++)
            {
                float yVal = 0.2f + (i * 0.18f);
                // Blade Unit
                CreateBox(root.transform, new Vector3(0, yVal, -0.05f), new Vector3(0.85f, 0.15f, 0.9f), bladeMat);
                
                // Status Lights
                for(int j = 0; j < 4; j++)
                {
                    float xOff = -0.3f + (j * 0.2f);
                    CreateLED(root.transform, new Vector3(xOff, yVal, -0.51f), Random.value > 0.5f ? Color.green : Color.red);
                }
            }
        }

        public static void CreateLamp(Transform parent, Vector3 pos)
        {
             GameObject root = new GameObject("Prop_HighPoly_Lamp");
             root.transform.SetParent(parent);
             root.transform.localPosition = pos;

             // Pole
             CreateBox(root.transform, new Vector3(0, 0.75f, 0), new Vector3(0.05f, 1.5f, 0.05f), null);
             
             // Base
             CreateBox(root.transform, new Vector3(0, 0.025f, 0), new Vector3(0.3f, 0.05f, 0.3f), null);

             // Shade (4 panels for definition)
             float h = 1.6f;
             CreateBox(root.transform, new Vector3(0, h, 0.15f), new Vector3(0.3f, 0.3f, 0.02f), null);
             CreateBox(root.transform, new Vector3(0, h, -0.15f), new Vector3(0.3f, 0.3f, 0.02f), null);
             CreateBox(root.transform, new Vector3(0.15f, h, 0), new Vector3(0.02f, 0.3f, 0.3f), null);
             CreateBox(root.transform, new Vector3(-0.15f, h, 0), new Vector3(0.02f, 0.3f, 0.3f), null);
             
             // Light
             GameObject l = new GameObject("Lamp_Light");
             l.transform.SetParent(root.transform, false);
             l.transform.localPosition = new Vector3(0, 1.5f, 0);
             var light = l.AddComponent<Light>();
             light.type = LightType.Point;
             light.range = 5;
             light.intensity = 2;
             light.color = new Color(1f, 0.8f, 0.6f); 
        }

        // --- COMPONENT HELPERS ---

        private static GameObject CreateBox(Transform parent, Vector3 pos, Vector3 scale, Material mat, Vector3? rotation = null)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            if (rotation.HasValue) obj.transform.localRotation = Quaternion.Euler(rotation.Value);
            
            if (mat) obj.GetComponent<Renderer>().material = mat;
            else obj.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);
            return obj;
        }

        private static void CreateMonitor(Transform parent, Vector3 pos)
        {
             // Base
             CreateBox(parent, pos + new Vector3(0, 0, 0), new Vector3(0.2f, 0.02f, 0.2f), null);
             // Neck
             CreateBox(parent, pos + new Vector3(0, 0.1f, 0.05f), new Vector3(0.05f, 0.2f, 0.02f), null);
             // Screen Body
             GameObject screen = CreateBox(parent, pos + new Vector3(0, 0.25f, 0), new Vector3(0.6f, 0.35f, 0.03f), null);
             
             // Emissive Display
             GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Quad);
             glow.transform.SetParent(screen.transform);
             glow.transform.localPosition = new Vector3(0, 0, -0.51f);
             glow.transform.localRotation = Quaternion.Euler(0, 180, 0);
             glow.transform.localScale = new Vector3(0.9f, 0.9f, 1); // Bezel margin
             var r = glow.GetComponent<Renderer>();
             r.material = new Material(Shader.Find("Standard"));
             r.material.EnableKeyword("_EMISSION");
             r.material.SetColor("_EmissionColor", Color.cyan * 1.2f);
             r.material.color = Color.black;
        }

        private static void CreateKeyboard(Transform parent, Vector3 pos)
        {
            // Board
            CreateBox(parent, pos, new Vector3(0.5f, 0.02f, 0.2f), null);
            // Simulate Keys (Just a texture or a few bumps? Let's do a few rows)
            // Row 1
            CreateBox(parent, pos + new Vector3(0, 0.015f, 0.05f), new Vector3(0.45f, 0.01f, 0.04f), null).GetComponent<Renderer>().material.color = Color.gray;
        }

        private static void CreateMouse(Transform parent, Vector3 pos)
        {
            CreateBox(parent, pos, new Vector3(0.08f, 0.03f, 0.12f), null);
        }

        private static void CreatePCUnit(Transform parent, Vector3 pos)
        {
             CreateBox(parent, pos, new Vector3(0.2f, 0.5f, 0.5f), null);
             // Power LED
             CreateLED(parent, pos + new Vector3(0, 0.2f, -0.26f), Color.blue);
        }

        private static void CreateLED(Transform parent, Vector3 pos, Color col)
        {
            GameObject led = GameObject.CreatePrimitive(PrimitiveType.Quad);
            led.transform.SetParent(parent);
            led.transform.localPosition = pos;
            led.transform.localScale = new Vector3(0.02f, 0.02f, 1);
            led.transform.localRotation = Quaternion.Euler(0, 180, 0); // Face forward (-Z)
            var r = led.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Standard"));
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", col * 3f);
        }
    }
}
