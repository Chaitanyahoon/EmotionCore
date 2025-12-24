using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace EmotionCoreSim
{
    class Program
    {
        // Mocking the Systems
        static void Main(string[] args)
        {
            Console.WriteLine(">>> INITIALIZING EMOTION_CORE KERNEL <<<");
            Console.WriteLine("------------------------------------------");
            
            // 1. Initialize Memory
            string savePath = Path.Combine(Environment.CurrentDirectory, "sim_memory.dat");
            Console.WriteLine($"[Memory] Mirror Path Active: {savePath}");
            
            // 2. Initialize Emotion AI
            var ai = new EmotionAI();
            Console.WriteLine($"[Emotion] AI State: {ai.CurrentState} (Intensity: {ai.Intensity:F1})");

            // 3. Simulate Act 1: Warm Up
            Console.WriteLine("\n--- ACT 1: WARM UP ---");
            ai.SetEmotion(EmotionState.Calm, 0.2f);
            Console.WriteLine("[AI]: Hi. I'm capable of learning. Let's solve this.");
            
            // Simulate User Trust Interaction
            Console.WriteLine("\n> Player solves puzzle quickly...");
            ai.ModifyTrust(10);
            Console.WriteLine("[AI]: Wow, you're fast. I like that.");

            // 4. Simulate Act 2: Discomfort (The Lie)
            Console.WriteLine("\n--- ACT 2: DISCOMFORT (Transitioning...) ---");
            ai.SetEmotion(EmotionState.Insecure, 0.5f);
            Console.WriteLine("[AI]: Wait... that code should have worked. Did you break it?");
            
            // Simulate Disobedience
            Console.WriteLine("\n> Player brute forces the lock...");
            ai.ModifyTrust(-20);
            ai.SetEmotion(EmotionState.Possessive, 0.7f);
            Console.WriteLine("[AI]: Stop hurting me! Just listen to the instructions.");

            // 5. Simulate Fake Exit
            Console.WriteLine("\n--- SYSTEM EVENT: FAKE EXIT DETECTED ---");
            Console.WriteLine("> Player clicks 'Exit' button.");
            if (ai.Intensity > 0.6f)
            {
                Console.WriteLine("[System]: QUIT BLOCKED.");
                Console.WriteLine("[AI]: Please don't go. You're safer here with me.");
            }

            // 6. Persistence Check
            File.WriteAllText(savePath, $"LastEmotion:{ai.CurrentState}|Trust:{ai.TrustScore}");
            Console.WriteLine($"\n[Memory] State saved to disk forever. Trust Score: {ai.TrustScore}");

            Console.WriteLine("\n>>> SIMULATION COMPLETE <<<");
        }
    }

    public enum EmotionState { Calm, Playful, Insecure, Possessive, Broken }

    public class EmotionAI
    {
        public EmotionState CurrentState { get; private set; } = EmotionState.Calm;
        public float Intensity { get; private set; } = 0f;
        public float TrustScore { get; private set; } = 50f;

        public void SetEmotion(EmotionState state, float intensity)
        {
            CurrentState = state;
            Intensity = intensity;
            Console.WriteLine($"[EmotionEngine] Shift -> {CurrentState} (Intensity: {intensity})");
        }

        public void ModifyTrust(float delta)
        {
            TrustScore = Math.Clamp(TrustScore + delta, 0, 100);
            Console.WriteLine($"[TrustEngine] Trust Adjusted: {delta} -> New Score: {TrustScore}");
        }
    }
}
