using UnityEngine;

namespace KaiAlphabetAdventure.Words
{
    /// <summary>
    /// ScriptableObject containing word data for challenges.
    /// Supports: REQ-1.3.1, REQ-5.2.1, REQ-5.2.2, REQ-5.2.3
    /// </summary>
    [CreateAssetMenu(fileName = "WordData", menuName = "Kai's Adventure/Word Data")]
    public class WordData : ScriptableObject
    {
        [Header("Word Properties")]
        [Tooltip("The word to spell (will be converted to uppercase)")]
        public string word;

        [Tooltip("Difficulty level of the word")]
        public WordDifficulty difficulty = WordDifficulty.Easy;

        [Tooltip("Category/theme of the word")]
        public WordCategory category = WordCategory.Animals;

        [Tooltip("Optional hint for the player")]
        [TextArea(2, 4)]
        public string hint;

        [Header("Rewards")]
        [Tooltip("Points awarded for completing this word")]
        public int pointValue = 10;

        public string Word => word.ToUpper();
        public int Length => word.Length;

        private void OnValidate()
        {
            // Ensure word is uppercase
            if (!string.IsNullOrEmpty(word))
            {
                word = word.ToUpper();
            }
        }
    }

    public enum WordDifficulty
    {
        Easy,       // 3-letter words
        Medium,     // 4-5 letter words
        Hard        // 6+ letter words
    }

    public enum WordCategory
    {
        Animals,
        Colors,
        Objects,
        Nature,
        Food,
        Toys,
        Family,
        Actions
    }
}
