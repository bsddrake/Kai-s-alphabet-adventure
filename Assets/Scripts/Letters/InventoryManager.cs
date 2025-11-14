using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KaiAlphabetAdventure.Core;

namespace KaiAlphabetAdventure.Letters
{
    /// <summary>
    /// Manages the player's letter inventory.
    /// Supports: REQ-1.2.3, REQ-1.2.4
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Inventory")]
        private List<char> collectedLetters = new List<char>();

        [Header("Settings")]
        [SerializeField] private int maxInventorySize = 50;
        [SerializeField] private bool allowDuplicates = true;

        public List<char> CollectedLetters => new List<char>(collectedLetters);
        public int LetterCount => collectedLetters.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Adds a letter to the inventory.
        /// </summary>
        public bool AddLetter(char letter)
        {
            letter = char.ToUpper(letter);

            // Check if inventory is full
            if (collectedLetters.Count >= maxInventorySize)
            {
                Debug.LogWarning("Inventory is full!");
                return false;
            }

            // Check if duplicates are allowed
            if (!allowDuplicates && collectedLetters.Contains(letter))
            {
                Debug.LogWarning($"Letter {letter} already in inventory!");
                return false;
            }

            collectedLetters.Add(letter);
            EventManager.Instance?.TriggerPlayerInventoryChanged();
            Debug.Log($"Added letter {letter} to inventory. Total: {collectedLetters.Count}");

            return true;
        }

        /// <summary>
        /// Removes a specific letter from inventory.
        /// </summary>
        public bool RemoveLetter(char letter)
        {
            letter = char.ToUpper(letter);

            if (collectedLetters.Contains(letter))
            {
                collectedLetters.Remove(letter);
                EventManager.Instance?.TriggerPlayerInventoryChanged();
                Debug.Log($"Removed letter {letter} from inventory. Total: {collectedLetters.Count}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes multiple letters from inventory.
        /// </summary>
        public bool RemoveLetters(IEnumerable<char> letters)
        {
            bool allRemoved = true;
            foreach (char letter in letters)
            {
                if (!RemoveLetter(letter))
                {
                    allRemoved = false;
                }
            }
            return allRemoved;
        }

        /// <summary>
        /// Checks if the inventory contains a specific letter.
        /// </summary>
        public bool HasLetter(char letter)
        {
            letter = char.ToUpper(letter);
            return collectedLetters.Contains(letter);
        }

        /// <summary>
        /// Checks if the inventory contains all letters for a word.
        /// </summary>
        public bool HasLettersForWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            word = word.ToUpper();

            // Create a temporary list to track used letters
            List<char> tempInventory = new List<char>(collectedLetters);

            foreach (char letter in word)
            {
                if (tempInventory.Contains(letter))
                {
                    tempInventory.Remove(letter);
                }
                else
                {
                    return false; // Missing a letter
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the count of a specific letter in inventory.
        /// </summary>
        public int GetLetterCount(char letter)
        {
            letter = char.ToUpper(letter);
            return collectedLetters.Count(l => l == letter);
        }

        /// <summary>
        /// Gets letters needed to complete a word.
        /// </summary>
        public List<char> GetMissingLetters(string word)
        {
            if (string.IsNullOrEmpty(word))
                return new List<char>();

            word = word.ToUpper();
            List<char> tempInventory = new List<char>(collectedLetters);
            List<char> missing = new List<char>();

            foreach (char letter in word)
            {
                if (tempInventory.Contains(letter))
                {
                    tempInventory.Remove(letter);
                }
                else
                {
                    missing.Add(letter);
                }
            }

            return missing;
        }

        /// <summary>
        /// Clears the entire inventory.
        /// </summary>
        public void ClearInventory()
        {
            collectedLetters.Clear();
            EventManager.Instance?.TriggerPlayerInventoryChanged();
            Debug.Log("Inventory cleared.");
        }

        /// <summary>
        /// Drops random letters from inventory (when hit by enemy).
        /// Supports: REQ-1.5.14
        /// </summary>
        public List<char> DropRandomLetters(int count)
        {
            List<char> droppedLetters = new List<char>();

            count = Mathf.Min(count, collectedLetters.Count);

            for (int i = 0; i < count; i++)
            {
                if (collectedLetters.Count == 0)
                    break;

                int randomIndex = Random.Range(0, collectedLetters.Count);
                char droppedLetter = collectedLetters[randomIndex];
                collectedLetters.RemoveAt(randomIndex);
                droppedLetters.Add(droppedLetter);

                EventManager.Instance?.TriggerLetterDropped(droppedLetter);
            }

            if (droppedLetters.Count > 0)
            {
                EventManager.Instance?.TriggerPlayerInventoryChanged();
                Debug.Log($"Dropped {droppedLetters.Count} letters from inventory.");
            }

            return droppedLetters;
        }

        /// <summary>
        /// Gets inventory data for saving.
        /// </summary>
        public InventorySaveData GetSaveData()
        {
            return new InventorySaveData
            {
                letters = new List<char>(collectedLetters)
            };
        }

        /// <summary>
        /// Loads inventory from save data.
        /// </summary>
        public void LoadSaveData(InventorySaveData data)
        {
            collectedLetters = new List<char>(data.letters);
            EventManager.Instance?.TriggerPlayerInventoryChanged();
        }
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<char> letters;
    }
}
