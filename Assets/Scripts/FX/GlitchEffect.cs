using UnityEngine;

namespace EmotionCore.FX
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class GlitchEffect : MonoBehaviour
    {
        public Material glitchMaterial;
        
        [Range(0, 1)] public float intensity = 0f;
        [Range(0, 1)] public float colorSplit = 0.5f;

        // Internal
        private Shader shader;

        void Start()
        {
            shader = Shader.Find("Hidden/DigitalGlitch");
            if(glitchMaterial == null)
            {
                glitchMaterial = new Material(shader);
                glitchMaterial.hideFlags = HideFlags.DontSave;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (intensity == 0 || glitchMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            glitchMaterial.SetFloat("_Intensity", intensity);
            glitchMaterial.SetFloat("_ColorSplit", colorSplit);
            glitchMaterial.SetFloat("_TimeX", Time.time);

            Graphics.Blit(source, destination, glitchMaterial);
        }
    }
}
