using UnityEngine;
// using UnityEngine.Rendering.PostProcessing; // Assuming standard pipeline or URP equivalent

namespace EmotionCore.FX
{
    public class RealityGlitchController : MonoBehaviour
    {
        public static RealityGlitchController Instance { get; private set; }

        [Header("Settings")]
        public Material GlitchMaterial; // Custom Shader Graph material
        public float MaxIntensity = 1.0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Update()
        {
            if (Core.EmotionEngine.Instance != null)
            {
                var emotion = Core.EmotionEngine.Instance.CurrentEmotion;
                float intensity = Core.EmotionEngine.Instance.EmotionalIntensity;

                if (emotion == Core.EmotionEngine.EmotionState.Broken || 
                    emotion == Core.EmotionEngine.EmotionState.Possessive)
                {
                    // Crank up effects
                    SetGlitchAmount(intensity * MaxIntensity);
                }
                else
                {
                    // Decay to zero
                    SetGlitchAmount(Mathf.Lerp(GetGlitchAmount(), 0f, Time.deltaTime * 2f));
                }
            }
        }

        private void SetGlitchAmount(float val)
        {
            if (GlitchMaterial != null)
            {
                GlitchMaterial.SetFloat("_GlitchStrength", val);
                GlitchMaterial.SetFloat("_ScanlineIntensity", val * 0.5f);
            }
        }

        private float GetGlitchAmount()
        {
            if (GlitchMaterial != null) return GlitchMaterial.GetFloat("_GlitchStrength");
            return 0f;
        }
    }
}
