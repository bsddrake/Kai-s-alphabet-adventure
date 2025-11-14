# Kai's Alphabet Adventure - Project Structure

## Overview

This document describes the organization and architecture of the Kai's Alphabet Adventure Unity project.

## Folder Structure

```
Kai-s-alphabet-adventure/
├── Assets/
│   ├── Scenes/                      # Unity scene files
│   │   ├── MainMenu.unity
│   │   └── GameScene.unity
│   │
│   ├── Scripts/                     # C# scripts
│   │   ├── Core/                    # Core game systems
│   │   │   ├── GameManager.cs
│   │   │   ├── EventManager.cs
│   │   │   ├── AudioManager.cs
│   │   │   ├── InputManager.cs
│   │   │   ├── UIManager.cs
│   │   │   ├── SaveManager.cs
│   │   │   └── PlayerInputActions.cs
│   │   │
│   │   ├── Player/                  # Player character scripts
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerAnimator.cs
│   │   │   └── PlayerInteraction.cs
│   │   │
│   │   ├── Letters/                 # Letter collection system
│   │   │   ├── Letter.cs
│   │   │   ├── InventoryManager.cs
│   │   │   └── LetterSpawner.cs
│   │   │
│   │   ├── Words/                   # Word spelling system
│   │   │   ├── WordData.cs
│   │   │   └── WordManager.cs
│   │   │
│   │   ├── NPCs/                    # NPC systems
│   │   │   ├── FriendlyNPC.cs
│   │   │   └── UnfriendlyNPC.cs
│   │   │
│   │   ├── UI/                      # User interface scripts
│   │   │   ├── LetterInventoryUI.cs
│   │   │   └── SettingsMenu.cs
│   │   │
│   │   ├── Utils/                   # Utility scripts
│   │   │   ├── ObjectPool.cs
│   │   │   ├── CameraFollow.cs
│   │   │   └── ParticleEffects.cs
│   │   │
│   │   └── Data/                    # Data and constants
│   │       └── GameConstants.cs
│   │
│   ├── Prefabs/                     # Reusable game objects
│   │   ├── Player/
│   │   ├── Letters/
│   │   ├── NPCs/
│   │   ├── UI/
│   │   └── Effects/
│   │
│   ├── Sprites/                     # 2D graphics
│   │   ├── Player/
│   │   ├── Letters/
│   │   ├── NPCs/
│   │   ├── Environment/
│   │   └── UI/
│   │
│   ├── Audio/                       # Sound and music
│   │   ├── Music/
│   │   └── SFX/
│   │
│   ├── Animations/                  # Animation controllers and clips
│   │   ├── Player/
│   │   ├── NPCs/
│   │   └── UI/
│   │
│   ├── Materials/                   # Materials for sprites/effects
│   │
│   └── ScriptableObjects/          # Data assets
│       ├── Words/                   # Word challenge data
│       └── Settings/                # Game configuration
│
├── ProjectSettings/                # Unity project settings
├── REQUIREMENTS.md                 # Game requirements specification
├── TASKS.md                        # Development task breakdown
├── SETUP_GUIDE.md                  # Setup instructions
├── PROJECT_STRUCTURE.md            # This file
└── README.md                       # Project overview
```

## Script Architecture

### Core Systems (Singleton Pattern)

All core managers use the Singleton pattern for global access:

- **GameManager**: Central game state management
- **EventManager**: Event-driven communication system
- **AudioManager**: Music and sound effect management
- **InputManager**: Input handling (keyboard + controller)
- **UIManager**: UI panel and HUD management
- **SaveManager**: Save/load game state
- **InventoryManager**: Player's letter inventory
- **WordManager**: Word challenges and validation
- **LetterSpawner**: Letter spawning and pooling

### Player System

- **PlayerController**: Movement and physics
- **PlayerAnimator**: Animation state management
- **PlayerInteraction**: Interaction with objects/NPCs

### Letter System

- **Letter**: Individual collectible letter behavior
- **InventoryManager**: Stores and manages collected letters
- **LetterSpawner**: Spawns letters using object pooling

### Word System

- **WordData** (ScriptableObject): Individual word configuration
- **WordManager**: Word challenge progression and validation

### NPC System

- **FriendlyNPC**: Helpful characters requiring name completion
- **UnfriendlyNPC**: Enemies that chase and attack player

### UI System

- **LetterInventoryUI**: Displays collected letters
- **LetterSlotUI**: Individual inventory slot
- **SettingsMenu**: Audio and display settings

### Utilities

- **ObjectPool<T>**: Generic object pooling for performance
- **CameraFollow**: Smooth camera following with deadzone
- **ParticleEffects**: Visual effect management

## Design Patterns Used

### 1. Singleton Pattern
Used for manager classes that need global access:
```csharp
public static GameManager Instance { get; private set; }
```

### 2. Observer Pattern (Event System)
EventManager provides event-driven communication:
```csharp
EventManager.Instance.OnLetterCollected += HandleLetterCollected;
```

### 3. Object Pooling
LetterSpawner and ParticleEffects use pooling for performance:
```csharp
ObjectPool<Letter> letterPool;
```

### 4. ScriptableObject Pattern
WordData uses ScriptableObjects for data-driven design:
```csharp
[CreateAssetMenu(fileName = "WordData", menuName = "Kai's Adventure/Word Data")]
```

### 5. Component Pattern
GameObjects use Unity's component-based architecture

### 6. State Pattern
GameManager uses state machine for game flow:
```csharp
public enum GameState { MainMenu, Playing, Paused, Victory, GameOver }
```

## Data Flow

### Letter Collection Flow
1. Player collides with Letter
2. Letter.CollectLetter() called
3. InventoryManager.AddLetter() updates inventory
4. EventManager.TriggerLetterCollected() fires event
5. LetterInventoryUI receives event and updates display
6. AudioManager plays collection sound
7. Letter returns to pool

### Word Completion Flow
1. Player interacts with FriendlyNPC
2. FriendlyNPC checks InventoryManager.HasLettersForWord()
3. If valid, removes letters from inventory
4. Triggers EventManager.OnWordCompleted
5. WordManager.CompleteCurrentWord() processes completion
6. GameManager receives event, updates progress
7. UIManager shows completion message
8. AudioManager plays completion sound
9. WordManager.SelectNextWord() for next challenge

### Enemy Attack Flow
1. UnfriendlyNPC chases player (within chase range)
2. OnCollisionEnter2D detects player collision
3. InventoryManager.DropRandomLetters() removes letters
4. LetterSpawner spawns dropped letters in world
5. EventManager.TriggerPlayerHitByEnemy() fires event
6. AudioManager plays hit sound
7. Knockback applied to player

## Performance Considerations

### Object Pooling
- Letters (20 pre-allocated)
- Particles (10 pre-allocated)
- Sound effects (10 audio sources)

### Optimizations
- Sprite batching enabled
- Physics layers minimize collision checks
- Event-driven updates (no polling in Update())
- Coroutines for animations
- DontDestroyOnLoad for persistent managers

## Extension Points

### Adding New Word Categories
1. Add enum value to `WordCategory` in WordData.cs
2. Create new WordData ScriptableObjects
3. No code changes needed

### Adding New NPCs
1. Create prefab with appropriate NPC script
2. Configure in inspector
3. Place in scene

### Adding New UI Panels
1. Create UI in Canvas
2. Register with UIManager
3. Add show/hide methods

### Adding New Sound Effects
1. Import audio file
2. Assign to AudioManager
3. Add public method to play

### Adding New Input Actions
1. Edit PlayerInputActions asset
2. Update InputManager to expose new inputs
3. Use in gameplay scripts

## Dependencies

### Unity Packages
- Input System (com.unity.inputsystem)
- TextMeshPro (com.unity.textmeshpro)
- 2D Sprite (built-in)
- 2D Physics (built-in)

### External Assets Needed
- Player sprite sheets
- Letter sprites (A-Z, 26 total)
- NPC sprites (friendly and unfriendly)
- Environment tilesets
- UI graphics
- Background music
- Sound effects

## Requirements Traceability

Each script includes comments mapping to requirements:
```csharp
/// Supports: REQ-1.1.2, REQ-1.1.3, REQ-2.2.2, REQ-2.2.3
```

See REQUIREMENTS.md for full requirement list.
See TASKS.md for implementation tasks.

## Notes for Developers

1. **Never use FindObjectOfType in Update()** - Use cached references or event system
2. **Always null-check singletons** - Use `?.` operator for safety
3. **Use object pooling** - For frequently spawned objects
4. **Tag and Layer constants** - Defined in GameConstants.cs
5. **Event subscription** - Always unsubscribe in OnDestroy()
6. **Save compatibility** - Use [System.Serializable] for save data classes
