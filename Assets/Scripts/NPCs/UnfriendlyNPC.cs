using UnityEngine;
using KaiAlphabetAdventure.Core;
using KaiAlphabetAdventure.Letters;

namespace KaiAlphabetAdventure.NPCs
{
    /// <summary>
    /// Unfriendly NPC (monster) that chases the player and makes them drop letters.
    /// Supports: REQ-1.5.9 through REQ-1.5.16
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class UnfriendlyNPC : MonoBehaviour
    {
        [Header("Chase Behavior")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float chaseRange = 10f;
        [SerializeField] private float stopDistance = 0.5f;

        [Header("Attack Settings")]
        [SerializeField] private int lettersToDrop = 2;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float knockbackForce = 5f;

        [Header("Patrol (when not chasing)")]
        [SerializeField] private bool usePatrol = true;
        [SerializeField] private Vector2 patrolAreaMin = new Vector2(-15f, -15f);
        [SerializeField] private Vector2 patrolAreaMax = new Vector2(15f, 15f);
        [SerializeField] private float patrolWaitTime = 2f;

        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Transform playerTransform;
        private float lastAttackTime = 0f;

        private Vector3 patrolTarget;
        private bool isPatrolling = false;
        private float patrolWaitTimer = 0f;

        private bool isChasing = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            rb.gravityScale = 0f; // Top-down movement
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Start()
        {
            FindPlayer();
            SetNewPatrolTarget();
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= chaseRange)
            {
                ChasePlayer();
            }
            else if (usePatrol)
            {
                Patrol();
            }
            else
            {
                // Idle
                rb.velocity = Vector2.zero;
                UpdateAnimation(Vector2.zero);
            }
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void ChasePlayer()
        {
            isChasing = true;
            isPatrolling = false;

            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance > stopDistance)
            {
                rb.velocity = direction * moveSpeed;
                UpdateAnimation(direction);
            }
            else
            {
                rb.velocity = Vector2.zero;
                UpdateAnimation(Vector2.zero);
            }
        }

        private void Patrol()
        {
            isChasing = false;

            if (patrolWaitTimer > 0f)
            {
                patrolWaitTimer -= Time.deltaTime;
                rb.velocity = Vector2.zero;
                UpdateAnimation(Vector2.zero);
                return;
            }

            if (!isPatrolling || Vector2.Distance(transform.position, patrolTarget) < 0.5f)
            {
                SetNewPatrolTarget();
            }

            Vector2 direction = (patrolTarget - transform.position).normalized;
            rb.velocity = direction * (moveSpeed * 0.5f); // Slower patrol speed
            UpdateAnimation(direction);
        }

        private void SetNewPatrolTarget()
        {
            patrolTarget = new Vector3(
                Random.Range(patrolAreaMin.x, patrolAreaMax.x),
                Random.Range(patrolAreaMin.y, patrolAreaMax.y),
                transform.position.z
            );

            isPatrolling = true;
            patrolWaitTimer = patrolWaitTime;
        }

        private void UpdateAnimation(Vector2 velocity)
        {
            if (animator == null) return;

            bool isMoving = velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsChasing", isChasing);

            if (isMoving)
            {
                animator.SetFloat("MoveX", velocity.x);
                animator.SetFloat("MoveY", velocity.y);

                // Flip sprite based on direction
                if (spriteRenderer != null && Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                {
                    spriteRenderer.flipX = velocity.x < 0;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackPlayer(collision.gameObject);
                    lastAttackTime = Time.time;
                }
            }
        }

        private void AttackPlayer(GameObject player)
        {
            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Play sound
            AudioManager.Instance?.PlayNPCHitSound();

            // Make player drop letters
            if (InventoryManager.Instance != null)
            {
                var droppedLetters = InventoryManager.Instance.DropRandomLetters(lettersToDrop);

                // Spawn dropped letters in world
                if (LetterSpawner.Instance != null && droppedLetters.Count > 0)
                {
                    Vector3 playerPos = player.transform.position;

                    foreach (char letter in droppedLetters)
                    {
                        // Random direction away from enemy
                        Vector2 dropDirection = (player.transform.position - transform.position).normalized;
                        dropDirection += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                        dropDirection.Normalize();

                        LetterSpawner.Instance.DropLetterAtPosition(letter, playerPos, dropDirection);
                        AudioManager.Instance?.PlayLetterDropSound();
                    }

                    EventManager.Instance?.TriggerShowMessage($"Oh no! You dropped {droppedLetters.Count} letter(s)!");
                }
            }

            // Apply knockback to player
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // Trigger event
            EventManager.Instance?.TriggerPlayerHitByEnemy(lettersToDrop);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw chase range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);

            // Draw stop distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopDistance);

            // Draw patrol area
            if (usePatrol)
            {
                Gizmos.color = Color.blue;
                Vector3 center = new Vector3((patrolAreaMin.x + patrolAreaMax.x) / 2f, (patrolAreaMin.y + patrolAreaMax.y) / 2f, 0f);
                Vector3 size = new Vector3(patrolAreaMax.x - patrolAreaMin.x, patrolAreaMax.y - patrolAreaMin.y, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
