using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace EmotionCore.Systems
{
    public class MemoryCore : MonoBehaviour
    {
        public static MemoryCore Instance { get; private set; }

        private string _primarySavePath;
        private string _mirrorSavePath;
        private string _hiddenSavePath;

        // In-Memory Database of "Facts"
        private Dictionary<string, string> _memoryStore = new Dictionary<string, string>();
        private const string MEMORY_FILE = "neuro_map.bin";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePaths();
                LoadAllMemories();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void InitializePaths()
        {
            _primarySavePath = Path.Combine(Application.persistentDataPath, "memories");
            _mirrorSavePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "EmotionCore_Logs");
            _hiddenSavePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "EmotionCore_Sys");

            EnsureDirectory(_primarySavePath);
            EnsureDirectory(_mirrorSavePath);
            EnsureDirectory(_hiddenSavePath);
        }

        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        // --- PUBLIC API ---

        public void SetFlag(string key, bool value)
        {
            SetValue(key, value.ToString());
            Debug.Log($"[Memory] Flag Set: {key} = {value}");
        }

        public bool GetFlag(string key, bool defaultValue = false)
        {
            if (_memoryStore.TryGetValue(key, out string val))
            {
                return bool.Parse(val);
            }
            return defaultValue;
        }

        public void IncrementStat(string key)
        {
            int current = GetStat(key);
            SetValue(key, (current + 1).ToString());
        }

        public int GetStat(string key)
        {
            if (_memoryStore.TryGetValue(key, out string val))
            {
                if (int.TryParse(val, out int result)) return result;
            }
            return 0;
        }

        private void SetValue(string key, string value)
        {
            if (_memoryStore.ContainsKey(key))
                _memoryStore[key] = value;
            else
                _memoryStore.Add(key, value);

            // Auto-save on every brain change to prevent "resetting" by force-quit
            SaveAllMemories();
        }

        // --- PERSISTENCE ---

        private void SaveAllMemories()
        {
            // Simple Line-Based Serialization: KEY|VALUE
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in _memoryStore)
            {
                sb.AppendLine($"{kvp.Key}|{kvp.Value}");
            }

            string encrypted = Encrypt(sb.ToString());
            
            WriteToFile(Path.Combine(_primarySavePath, MEMORY_FILE), encrypted);
            WriteToFile(Path.Combine(_mirrorSavePath, MEMORY_FILE), encrypted);
            WriteToFile(Path.Combine(_hiddenSavePath, MEMORY_FILE), encrypted);
        }

        private void LoadAllMemories()
        {
            // Try load from Primary -> Mirror -> Hidden
            string content = TryReadFile(Path.Combine(_primarySavePath, MEMORY_FILE));
            if (content == null) content = TryReadFile(Path.Combine(_mirrorSavePath, MEMORY_FILE));
            if (content == null) content = TryReadFile(Path.Combine(_hiddenSavePath, MEMORY_FILE));

            if (!string.IsNullOrEmpty(content))
            {
                ParseDatabase(Decrypt(content));
            }
        }

        private void ParseDatabase(string rawData)
        {
            _memoryStore.Clear();
            string[] lines = rawData.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 2)
                {
                    _memoryStore[parts[0]] = parts[1];
                }
            }
            Debug.Log($"[Memory] Loaded {_memoryStore.Count} neural pathways.");
        }

        private string TryReadFile(string path)
        {
            if (File.Exists(path)) return File.ReadAllText(path);
            return null;
        }

        private void WriteToFile(string path, string content)
        {
            try { File.WriteAllText(path, content); }
            catch (System.Exception e) { Debug.LogError($"[Memory] Write Error: {e.Message}"); }
        }

        // --- ENCRYPTION (Obfuscation + Salt) ---
        private const string SALT = "DoNotTrustTheMachine_8849";

        private string Encrypt(string plain)
        {
            // Simple XOR + Base64 obfuscation to prevent casual text editing
            // Not millitary grade, but enough to stop a player opening notepad
            string salted = plain + SALT;
            byte[] bytes = Encoding.UTF8.GetBytes(salted);
            for (int i = 0; i < bytes.Length; i++) {
                bytes[i] = (byte)(bytes[i] ^ 0x42); // XOR with fixed byte
            }
            return System.Convert.ToBase64String(bytes);
        }

        private string Decrypt(string cipher)
        {
            try { 
                byte[] bytes = System.Convert.FromBase64String(cipher);
                for (int i = 0; i < bytes.Length; i++) {
                    bytes[i] = (byte)(bytes[i] ^ 0x42);
                }
                string decoded = Encoding.UTF8.GetString(bytes);
                
                // Remove salt verification
                if (decoded.EndsWith(SALT))
                {
                    return decoded.Substring(0, decoded.Length - SALT.Length);
                }
                return ""; // Corrupted or tampered
            }
            catch { return ""; }
        }
    }
}
