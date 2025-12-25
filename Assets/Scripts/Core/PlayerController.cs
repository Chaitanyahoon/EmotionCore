using UnityEngine;

namespace EmotionCore.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float WalkSpeed = 5.0f;
        public float RunSpeed = 8.0f;
        public float JumpHeight = 1.0f;
        public float Gravity = -9.81f;

        [Header("Look Settings")]
        public Transform CameraTransform;
        public float MouseSensitivity = 2.0f;
        public float LookXLimit = 85.0f;
        
        [Header("Head Bob")]
        public float BobFrequency = 5f;
        public float BobAmplitude = 0.1f;
        private float _defaultYPos = 0;
        private float _timer = 0;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0;

        private bool canMove = true;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            if(CameraTransform) _defaultYPos = CameraTransform.localPosition.y;

            // Lock Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            // We are Grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? RunSpeed : WalkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? RunSpeed : WalkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = CheckJumpHeight();
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y += Gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);
            
            // Head Bob
            if (canMove && (curSpeedX != 0 || curSpeedY != 0) && characterController.isGrounded)
            {
                _timer += Time.deltaTime * (isRunning ? RunSpeed : WalkSpeed);
                if(CameraTransform)
                {
                    CameraTransform.localPosition = new Vector3(
                        CameraTransform.localPosition.x,
                        _defaultYPos + Mathf.Sin(_timer * BobFrequency) * BobAmplitude,
                        CameraTransform.localPosition.z
                    );
                }
            }
            else
            {
                _timer = 0;
                if(CameraTransform)
                {
                    CameraTransform.localPosition = new Vector3(
                        CameraTransform.localPosition.x,
                        Mathf.Lerp(CameraTransform.localPosition.y, _defaultYPos, Time.deltaTime * BobFrequency),
                        CameraTransform.localPosition.z
                    );
                }
            }

            // Camera Rotation
            if (canMove && CameraTransform != null)
            {
                rotationX += -Input.GetAxis("Mouse Y") * MouseSensitivity;
                rotationX = Mathf.Clamp(rotationX, -LookXLimit, LookXLimit);
                CameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * MouseSensitivity, 0);
            }
        }

        private float CheckJumpHeight()
        {
             return Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        public void SetControl(bool state)
        {
            canMove = state;
        }
    }
}
