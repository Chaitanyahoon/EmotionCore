using UnityEngine;
using UnityEngine.UI;
using EmotionCore.Systems;

namespace EmotionCore.Puzzles
{
    public class Puzzle4_Settings : PuzzleBase
    {
        [Header("UI References")]
        public Slider BrightnessSlider;
        public Slider VolumeSlider;
        public TMPro.TextMeshProUGUI SecretMessageText;

        [Header("Solution")]
        public float TargetBrightness = 0.42f; // Specific value
        public float TargetVolume = 0.66f;
        public float Tolerance = 0.05f;

        private bool _foundSecret = false;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "SettingsMenu";

            if (BrightnessSlider) BrightnessSlider.onValueChanged.AddListener(CheckValues);
            if (VolumeSlider) VolumeSlider.onValueChanged.AddListener(CheckValues);
            
            if (SecretMessageText) SecretMessageText.alpha = 0;
        }

        private void CheckValues(float val)
        {
            if (IsSolved || _foundSecret) return;

            bool brightnessMatch = Mathf.Abs(BrightnessSlider.value - TargetBrightness) < Tolerance;
            bool volumeMatch = Mathf.Abs(VolumeSlider.value - TargetVolume) < Tolerance;

            if (brightnessMatch && volumeMatch)
            {
                RevealSecret();
            }
        }

        private void RevealSecret()
        {
            _foundSecret = true;
            SecretMessageText.text = "You trust me... don't you? Code: 8849";
            SecretMessageText.alpha = 1;

            // Whisper audio
            // AudioManager.Instance.PlayWhisper("You trust me");

            // Grant achievement trait
            TrustEngine.Instance.RegisterDisobedience(); // Wait, no, this builds trust? Or is investigating settings "Curious"?
            TrustEngine.Instance.SetTrait("Curious", true);
            
            Solve();
        }
    }
}
