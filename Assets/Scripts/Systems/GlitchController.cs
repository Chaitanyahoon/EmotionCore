using UnityEngine;
using UnityEngine.Rendering;
// Note: Requires Unity Post Processing or URP/HDRP Volume setup
// This script assumes a custom "Glitch" shader property exists on the material

namespace EmotionCore.Visuals
{
    public class GlitchController : MonoBehaviour
    {
        [Header("Settings")]
        public Material GlitchMaterial; // Assign a material with a glitch shader
        public float MaxIntensity = 1.0f;
        public float GlitchSpeed = 10f;

        private float _currentIntensity = 0f;
        private float _targetIntensity = 0f;

        private void Update()
        {
            // Smoothly interpolate intensity
            _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, Time.deltaTime * GlitchSpeed);
            
            if (GlitchMaterial != null)
            {
                // Standard property names for glitch shaders
                GlitchMaterial.SetFloat("_Intensity", _currentIntensity);
                GlitchMaterial.SetFloat("_ScanlineJitter", _currentIntensity * 0.5f);
                GlitchMaterial.SetFloat("_ColorDrift", _currentIntensity * 0.2f);
            }
        }

        public void SetGlitch(float intensity)
        {
            _targetIntensity = Mathf.Clamp(intensity, 0f, MaxIntensity);
        }

        public void TriggerBurst(float intensity, float duration)
        {
            StartCoroutine(BurstRoutine(intensity, duration));
        }

        private System.Collections.IEnumerator BurstRoutine(float intensity, float duration)
        {
            float original = _targetIntensity;
            _targetIntensity = intensity;
            yield return new WaitForSeconds(duration);
            _targetIntensity = original;
        }
    }
}
