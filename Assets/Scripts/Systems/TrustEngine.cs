using UnityEngine;

namespace EmotionCore.Systems
{
    public class TrustEngine : MonoBehaviour
    {
        public static TrustEngine Instance { get; private set; }

        [System.Serializable]
        public class TrustProfile
        {
            public float TrustScore = 50f; // 0-100
            public int DisobedienceCount = 0;
            public int HesitationCount = 0;
            public bool IsAggressive = false; // Toggled by brute force attempts
        }

        public TrustProfile PlayerProfile = new TrustProfile();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ModifyTrust(float amount)
        {
            PlayerProfile.TrustScore = Mathf.Clamp(PlayerProfile.TrustScore + amount, 0f, 100f);
            Debug.Log($"[TrustEngine] Trust Updated: {PlayerProfile.TrustScore}");
            
            CheckTrustThresholds();
        }

        public void RegisterDisobedience()
        {
            PlayerProfile.DisobedienceCount++;
            ModifyTrust(-10f);
            // Trigger EmotionEngine reaction?
        }

        private void CheckTrustThresholds()
        {
            if (PlayerProfile.TrustScore < 20f)
            {
                // Trigger paranoid/insecure state
            }
        }
    }
}
