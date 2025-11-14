using UnityEngine;
using TMPro;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Manages all UI panels and HUD elements.
    /// Supports: REQ-3.1.1, REQ-3.2.1, REQ-3.2.2, REQ-3.3.3
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD References")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject inventoryDisplay;
        [SerializeField] private TextMeshProUGUI currentWordText;
        [SerializeField] private TextMeshProUGUI wordsCompletedText;
        [SerializeField] private GameObject interactionPrompt;
        [SerializeField] private TextMeshProUGUI interactionPromptText;

        [Header("Menu References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject settingsMenuPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("Feedback")]
        [SerializeField] private GameObject messagePanel;
        [SerializeField] private TextMeshProUGUI messageText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            // Subscribe to events
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                EventManager.Instance.OnShowMessage += ShowMessage;
                EventManager.Instance.OnShowInteractionPrompt += ShowInteractionPrompt;
            }

            InitializeUI();
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
                EventManager.Instance.OnShowMessage -= ShowMessage;
                EventManager.Instance.OnShowInteractionPrompt -= ShowInteractionPrompt;
            }
        }

        private void InitializeUI()
        {
            HideAllPanels();
        }

        private void HideAllPanels()
        {
            if (hudPanel != null) hudPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (messagePanel != null) messagePanel.SetActive(false);
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }

        private void HandleGameStateChanged(GameState previousState, GameState newState)
        {
            HideAllPanels();

            switch (newState)
            {
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;

                case GameState.Playing:
                    ShowHUD();
                    break;

                case GameState.Paused:
                    ShowPauseMenu();
                    break;

                case GameState.Victory:
                    ShowVictoryScreen();
                    break;
            }
        }

        // Panel Management
        public void ShowHUD()
        {
            if (hudPanel != null) hudPanel.SetActive(true);
        }

        public void HideHUD()
        {
            if (hudPanel != null) hudPanel.SetActive(false);
        }

        public void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void HideMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        }

        public void ShowPauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
                if (hudPanel != null) hudPanel.SetActive(true); // Keep HUD visible in background
            }
        }

        public void HidePauseMenu()
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }

        public void ShowSettingsMenu()
        {
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        }

        public void HideSettingsMenu()
        {
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        }

        public void ShowVictoryScreen()
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }

        // HUD Updates
        public void UpdateCurrentWord(string word)
        {
            if (currentWordText != null)
            {
                currentWordText.text = $"Current Word: {word}";
            }
        }

        public void UpdateWordsCompleted(int completed, int total)
        {
            if (wordsCompletedText != null)
            {
                wordsCompletedText.text = $"Words: {completed}/{total}";
            }
        }

        // Interaction Prompt
        public void ShowInteractionPrompt(bool show, string promptText = "Press E to interact")
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(show);
                if (show && interactionPromptText != null)
                {
                    interactionPromptText.text = promptText;
                }
            }
        }

        // Message Display
        public void ShowMessage(string message, float duration = 3f)
        {
            if (messagePanel != null && messageText != null)
            {
                messageText.text = message;
                messagePanel.SetActive(true);
                CancelInvoke(nameof(HideMessage));
                Invoke(nameof(HideMessage), duration);
            }
        }

        private void HideMessage()
        {
            if (messagePanel != null)
            {
                messagePanel.SetActive(false);
            }
        }

        // Button Handlers
        public void OnPlayButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            GameManager.Instance?.StartNewGame();
        }

        public void OnResumeButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            GameManager.Instance?.ResumeGame();
        }

        public void OnSettingsButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            ShowSettingsMenu();
        }

        public void OnBackButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            HideSettingsMenu();
        }

        public void OnMainMenuButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            GameManager.Instance?.ReturnToMainMenu();
        }

        public void OnQuitButtonClicked()
        {
            AudioManager.Instance?.PlayButtonClickSound();
            GameManager.Instance?.QuitGame();
        }
    }
}
