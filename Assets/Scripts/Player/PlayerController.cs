using UnityEngine;
using KaiAlphabetAdventure.Core;

namespace KaiAlphabetAdventure.Player
{
    /// <summary>
    /// Main player controller handling movement and basic physics.
    /// Supports: REQ-1.1.2, REQ-1.1.3, REQ-2.2.2, REQ-2.2.3
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("Boundaries")]
        [SerializeField] private bool useBoundaries = true;
        [SerializeField] private Vector2 minBoundary = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 maxBoundary = new Vector2(50f, 50f);

        [Header("Components")]
        private Rigidbody2D rb;
        private PlayerAnimator playerAnimator;
        private PlayerInteraction playerInteraction;

        private Vector2 currentVelocity;
        private Vector2 moveInput;
        private bool canMove = true;

        public bool CanMove
        {
            get => canMove;
            set => canMove = value;
        }

        public Vector2 MoveDirection => moveInput;
        public bool IsMoving => currentVelocity.magnitude > 0.1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerAnimator = GetComponent<PlayerAnimator>();
            playerInteraction = GetComponent<PlayerInteraction>();

            // Configure Rigidbody2D
            rb.gravityScale = 0f; // Top-down movement
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Update()
        {
            if (canMove && GameManager.Instance?.CurrentState == GameState.Playing)
            {
                GetInput();
                HandlePause();
            }
        }

        private void FixedUpdate()
        {
            if (canMove && GameManager.Instance?.CurrentState == GameState.Playing)
            {
                Move();
                ApplyBoundaries();
            }
            else
            {
                // Decelerate when not able to move
                currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
                rb.velocity = currentVelocity;
            }
        }

        private void GetInput()
        {
            if (InputManager.Instance != null)
            {
                moveInput = InputManager.Instance.MoveInput;
            }
        }

        private void HandlePause()
        {
            if (InputManager.Instance != null && InputManager.Instance.PausePressed)
            {
                GameManager.Instance?.PauseGame();
            }
        }

        private void Move()
        {
            // Calculate target velocity
            Vector2 targetVelocity = moveInput.normalized * moveSpeed;

            // Smoothly interpolate to target velocity
            if (moveInput.magnitude > 0.1f)
            {
                currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }
            else
            {
                currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            }

            // Apply velocity
            rb.velocity = currentVelocity;

            // Update animator
            if (playerAnimator != null)
            {
                playerAnimator.SetMovement(currentVelocity);
            }
        }

        private void ApplyBoundaries()
        {
            if (!useBoundaries) return;

            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, minBoundary.x, maxBoundary.x);
            position.y = Mathf.Clamp(position.y, minBoundary.y, maxBoundary.y);
            transform.position = position;
        }

        public void SetBoundaries(Vector2 min, Vector2 max)
        {
            minBoundary = min;
            maxBoundary = max;
            useBoundaries = true;
        }

        public void DisableBoundaries()
        {
            useBoundaries = false;
        }

        public void Teleport(Vector3 position)
        {
            transform.position = position;
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if (useBoundaries)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((minBoundary.x + maxBoundary.x) / 2f, (minBoundary.y + maxBoundary.y) / 2f, 0f);
                Vector3 size = new Vector3(maxBoundary.x - minBoundary.x, maxBoundary.y - minBoundary.y, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
