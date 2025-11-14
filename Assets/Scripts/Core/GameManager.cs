using UnityEngine;
using UnityEngine.SceneManagement;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Central game manager handling game state and flow.
    /// Supports: REQ-1.3.1, REQ-1.3.2, REQ-1.3.4
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("Game Settings")]
        [SerializeField] private int targetWordsToComplete = 5;

        private int wordsCompleted = 0;
        private int totalLettersCollected = 0;

        public GameState CurrentState => currentState;
        public int WordsCompleted => wordsCompleted;
        public int TotalLettersCollected => totalLettersCollected;

        private void Awake()
        {
            // Singleton pattern
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
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Subscribe to events
            EventManager.Instance.OnWordCompleted += HandleWordCompleted;
            EventManager.Instance.OnLetterCollected += HandleLetterCollected;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnWordCompleted -= HandleWordCompleted;
                EventManager.Instance.OnLetterCollected -= HandleLetterCollected;
            }
        }

        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            GameState previousState = currentState;
            currentState = newState;

            // Handle state transitions
            OnStateChanged(previousState, newState);
            EventManager.Instance.TriggerGameStateChanged(previousState, newState);
        }

        private void OnStateChanged(GameState previousState, GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.Victory:
                    Time.timeScale = 0f;
                    HandleVictory();
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        }

        private void HandleWordCompleted(string word)
        {
            wordsCompleted++;
            Debug.Log($"Word completed: {word}. Total words: {wordsCompleted}");

            // Check if player has won
            if (wordsCompleted >= targetWordsToComplete)
            {
                SetGameState(GameState.Victory);
            }
        }

        private void HandleLetterCollected(char letter)
        {
            totalLettersCollected++;
        }

        private void HandleVictory()
        {
            Debug.Log("Victory! All words completed!");
            EventManager.Instance.TriggerVictory();
        }

        public void StartNewGame()
        {
            wordsCompleted = 0;
            totalLettersCollected = 0;
            SetGameState(GameState.Playing);
            SceneManager.LoadScene("GameScene"); // Load main game scene
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }

        public void ReturnToMainMenu()
        {
            SetGameState(GameState.MainMenu);
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Victory,
        GameOver
    }
}
