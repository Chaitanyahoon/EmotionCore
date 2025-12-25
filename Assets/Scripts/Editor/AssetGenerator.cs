#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EmotionCore.EditorTools
{
    public class AssetGenerator
    {
        public static Material CreateWallMaterial()
        {
            // Higher Res, thicker lines, emission support
            Texture2D tex = GenerateGridTexture(Color.black, new Color(0.2f, 0.9f, 0.9f), 1024, 128, 4);
            return SaveMaterial("Mat_CyberWall", tex, Color.white, true);
        }

        public static Material CreateFloorMaterial()
        {
             // Darker, cleaner grid
             Texture2D tex = GenerateGridTexture(new Color(0.05f, 0.05f, 0.05f), new Color(0.6f, 0.6f, 0.6f), 1024, 256, 2);
             return SaveMaterial("Mat_DarkFloor", tex, Color.gray, false);
        }

        public static Material CreateConcreteMaterial()
        {
            // Layered noise for realism
            Texture2D tex = GenerateLayeredNoiseTexture(1024);
            return SaveMaterial("Mat_Concrete", tex, new Color(0.6f, 0.6f, 0.65f), false);
        }

        public static Material CreateGlitchMaterial()
        {
            Shader shader = Shader.Find("EmotionCore/DigitalGlitch");
            if(shader == null) shader = Shader.Find("Standard"); // Fallback

            Material mat = new Material(shader);
            string path = "Assets/Materials/Mat_Glitch.mat";
            
            CheckDirectory("Assets/Materials");
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static Texture2D GenerateGridTexture(Color bg, Color line, int size, int cellSize, int lineThickness)
        {
            Texture2D tex = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isLine = (x % cellSize < lineThickness) || (y % cellSize < lineThickness);
                    pixels[y * size + x] = isLine ? line : bg;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Texture2D GenerateLayeredNoiseTexture(int size)
        {
            Texture2D tex = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            float scale = 10f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float xCoord = (float)x / size * scale;
                    float yCoord = (float)y / size * scale;
                    
                    // FBM (Fractal Brownian Motion)
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    sample += Mathf.PerlinNoise(xCoord * 2, yCoord * 2) * 0.5f;
                    sample += Mathf.PerlinNoise(xCoord * 4, yCoord * 4) * 0.25f;
                    sample /= 1.75f; // Normalize roughly

                    // Contrast push
                    sample = Mathf.Clamp01(sample * 1.2f - 0.1f);
                    
                    pixels[y * size + x] = new Color(sample, sample, sample);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private static Material SaveMaterial(string name, Texture2D texture, Color tint, bool emit)
        {
            CheckDirectory("Assets/Materials");
            CheckDirectory("Assets/Textures");

            // Save Texture
            string texPath = $"Assets/Textures/{name}_Tex.png";
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(texPath, bytes);
            AssetDatabase.ImportAsset(texPath);

            Texture2D importedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

            // Create Material
            Material mat = new Material(Shader.Find("Standard")); 
            mat.mainTexture = importedTex;
            mat.color = tint;
            
            // Visual Polish
            mat.SetFloat("_Glossiness", 0.8f); // Shiny
            mat.SetFloat("_Metallic", 0.5f);
            
            if (emit)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetTexture("_EmissionMap", importedTex);
                mat.SetColor("_EmissionColor", tint * 2.0f); // Bright glow
            }

            string matPath = $"Assets/Materials/{name}.mat";
            AssetDatabase.CreateAsset(mat, matPath);

            return mat;
        }

        private static void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif
