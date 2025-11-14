using UnityEngine;
using TMPro;
using KaiAlphabetAdventure.Core;
using KaiAlphabetAdventure.Player;
using KaiAlphabetAdventure.Letters;

namespace KaiAlphabetAdventure.NPCs
{
    /// <summary>
    /// Friendly NPC that requires letters to complete their name.
    /// Supports: REQ-1.5.1 through REQ-1.5.8, REQ-1.5.17, REQ-1.5.18, REQ-1.3.6, REQ-1.3.7
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FriendlyNPC : MonoBehaviour, IInteractable
    {
        [Header("NPC Properties")]
        [SerializeField] private string npcName = "BEAR";
        [SerializeField] private char missingLetter = 'A';

        [Header("Display")]
        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private Color incompleteNameColor = Color.yellow;
        [SerializeField] private Color completeNameColor = Color.green;

        [Header("Movement")]
        [SerializeField] private bool canMove = true;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float moveInterval = 3f;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private Vector2 movementAreaMin = new Vector2(-10f, -10f);
        [SerializeField] private Vector2 movementAreaMax = new Vector2(10f, 10f);

        [Header("Animation")]
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private bool isNameComplete = false;
        private Vector3 targetPosition;
        private bool isMoving = false;
        private float moveTimer = 0f;
        private float intervalTimer = 0f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Ensure name is uppercase
            npcName = npcName.ToUpper();
            missingLetter = char.ToUpper(missingLetter);
        }

        private void Start()
        {
            UpdateNameDisplay();
            targetPosition = transform.position;
            intervalTimer = Random.Range(0f, moveInterval);
        }

        private void Update()
        {
            if (!isNameComplete && canMove)
            {
                HandleMovement();
            }
        }

        private void HandleMovement()
        {
            intervalTimer += Time.deltaTime;

            if (!isMoving && intervalTimer >= moveInterval)
            {
                // Start new movement
                intervalTimer = 0f;
                StartNewMovement();
            }

            if (isMoving)
            {
                moveTimer += Time.deltaTime;

                // Move towards target
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Update animation
                if (animator != null)
                {
                    animator.SetBool("IsMoving", true);
                }

                // Stop moving after duration
                if (moveTimer >= moveDuration || Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    isMoving = false;
                    moveTimer = 0f;

                    if (animator != null)
                    {
                        animator.SetBool("IsMoving", false);
                    }
                }
            }
        }

        private void StartNewMovement()
        {
            // Pick random position within movement area
            Vector3 newPosition = new Vector3(
                Random.Range(movementAreaMin.x, movementAreaMax.x),
                Random.Range(movementAreaMin.y, movementAreaMax.y),
                transform.position.z
            );

            targetPosition = newPosition;
            isMoving = true;
            moveTimer = 0f;
        }

        private void UpdateNameDisplay()
        {
            if (nameText == null) return;

            if (isNameComplete)
            {
                nameText.text = npcName;
                nameText.color = completeNameColor;
            }
            else
            {
                // Display name with missing letter as underscore
                string displayName = npcName.Replace(missingLetter, '_');
                nameText.text = displayName;
                nameText.color = incompleteNameColor;
            }
        }

        // IInteractable Implementation
        public bool CanInteract()
        {
            return !isNameComplete;
        }

        public void Interact(GameObject interactor)
        {
            if (isNameComplete) return;

            // Check if player has the missing letter
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasLettersForWord(npcName))
            {
                CompleteNPCName();
            }
            else
            {
                // Give hint
                char[] needed = InventoryManager.Instance.GetMissingLetters(npcName).ToArray();
                string needText = needed.Length > 0 ? string.Join(", ", needed) : missingLetter.ToString();
                EventManager.Instance?.TriggerShowMessage($"I need the letter {needText} to complete my name!");
            }
        }

        public string GetInteractionPrompt()
        {
            return $"Press E to help {npcName}";
        }

        private void CompleteNPCName()
        {
            // Remove letters from inventory
            if (InventoryManager.Instance != null)
            {
                foreach (char letter in npcName)
                {
                    InventoryManager.Instance.RemoveLetter(letter);
                }
            }

            isNameComplete = true;
            UpdateNameDisplay();

            // Trigger events
            EventManager.Instance?.TriggerNPCNameCompleted(npcName);
            EventManager.Instance?.TriggerWordCompleted(npcName);
            AudioManager.Instance?.PlayWordCompleteSound();

            // Show celebration message
            EventManager.Instance?.TriggerShowMessage($"Thank you! You spelled my name: {npcName}!");

            // Play celebration animation
            if (animator != null)
            {
                animator.SetTrigger("Celebrate");
            }

            Debug.Log($"Completed NPC name: {npcName}");
        }

        public void SetNPCName(string name, char missing)
        {
            npcName = name.ToUpper();
            missingLetter = char.ToUpper(missing);
            isNameComplete = false;
            UpdateNameDisplay();
        }

        private void OnDrawGizmosSelected()
        {
            if (canMove)
            {
                Gizmos.color = Color.cyan;
                Vector3 center = new Vector3((movementAreaMin.x + movementAreaMax.x) / 2f, (movementAreaMin.y + movementAreaMax.y) / 2f, 0f);
                Vector3 size = new Vector3(movementAreaMax.x - movementAreaMin.x, movementAreaMax.y - movementAreaMin.y, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
