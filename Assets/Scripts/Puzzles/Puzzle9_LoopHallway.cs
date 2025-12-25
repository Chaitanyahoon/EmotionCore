using UnityEngine;
using EmotionCore.Core;
using EmotionCore.UI;

namespace EmotionCore.Puzzles
{
    public class Puzzle9_LoopHallway : PuzzleBase
    {
        [Header("Room Setup")]
        public Transform PlayerTransform;
        public Transform StartPoint;
        public Transform EndTriggerPoint;
        public GameObject RealDoor;
        public GameObject FakeWall;
        
        [Header("Settings")]
        public float ResetDistance = 1.0f;
        public float RequiredStillTime = 10.0f;

        private CharacterController _playerController;
        private float _currentStillTime = 0f;
        private int _loopCount = 0;

        protected override void Start()
        {
            base.Start();
            PuzzleID = "EndlessHallway";
            
            GameManager.Instance.AdvanceAct(GameManager.GameAct.Act4_Possession);
            EmotionEngine.Instance.SetEmotion(EmotionEngine.EmotionState.Possessive, 1.0f);

            if (PlayerTransform) _playerController = PlayerTransform.GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (IsSolved || PlayerTransform == null) return;

            CheckReset();
            CheckStandingStill();
        }

        private void CheckReset()
        {
            if (Vector3.Distance(PlayerTransform.position, EndTriggerPoint.position) < ResetDistance)
            {
                // Teleport back
                // Disable CC to prevent physics glitch during warp
                if (_playerController) _playerController.enabled = false;
                PlayerTransform.position = StartPoint.position;
                if (_playerController) _playerController.enabled = true;

                _loopCount++;
                HandleResetReaction();
            }
        }

        private void HandleResetReaction()
        {
            string reaction = "";
            switch(_loopCount)
            {
                case 1: reaction = "See? The world outside hurts."; break;
                case 2: reaction = "Stay where you're safe..."; break;
                case 3: reaction = "We can just walk here forever. Together."; break;
                case 4: reaction = "Why are you running?"; break;
                default: reaction = "No. You belong here."; break;
            }
            UIControlManager.Instance.ShowDialogue(reaction);
        }

        private void CheckStandingStill()
        {
            float velocity = _playerController ? _playerController.velocity.magnitude : 0f;
            
            if (velocity < 0.1f)
            {
                _currentStillTime += Time.deltaTime;
                
                if (_currentStillTime > 3.0f && _currentStillTime < 3.2f)
                {
                    UIControlManager.Instance.ShowDialogue("...why aren't you playing?");
                }

                if (_currentStillTime >= RequiredStillTime)
                {
                    BreakLoop();
                }
            }
            else
            {
                _currentStillTime = 0f;
            }
        }

        private void BreakLoop()
        {
            if (FakeWall) FakeWall.SetActive(false);
            if (RealDoor) RealDoor.SetActive(true);
            
            UIControlManager.Instance.ShowDialogue("...fine. Go.");
            Solve();
        }
    }
}
