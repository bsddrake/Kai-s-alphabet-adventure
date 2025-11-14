namespace KaiAlphabetAdventure.Data
{
    /// <summary>
    /// Global game constants and configuration values.
    /// </summary>
    public static class GameConstants
    {
        // Layer Names
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_COLLECTIBLE = "Collectible";
        public const string LAYER_INTERACTABLE = "Interactable";
        public const string LAYER_ENEMY = "Enemy";
        public const string LAYER_ENVIRONMENT = "Environment";

        // Tag Names
        public const string TAG_PLAYER = "Player";
        public const string TAG_LETTER = "Letter";
        public const string TAG_FRIENDLY_NPC = "FriendlyNPC";
        public const string TAG_UNFRIENDLY_NPC = "UnfriendlyNPC";

        // Scene Names
        public const string SCENE_MAIN_MENU = "MainMenu";
        public const string SCENE_GAME = "GameScene";
        public const string SCENE_LOADING = "Loading";

        // Gameplay Constants
        public const int MAX_INVENTORY_SIZE = 50;
        public const int DEFAULT_LETTERS_TO_DROP = 2;
        public const float DEFAULT_LETTER_SPAWN_INTERVAL = 5f;
        public const int MAX_ACTIVE_LETTERS = 15;

        // UI Constants
        public const float MESSAGE_DISPLAY_DURATION = 3f;
        public const float UI_TRANSITION_DURATION = 0.3f;

        // Audio Constants
        public const float DEFAULT_MASTER_VOLUME = 1f;
        public const float DEFAULT_MUSIC_VOLUME = 0.7f;
        public const float DEFAULT_SFX_VOLUME = 1f;
        public const float AUDIO_FADE_DURATION = 1f;

        // Animation Constants
        public const float LETTER_COLLECTION_ANIM_DURATION = 0.3f;
        public const float LETTER_BOUNCE_HEIGHT = 1f;
        public const float LETTER_BOUNCE_DURATION = 0.5f;

        // Performance Constants
        public const int PARTICLE_POOL_SIZE = 10;
        public const int LETTER_POOL_SIZE = 20;
        public const int SFX_POOL_SIZE = 10;
    }
}
