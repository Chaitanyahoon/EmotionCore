# UNITY IMPORT GUIDE

**Welcome to EmotionCore.**
You have the brain, the heart, and the horror of the AI. Now you need to give it a body in Unity.

## 1. Scene Setup
1.  **Create a New Scene**: Name it `MainChamber`.
2.  **Create System Object**:
    - Right-click Hierarchy -> Create Empty. Name it `_SYSTEM`.
    - **Add Components**:
        - `GameManager`
        - `EmotionEngine`
        - `MemoryCore`
        - `TrustEngine`
        - `NarrativeDirector`
        - `ActEvents`
        - `MicrophoneListener`
        - `AudioManager`
    - **Mark as DontDestroyOnLoad**: The `GameManager` script handles this automatically, but ensure this object is in your first scene.

## 2. UI Setup
1.  **Canvas**: Create a UI Canvas. Render Mode: `Screen Space - Overlay`.
2.  **UI Controller**:
    - Create an Empty Object named `UI_Manager`. Attach `UIControlManager`.
    - **Assign References**: transform the `DialogueText` slot to a `TextMeshPro - Text` object in your canvas.
3.  **Glitch Layer**:
    - Create a Panel (black, full screen). Set Alpha to 0.
    - Assign this to the `FakeOSLayer` slot in `UIControlManager`.

## 3. Post-Processing (The Glitch)
1.  **Volume**: Create a Global Volume in the scene.
2.  **Profile**: Add a new profile. Add `Chromatic Aberration` and `Film Grain`.
3.  **Controller**:
    - Create an object `FX_Controller`. Attach `RealityGlitchController`.
    - **Important**: You need to create a Material using a Glitch Shader (search "Unity URP Glitch Shader" online) and assign it to the `GlitchMaterial` slot.

## 4. Testing
1.  Press **Play**.
2.  See the log: `[GameManager] Initializing EmotionCore...`
3.  **Wait**: After 2 seconds, the Act 1 Intro dialogue will begin typing on screen.
4.  **Debug**: Use the top menu `EmotionCore > 🔧 Reset Memory` if you want to restart the "First Time Experience".

## 5. Building
- When building for Windows, ensure "Microphone Usage" description is set in Player Settings if required (more common on macOS/iOS).

**You are now the Architect.** Good luck.
