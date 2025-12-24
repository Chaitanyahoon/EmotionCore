using UnityEngine;

namespace EmotionCore.Puzzles
{
    public class LoopRoomManager : MonoBehaviour
    {
        [Header("Puzzle Logic")]
        public Transform Ref_Player;
        public Transform Ref_StartPoint;
        public Transform Ref_LoopTrigger;
        
        [Header("Solution")]
        public float TimeStandingStillRequired = 5.0f;
        private float _standTimer = 0f;
        private bool _isEscaped = false;

        private void Update()
        {
            if (_isEscaped || Ref_Player == null) return;

            // Check if player is moving
            // Using loose magnitude check
            // (Assuming CharacterController or Rigidbody logic exists on player)
            Vector3 velocity = Vector3.zero; // Placeholder: Ref_Player.GetComponent<Rigidbody>().velocity;
            
            // Mock velocity for logic: 
            // if(Input.anyKey) velocity = Vector3.forward; 

            if (velocity.magnitude < 0.1f)
            {
                _standTimer += Time.deltaTime;
                if (_standTimer > 2.0f) 
                {
                    // Hint
                    UI.UIControlManager.Instance.ShowDialogue("...why did you stop?");
                }
                
                if (_standTimer >= TimeStandingStillRequired)
                {
                    BreakLoop();
                }
            }
            else
            {
                _standTimer = 0f;
            }
        }

        // Called when player hits the "end" of the hallway collider
        public void OnTriggerEnterEnd()
        {
            if (!_isEscaped)
            {
                // Teleport back to start seamlessly
                // In a real game, we'd match offsets carefully
                Debug.Log("[LoopRoom] Resetting player position.");
                Ref_Player.position = Ref_StartPoint.position;
                
                // Show frustration text
                UI.UIControlManager.Instance.ShowDialogue("Don't leave me. Stay here.");
            }
        }

        private void BreakLoop()
        {
            _isEscaped = true;
            Debug.Log("[LoopRoom] Loop Broken.");
            UI.UIControlManager.Instance.ShowDialogue("Fine. If you want to leave so badly...");
            
            // Open the real door
            // DoorAnimator.SetTrigger("Open");
        }
    }
}
