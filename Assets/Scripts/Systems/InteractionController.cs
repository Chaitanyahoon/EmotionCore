using UnityEngine;

namespace EmotionCore.Systems
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float InteractionDistance = 3.0f;
        public LayerMask InteractableLayer;
        public Camera PlayerCamera;

        private Interactable _currentInteractable;

        void Update()
        {
            HandleInteractionRaycast();

            // Support both 'E' key and Left Mouse Click
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) 
            {
                TryInteract();
            }
        }

        private void HandleInteractionRaycast()
        {
            if (PlayerCamera == null) return;

            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, InteractionDistance, InteractableLayer))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();

                if (interactable != null && interactable.IsInteractable)
                {
                    if (_currentInteractable != interactable)
                    {
                        if (_currentInteractable != null) _currentInteractable.OnLoseFocus();
                        _currentInteractable = interactable;
                        _currentInteractable.OnFocus();
                        
                        // Update UI Prompt (Phase 2: Use UIControlManager)
                        UI.UIControlManager.Instance?.ShowInteractionPrompt(interactable.PromptMessage);
                    }
                    return;
                }
            }

            // If we hit nothing or non-interactable
            if (_currentInteractable != null)
            {
                _currentInteractable.OnLoseFocus();
                _currentInteractable = null;
                
                // Hide UI Prompt
                UI.UIControlManager.Instance?.HideInteractionPrompt();
            }
        }

        private void TryInteract()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.OnInteract();
            }
        }
    }
}
