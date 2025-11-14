using UnityEngine;

namespace KaiAlphabetAdventure.Player
{
    /// <summary>
    /// Handles player character animations.
    /// Supports: REQ-1.1.1, REQ-1.1.6
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Components")]
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        [Header("Animation Parameters")]
        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int MoveXParam = Animator.StringToHash("MoveX");
        private static readonly int MoveYParam = Animator.StringToHash("MoveY");
        private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
        private static readonly int InteractParam = Animator.StringToHash("Interact");
        private static readonly int PickUpParam = Animator.StringToHash("PickUp");
        private static readonly int CarryingParam = Animator.StringToHash("Carrying");

        private Vector2 lastMoveDirection = Vector2.down;
        private bool isCarrying = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        public void SetMovement(Vector2 velocity)
        {
            float speed = velocity.magnitude;
            bool isMoving = speed > 0.1f;

            // Update animator parameters
            if (animator != null)
            {
                animator.SetFloat(SpeedParam, speed);
                animator.SetBool(IsMovingParam, isMoving);

                // Update direction if moving
                if (isMoving)
                {
                    Vector2 direction = velocity.normalized;
                    animator.SetFloat(MoveXParam, direction.x);
                    animator.SetFloat(MoveYParam, direction.y);
                    lastMoveDirection = direction;

                    // Handle sprite flipping for left/right movement
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.flipX = direction.x < 0;
                        }
                    }
                }
                else
                {
                    // Use last direction when idle
                    animator.SetFloat(MoveXParam, lastMoveDirection.x);
                    animator.SetFloat(MoveYParam, lastMoveDirection.y);
                }
            }
        }

        public void PlayInteractAnimation()
        {
            animator?.SetTrigger(InteractParam);
        }

        public void PlayPickUpAnimation()
        {
            animator?.SetTrigger(PickUpParam);
        }

        public void SetCarrying(bool carrying)
        {
            isCarrying = carrying;
            animator?.SetBool(CarryingParam, carrying);
        }

        public Vector2 GetLastMoveDirection()
        {
            return lastMoveDirection;
        }
    }
}
