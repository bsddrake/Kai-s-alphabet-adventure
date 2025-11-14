using UnityEngine;
using KaiAlphabetAdventure.Core;
using KaiAlphabetAdventure.Player;

namespace KaiAlphabetAdventure.Letters
{
    /// <summary>
    /// Represents a collectible letter in the game world.
    /// Supports: REQ-1.2.1, REQ-1.2.2, REQ-1.2.5, REQ-1.2.7
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Letter : MonoBehaviour, IInteractable
    {
        [Header("Letter Properties")]
        [SerializeField] private char letterCharacter = 'A';
        [SerializeField] private bool collectOnContact = true;
        [SerializeField] private bool requireInteraction = false;

        [Header("Visual Effects")]
        [SerializeField] private GameObject collectParticles;
        [SerializeField] private float floatAmplitude = 0.2f;
        [SerializeField] private float floatSpeed = 2f;
        [SerializeField] private bool enableFloating = true;

        [Header("Bounce Settings")]
        [SerializeField] private float bounceHeight = 1f;
        [SerializeField] private float bounceSpeed = 10f;
        [SerializeField] private float bounceDuration = 0.5f;

        private SpriteRenderer spriteRenderer;
        private Collider2D letterCollider;
        private Vector3 startPosition;
        private bool isCollected = false;
        private bool isBouncing = false;
        private float bounceTimer = 0f;

        public char Character => letterCharacter;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            letterCollider = GetComponent<Collider2D>();
            letterCollider.isTrigger = true;
        }

        private void Start()
        {
            startPosition = transform.position;

            // Play bounce animation when spawned (REQ-1.2.7)
            if (bounceHeight > 0)
            {
                StartBounce();
            }
        }

        private void Update()
        {
            if (isCollected) return;

            if (isBouncing)
            {
                UpdateBounce();
            }
            else if (enableFloating)
            {
                UpdateFloating();
            }
        }

        private void UpdateFloating()
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void StartBounce()
        {
            isBouncing = true;
            bounceTimer = 0f;
        }

        private void UpdateBounce()
        {
            bounceTimer += Time.deltaTime;

            if (bounceTimer < bounceDuration)
            {
                // Parabolic bounce
                float t = bounceTimer / bounceDuration;
                float height = bounceHeight * (1 - (t * 2 - 1) * (t * 2 - 1)); // Parabola
                transform.position = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);
            }
            else
            {
                isBouncing = false;
                transform.position = startPosition;
            }
        }

        public void SetLetter(char letter)
        {
            letterCharacter = char.ToUpper(letter);
            gameObject.name = $"Letter_{letterCharacter}";
        }

        public void DropAtPosition(Vector3 position, Vector2 dropDirection)
        {
            transform.position = position;
            startPosition = position + (Vector3)(dropDirection * 2f); // Bounce away
            StartBounce();

            // After bounce, update start position
            Invoke(nameof(UpdateStartPosition), bounceDuration);
        }

        private void UpdateStartPosition()
        {
            startPosition = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected) return;

            if (collectOnContact && other.CompareTag("Player"))
            {
                CollectLetter(other.gameObject);
            }
        }

        // IInteractable Implementation
        public bool CanInteract()
        {
            return !isCollected && requireInteraction;
        }

        public void Interact(GameObject interactor)
        {
            if (!isCollected && requireInteraction)
            {
                CollectLetter(interactor);
            }
        }

        public string GetInteractionPrompt()
        {
            return $"Press E to collect {letterCharacter}";
        }

        private void CollectLetter(GameObject collector)
        {
            if (isCollected) return;

            isCollected = true;

            // Add to inventory
            InventoryManager.Instance?.AddLetter(letterCharacter);

            // Trigger events
            EventManager.Instance?.TriggerLetterCollected(letterCharacter);

            // Play collection effects
            PlayCollectionEffects();

            // Play sound
            AudioManager.Instance?.PlayLetterCollectSound();

            // Return to pool or destroy
            LetterSpawner.Instance?.ReturnLetterToPool(this);
        }

        private void PlayCollectionEffects()
        {
            // Spawn particles
            if (collectParticles != null)
            {
                Instantiate(collectParticles, transform.position, Quaternion.identity);
            }

            // Play animation (scale up and fade out)
            StartCoroutine(CollectionAnimation());
        }

        private System.Collections.IEnumerator CollectionAnimation()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = startScale * 1.5f;
            Color startColor = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                Color newColor = startColor;
                newColor.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = newColor;

                yield return null;
            }

            gameObject.SetActive(false);
        }

        public void ResetLetter()
        {
            isCollected = false;
            isBouncing = false;
            transform.localScale = Vector3.one;
            spriteRenderer.color = Color.white;
            gameObject.SetActive(true);
        }
    }
}
