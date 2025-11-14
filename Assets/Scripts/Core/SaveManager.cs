using UnityEngine;
using System.IO;
using KaiAlphabetAdventure.Letters;
using KaiAlphabetAdventure.Words;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Manages game save and load functionality.
    /// Supports: REQ-6.2.3
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Save Settings")]
        [SerializeField] private bool useAutoSave = true;
        [SerializeField] private float autoSaveInterval = 60f; // Save every 60 seconds

        private string saveFilePath;
        private float autoSaveTimer = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }

        private void Update()
        {
            if (useAutoSave && GameManager.Instance?.CurrentState == GameState.Playing)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    autoSaveTimer = 0f;
                    SaveGame();
                }
            }
        }

        public void SaveGame()
        {
            GameSaveData saveData = new GameSaveData
            {
                // Game progress
                wordsCompleted = GameManager.Instance?.WordsCompleted ?? 0,
                totalLettersCollected = GameManager.Instance?.TotalLettersCollected ?? 0,

                // Inventory
                inventory = InventoryManager.Instance?.GetSaveData(),

                // Word progress
                wordProgress = WordManager.Instance?.GetSaveData(),

                // Timestamp
                saveTime = System.DateTime.Now.ToString(),
                playTime = Time.timeSinceLevelLoad
            };

            try
            {
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"Game saved to: {saveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public bool LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.Log("No save file found.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                // Load inventory
                if (saveData.inventory != null && InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.LoadSaveData(saveData.inventory);
                }

                // Load word progress
                if (saveData.wordProgress != null && WordManager.Instance != null)
                {
                    WordManager.Instance.LoadSaveData(saveData.wordProgress);
                }

                Debug.Log($"Game loaded from: {saveFilePath} (Saved: {saveData.saveTime})");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return false;
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted.");
            }
        }

        public bool HasSaveFile()
        {
            return File.Exists(saveFilePath);
        }

        public string GetSaveFilePath()
        {
            return saveFilePath;
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public int wordsCompleted;
        public int totalLettersCollected;
        public InventorySaveData inventory;
        public WordSaveData wordProgress;
        public string saveTime;
        public float playTime;
    }
}
