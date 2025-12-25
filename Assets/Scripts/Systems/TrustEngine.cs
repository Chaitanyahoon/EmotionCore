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
            
            // New Personality Traits
            public bool IsCurious = false; // Explores everything
            public bool IsPatient = false; // Waits or listens
            public bool IsImpulsive = false; // Rushes
            public bool IsRiskTaker = false; // Takes dangerous paths
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

        private void Start()
        {
            LoadProfile();
        }

        public void ModifyTrust(float amount)
        {
            PlayerProfile.TrustScore = Mathf.Clamp(PlayerProfile.TrustScore + amount, 0f, 100f);
            Debug.Log($"[TrustEngine] Trust Updated: {PlayerProfile.TrustScore}");
            
            CheckTrustThresholds();
            SaveProfile();
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
                // Trigger paranoid/insecure state
            }
        }

        public void SetTrait(string trait, bool value)
        {
            switch(trait)
            {
                case "Curious": PlayerProfile.IsCurious = value; break;
                case "Patient": PlayerProfile.IsPatient = value; break;
                case "Impulsive": PlayerProfile.IsImpulsive = value; break;
                case "RiskTaker": PlayerProfile.IsRiskTaker = value; break;
                case "Aggressive": PlayerProfile.IsAggressive = value; break;
            }
            Debug.Log($"[TrustEngine] Trait Updated: {trait} = {value}");
            SaveProfile();
        }

        private void SaveProfile()
        {
            if (MemoryCore.Instance != null)
            {
                string json = JsonUtility.ToJson(PlayerProfile);
                MemoryCore.Instance.SetMemory("TrustProfile", json);
            }
        }

        private void LoadProfile()
        {
            if (MemoryCore.Instance != null)
            {
                string json = MemoryCore.Instance.GetMemory("TrustProfile");
                if (!string.IsNullOrEmpty(json))
                {
                    PlayerProfile = JsonUtility.FromJson<TrustProfile>(json);
                    Debug.Log("[TrustEngine] Profile Loaded from Memory.");
                }
            }
        }
    }
}
