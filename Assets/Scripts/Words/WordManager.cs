using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KaiAlphabetAdventure.Core;
using KaiAlphabetAdventure.Letters;

namespace KaiAlphabetAdventure.Words
{
    /// <summary>
    /// Manages word challenges and validation.
    /// Supports: REQ-1.3.1, REQ-1.3.2, REQ-1.3.3, REQ-1.3.4, REQ-1.3.5
    /// </summary>
    public class WordManager : MonoBehaviour
    {
        public static WordManager Instance { get; private set; }

        [Header("Word Database")]
        [SerializeField] private List<WordData> allWords = new List<WordData>();
        [SerializeField] private bool loadWordsFromResources = true;

        [Header("Progression")]
        [SerializeField] private bool progressiveDifficulty = true;
        [SerializeField] private int wordsPerDifficulty = 3;

        private WordData currentWord;
        private List<WordData> completedWords = new List<WordData>();
        private List<WordData> availableWords = new List<WordData>();
        private int currentDifficultyLevel = 0;

        public WordData CurrentWord => currentWord;
        public int CompletedWordCount => completedWords.Count;
        public List<string> CompletedWordStrings => completedWords.Select(w => w.Word).ToList();

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

        private void Start()
        {
            LoadWordDatabase();
            InitializeWordList();
            SelectNextWord();
        }

        private void LoadWordDatabase()
        {
            if (loadWordsFromResources)
            {
                WordData[] loadedWords = Resources.LoadAll<WordData>("Words");
                if (loadedWords.Length > 0)
                {
                    allWords.AddRange(loadedWords);
                    Debug.Log($"Loaded {loadedWords.Length} words from Resources.");
                }
            }

            // If no words loaded, create some defaults
            if (allWords.Count == 0)
            {
                Debug.LogWarning("No words found! Creating default word list.");
                CreateDefaultWords();
            }
        }

        private void CreateDefaultWords()
        {
            // This method creates runtime word data as fallback
            // In a real project, these would be ScriptableObject assets
            string[] easyWords = { "CAT", "DOG", "SUN", "BAT", "HAT", "BEE", "FOX" };
            string[] mediumWords = { "BEAR", "BIRD", "FROG", "STAR", "TREE", "MOON" };
            string[] hardWords = { "TIGER", "SNAKE", "FLOWER", "RABBIT" };

            foreach (string word in easyWords)
            {
                WordData data = ScriptableObject.CreateInstance<WordData>();
                data.word = word;
                data.difficulty = WordDifficulty.Easy;
                data.category = WordCategory.Animals;
                allWords.Add(data);
            }

            foreach (string word in mediumWords)
            {
                WordData data = ScriptableObject.CreateInstance<WordData>();
                data.word = word;
                data.difficulty = WordDifficulty.Medium;
                data.category = WordCategory.Animals;
                allWords.Add(data);
            }

            foreach (string word in hardWords)
            {
                WordData data = ScriptableObject.CreateInstance<WordData>();
                data.word = word;
                data.difficulty = WordDifficulty.Hard;
                data.category = WordCategory.Animals;
                allWords.Add(data);
            }
        }

        private void InitializeWordList()
        {
            availableWords = new List<WordData>(allWords);

            if (progressiveDifficulty)
            {
                // Start with easy words only
                availableWords = availableWords
                    .Where(w => w.difficulty == WordDifficulty.Easy)
                    .ToList();
            }
        }

        public void SelectNextWord()
        {
            if (availableWords.Count == 0)
            {
                Debug.Log("No more words available!");
                EventManager.Instance?.TriggerVictory();
                return;
            }

            // Select random word from available
            currentWord = availableWords[Random.Range(0, availableWords.Count)];
            EventManager.Instance?.TriggerWordStarted(currentWord.Word);

            // Update UI
            UIManager.Instance?.UpdateCurrentWord(currentWord.Word);

            Debug.Log($"New word challenge: {currentWord.Word} (Difficulty: {currentWord.difficulty})");
        }

        public bool ValidateWord()
        {
            if (currentWord == null) return false;

            return ValidateWord(currentWord.Word);
        }

        public bool ValidateWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;

            // Check if player has the required letters
            return InventoryManager.Instance?.HasLettersForWord(word) ?? false;
        }

        public void CompleteCurrentWord()
        {
            if (currentWord == null) return;

            // Remove used letters from inventory
            if (InventoryManager.Instance != null)
            {
                foreach (char letter in currentWord.Word)
                {
                    InventoryManager.Instance.RemoveLetter(letter);
                }
            }

            // Mark as completed
            completedWords.Add(currentWord);
            availableWords.Remove(currentWord);

            // Trigger events
            EventManager.Instance?.TriggerWordCompleted(currentWord.Word);
            AudioManager.Instance?.PlayWordCompleteSound();

            // Show feedback
            EventManager.Instance?.TriggerShowMessage($"Great job! You spelled {currentWord.Word}!");

            // Update UI
            UIManager.Instance?.UpdateWordsCompleted(completedWords.Count, allWords.Count);

            Debug.Log($"Completed word: {currentWord.Word}. Total completed: {completedWords.Count}");

            // Check for difficulty progression
            if (progressiveDifficulty)
            {
                CheckDifficultyProgression();
            }

            // Select next word
            SelectNextWord();
        }

        private void CheckDifficultyProgression()
        {
            int completedInCurrentDifficulty = completedWords.Count(w => (int)w.difficulty == currentDifficultyLevel);

            if (completedInCurrentDifficulty >= wordsPerDifficulty)
            {
                // Advance to next difficulty
                currentDifficultyLevel++;

                if (currentDifficultyLevel <= 2) // 0=Easy, 1=Medium, 2=Hard
                {
                    WordDifficulty nextDifficulty = (WordDifficulty)currentDifficultyLevel;

                    // Add words of next difficulty to available pool
                    List<WordData> nextWords = allWords
                        .Where(w => w.difficulty == nextDifficulty && !completedWords.Contains(w))
                        .ToList();

                    availableWords.AddRange(nextWords);

                    Debug.Log($"Difficulty increased to: {nextDifficulty}");
                    EventManager.Instance?.TriggerShowMessage($"Difficulty increased! Get ready for harder words!");
                }
            }
        }

        public List<char> GetMissingLetters()
        {
            if (currentWord == null) return new List<char>();

            return InventoryManager.Instance?.GetMissingLetters(currentWord.Word) ?? new List<char>();
        }

        public WordData GetWordByString(string word)
        {
            word = word.ToUpper();
            return allWords.FirstOrDefault(w => w.Word == word);
        }

        public WordSaveData GetSaveData()
        {
            return new WordSaveData
            {
                currentWordIndex = allWords.IndexOf(currentWord),
                completedWordIndices = completedWords.Select(w => allWords.IndexOf(w)).ToList(),
                currentDifficultyLevel = currentDifficultyLevel
            };
        }

        public void LoadSaveData(WordSaveData data)
        {
            completedWords.Clear();
            foreach (int index in data.completedWordIndices)
            {
                if (index >= 0 && index < allWords.Count)
                {
                    completedWords.Add(allWords[index]);
                }
            }

            currentDifficultyLevel = data.currentDifficultyLevel;

            if (data.currentWordIndex >= 0 && data.currentWordIndex < allWords.Count)
            {
                currentWord = allWords[data.currentWordIndex];
            }

            InitializeWordList();
        }
    }

    [System.Serializable]
    public class WordSaveData
    {
        public int currentWordIndex;
        public List<int> completedWordIndices;
        public int currentDifficultyLevel;
    }
}
