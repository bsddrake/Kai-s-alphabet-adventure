using UnityEngine;
using System.Collections.Generic;
using KaiAlphabetAdventure.Core;

namespace KaiAlphabetAdventure.Player
{
    /// <summary>
    /// Handles player interaction with objects and NPCs.
    /// Supports: REQ-1.1.4, REQ-2.2.2, REQ-2.2.3
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Detection")]
        private List<IInteractable> nearbyInteractables = new List<IInteractable>();
        private IInteractable currentInteractable;
        private PlayerAnimator playerAnimator;

        private void Awake()
        {
            playerAnimator = GetComponent<PlayerAnimator>();
        }

        private void Update()
        {
            if (GameManager.Instance?.CurrentState != GameState.Playing)
                return;

            DetectNearbyInteractables();
            HandleInteractionInput();
        }

        private void DetectNearbyInteractables()
        {
            nearbyInteractables.Clear();
            currentInteractable = null;

            // Find all interactables in range
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

            float closestDistance = float.MaxValue;
            IInteractable closest = null;

            foreach (Collider2D col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract())
                {
                    nearbyInteractables.Add(interactable);

                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = interactable;
                    }
                }
            }

            currentInteractable = closest;

            // Update interaction prompt
            if (currentInteractable != null)
            {
                EventManager.Instance?.TriggerShowInteractionPrompt(true);
            }
            else
            {
                EventManager.Instance?.TriggerShowInteractionPrompt(false);
            }
        }

        private void HandleInteractionInput()
        {
            if (InputManager.Instance != null && InputManager.Instance.InteractPressed)
            {
                if (currentInteractable != null)
                {
                    Interact(currentInteractable);
                }
            }
        }

        private void Interact(IInteractable interactable)
        {
            if (interactable.CanInteract())
            {
                playerAnimator?.PlayInteractAnimation();
                interactable.Interact(gameObject);
                AudioManager.Instance?.PlayInteractionSound();
            }
        }

        public void ForceInteractWith(IInteractable interactable)
        {
            if (interactable != null && interactable.CanInteract())
            {
                Interact(interactable);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }

    /// <summary>
    /// Interface for interactable objects.
    /// </summary>
    public interface IInteractable
    {
        bool CanInteract();
        void Interact(GameObject interactor);
        string GetInteractionPrompt();
    }
}
