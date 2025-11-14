using System;
using UnityEngine;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Central event system for game-wide communication.
    /// Supports: TASK-1.3.5
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        // Letter Collection Events
        public event Action<char> OnLetterCollected;
        public event Action<char> OnLetterDropped;

        // Word Events
        public event Action<string> OnWordCompleted;
        public event Action<string> OnWordStarted;

        // NPC Events
        public event Action<string> OnNPCInteracted;
        public event Action<string> OnNPCNameCompleted;

        // Player Events
        public event Action<int> OnPlayerHitByEnemy;
        public event Action OnPlayerInventoryChanged;

        // Game State Events
        public event Action<GameState, GameState> OnGameStateChanged;
        public event Action OnVictory;

        // UI Events
        public event Action<string> OnShowMessage;
        public event Action<bool> OnShowInteractionPrompt;

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

        // Letter Collection
        public void TriggerLetterCollected(char letter)
        {
            OnLetterCollected?.Invoke(letter);
            OnPlayerInventoryChanged?.Invoke();
        }

        public void TriggerLetterDropped(char letter)
        {
            OnLetterDropped?.Invoke(letter);
            OnPlayerInventoryChanged?.Invoke();
        }

        // Words
        public void TriggerWordCompleted(string word)
        {
            OnWordCompleted?.Invoke(word);
        }

        public void TriggerWordStarted(string word)
        {
            OnWordStarted?.Invoke(word);
        }

        // NPCs
        public void TriggerNPCInteracted(string npcName)
        {
            OnNPCInteracted?.Invoke(npcName);
        }

        public void TriggerNPCNameCompleted(string npcName)
        {
            OnNPCNameCompleted?.Invoke(npcName);
        }

        // Player
        public void TriggerPlayerHitByEnemy(int damageAmount)
        {
            OnPlayerHitByEnemy?.Invoke(damageAmount);
        }

        // Game State
        public void TriggerGameStateChanged(GameState previousState, GameState newState)
        {
            OnGameStateChanged?.Invoke(previousState, newState);
        }

        public void TriggerVictory()
        {
            OnVictory?.Invoke();
        }

        // UI
        public void TriggerShowMessage(string message)
        {
            OnShowMessage?.Invoke(message);
        }

        public void TriggerShowInteractionPrompt(bool show)
        {
            OnShowInteractionPrompt?.Invoke(show);
        }
    }
}
