using UnityEngine;
using TMPro;
using System.Collections.Generic;
using KaiAlphabetAdventure.Core;
using KaiAlphabetAdventure.Letters;

namespace KaiAlphabetAdventure.UI
{
    /// <summary>
    /// Displays the player's letter inventory in the HUD.
    /// Supports: REQ-1.2.4, REQ-3.1.1
    /// </summary>
    public class LetterInventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform letterSlotContainer;
        [SerializeField] private GameObject letterSlotPrefab;

        [Header("Settings")]
        [SerializeField] private int maxDisplaySlots = 26;
        [SerializeField] private bool sortAlphabetically = true;

        private List<LetterSlotUI> letterSlots = new List<LetterSlotUI>();

        private void Start()
        {
            InitializeSlots();

            // Subscribe to inventory changes
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPlayerInventoryChanged += UpdateDisplay;
                EventManager.Instance.OnLetterCollected += OnLetterCollected;
            }

            UpdateDisplay();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPlayerInventoryChanged -= UpdateDisplay;
                EventManager.Instance.OnLetterCollected -= OnLetterCollected;
            }
        }

        private void InitializeSlots()
        {
            if (letterSlotPrefab == null || letterSlotContainer == null)
            {
                Debug.LogError("LetterInventoryUI: Missing prefab or container!");
                return;
            }

            // Create initial slots (will expand as needed)
            for (int i = 0; i < 10; i++)
            {
                CreateLetterSlot();
            }
        }

        private LetterSlotUI CreateLetterSlot()
        {
            GameObject slotObj = Instantiate(letterSlotPrefab, letterSlotContainer);
            LetterSlotUI slot = slotObj.GetComponent<LetterSlotUI>();

            if (slot == null)
            {
                slot = slotObj.AddComponent<LetterSlotUI>();
            }

            letterSlots.Add(slot);
            slot.SetLetter(' ', false); // Empty initially

            return slot;
        }

        private void UpdateDisplay()
        {
            if (InventoryManager.Instance == null) return;

            List<char> letters = InventoryManager.Instance.CollectedLetters;

            if (sortAlphabetically)
            {
                letters.Sort();
            }

            // Ensure we have enough slots
            while (letterSlots.Count < letters.Count && letterSlots.Count < maxDisplaySlots)
            {
                CreateLetterSlot();
            }

            // Update slots
            for (int i = 0; i < letterSlots.Count; i++)
            {
                if (i < letters.Count)
                {
                    letterSlots[i].SetLetter(letters[i], true);
                }
                else
                {
                    letterSlots[i].SetLetter(' ', false);
                }
            }
        }

        private void OnLetterCollected(char letter)
        {
            // Find the slot for this letter and play collection animation
            UpdateDisplay();

            // Play animation on the newly filled slot
            List<char> letters = InventoryManager.Instance.CollectedLetters;
            int index = letters.LastIndexOf(letter);

            if (index >= 0 && index < letterSlots.Count)
            {
                letterSlots[index].PlayCollectionAnimation();
            }
        }
    }

    /// <summary>
    /// Individual letter slot in the inventory UI.
    /// </summary>
    public class LetterSlotUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private UnityEngine.UI.Image background;

        [Header("Colors")]
        [SerializeField] private Color filledColor = Color.white;
        [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);

        private char currentLetter;
        private bool isFilled;

        private void Awake()
        {
            if (letterText == null)
                letterText = GetComponentInChildren<TextMeshProUGUI>();

            if (background == null)
                background = GetComponent<UnityEngine.UI.Image>();
        }

        public void SetLetter(char letter, bool filled)
        {
            currentLetter = letter;
            isFilled = filled;

            if (letterText != null)
            {
                letterText.text = filled ? letter.ToString() : "";
            }

            if (background != null)
            {
                background.color = filled ? filledColor : emptyColor;
            }

            gameObject.SetActive(filled || currentLetter == ' ');
        }

        public void PlayCollectionAnimation()
        {
            if (!isFilled) return;

            StopAllCoroutines();
            StartCoroutine(CollectionAnimationCoroutine());
        }

        private System.Collections.IEnumerator CollectionAnimationCoroutine()
        {
            Vector3 originalScale = transform.localScale;
            float duration = 0.3f;
            float elapsed = 0f;

            // Scale up
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.3f, t);
                yield return null;
            }

            // Scale back down
            elapsed = 0f;
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(originalScale * 1.3f, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }
    }
}
