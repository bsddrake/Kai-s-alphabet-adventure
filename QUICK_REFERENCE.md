# Kai's Alphabet Adventure - Quick Reference

## 🎮 Controls

### Keyboard
- **WASD / Arrow Keys**: Move player
- **E / Space**: Interact with objects/NPCs
- **ESC**: Pause game

### Controller
- **Left Stick**: Move player
- **Button South (A/Cross)**: Interact
- **Start**: Pause game

## 📝 Key Script Components

### Core Managers (Singletons)

| Manager | Purpose | Key Methods |
|---------|---------|-------------|
| `GameManager` | Game state and flow | `StartNewGame()`, `PauseGame()`, `SetGameState()` |
| `EventManager` | Event communication | `TriggerLetterCollected()`, `TriggerWordCompleted()` |
| `AudioManager` | Sound and music | `PlayLetterCollectSound()`, `PlayGameplayMusic()` |
| `InputManager` | Input handling | `MoveInput`, `InteractPressed` properties |
| `UIManager` | UI panel management | `ShowHUD()`, `UpdateCurrentWord()` |
| `SaveManager` | Save/load game | `SaveGame()`, `LoadGame()` |
| `InventoryManager` | Letter inventory | `AddLetter()`, `HasLettersForWord()` |
| `WordManager` | Word challenges | `ValidateWord()`, `CompleteCurrentWord()` |
| `LetterSpawner` | Spawn letters | `SpawnLetter()`, `StartSpawning()` |

### Player Scripts

| Script | Purpose | Attach To |
|--------|---------|-----------|
| `PlayerController` | Movement and physics | Player GameObject |
| `PlayerAnimator` | Animation control | Player GameObject |
| `PlayerInteraction` | Interact with objects | Player GameObject |

### Collectible Scripts

| Script | Purpose | Attach To |
|--------|---------|-----------|
| `Letter` | Letter collectible | Letter prefab |
| `FriendlyNPC` | Helpful NPC | Friendly NPC prefab |
| `UnfriendlyNPC` | Enemy NPC | Enemy prefab |

### UI Scripts

| Script | Purpose | Attach To |
|--------|---------|-----------|
| `LetterInventoryUI` | Display inventory | Canvas/HUD |
| `SettingsMenu` | Settings panel | Settings UI panel |

### Utility Scripts

| Script | Purpose | Use Case |
|--------|---------|----------|
| `ObjectPool<T>` | Object pooling | Letters, particles, SFX |
| `CameraFollow` | Camera control | Main Camera |
| `ParticleEffects` | VFX management | Effects GameObject |

## 🏷️ Tags and Layers

### Tags
- `Player` - Player character
- `Letter` - Collectible letters
- `FriendlyNPC` - Helpful NPCs
- `UnfriendlyNPC` - Enemy NPCs

### Layers
- Layer 6: `Player`
- Layer 7: `Collectible`
- Layer 8: `Interactable`
- Layer 9: `Enemy`
- Layer 10: `Environment`

## 📋 Common Tasks

### Add a New Word
1. Right-click in `Assets/ScriptableObjects/Words/`
2. Create → Kai's Adventure → Word Data
3. Set word, difficulty, category, hint
4. Place in `Resources/Words/` or add to WordManager list

### Add a New Letter Sprite
1. Import sprite to `Assets/Sprites/Letters/`
2. Name it `Letter_X.png` (where X is the letter)
3. Assign to LetterSpawner's letterSprites array
4. Index: A=0, B=1, ..., Z=25

### Add a New Sound Effect
1. Import audio to `Assets/Audio/SFX/`
2. Assign to AudioManager in Inspector
3. Call `AudioManager.Instance.PlayXXXSound()` in code

### Add Background Music
1. Import to `Assets/Audio/Music/`
2. Assign to AudioManager's music clips
3. Call `AudioManager.Instance.PlayGameplayMusic()` etc.

### Create a New Friendly NPC
1. Create GameObject with SpriteRenderer
2. Add CircleCollider2D (Is Trigger: true)
3. Add FriendlyNPC script
4. Set NPC Name (e.g., "BEAR")
5. Set Missing Letter
6. Add TextMeshPro child for name display
7. Tag as `FriendlyNPC`, Layer `Interactable`

### Create a New Enemy NPC
1. Create GameObject with SpriteRenderer
2. Add Rigidbody2D (Gravity: 0)
3. Add CircleCollider2D
4. Add UnfriendlyNPC script
5. Configure chase range and behavior
6. Tag as `UnfriendlyNPC`, Layer `Enemy`

## 🎯 Events You Can Subscribe To

```csharp
// In Start()
EventManager.Instance.OnLetterCollected += HandleLetterCollected;
EventManager.Instance.OnLetterDropped += HandleLetterDropped;
EventManager.Instance.OnWordCompleted += HandleWordCompleted;
EventManager.Instance.OnWordStarted += HandleWordStarted;
EventManager.Instance.OnNPCInteracted += HandleNPCInteraction;
EventManager.Instance.OnNPCNameCompleted += HandleNPCComplete;
EventManager.Instance.OnPlayerHitByEnemy += HandlePlayerHit;
EventManager.Instance.OnPlayerInventoryChanged += UpdateInventoryUI;
EventManager.Instance.OnGameStateChanged += HandleStateChange;
EventManager.Instance.OnVictory += HandleVictory;
EventManager.Instance.OnShowMessage += DisplayMessage;
EventManager.Instance.OnShowInteractionPrompt += ShowPrompt;

// Always unsubscribe in OnDestroy()!
```

## 🔧 Configuration Values

### PlayerController
- **Move Speed**: 5 (recommended)
- **Acceleration**: 10
- **Deceleration**: 10

### Letter
- **Float Amplitude**: 0.2
- **Float Speed**: 2
- **Bounce Height**: 1
- **Bounce Duration**: 0.5

### LetterSpawner
- **Spawn Interval**: 5 seconds
- **Max Active Letters**: 15

### UnfriendlyNPC
- **Move Speed**: 3
- **Chase Range**: 10
- **Letters to Drop**: 2
- **Attack Cooldown**: 2 seconds

### CameraFollow
- **Smooth Speed**: 5
- **Deadzone Size**: (2, 2)
- **Offset**: (0, 0, -10)

## 📊 Word Difficulty Guidelines

| Difficulty | Length | Examples |
|------------|--------|----------|
| Easy | 3 letters | CAT, DOG, SUN, BAT, HAT |
| Medium | 4-5 letters | BEAR, BIRD, FROG, STAR, TREE |
| Hard | 6+ letters | TIGER, SNAKE, FLOWER, RABBIT |

## 🎨 Sprite Requirements

### Letters (26 sprites)
- Size: 128x128 or 256x256
- Format: PNG with transparency
- Naming: `Letter_A.png` through `Letter_Z.png`
- Style: Bold, colorful, child-friendly

### Player Character
- Idle animation (4 directions)
- Walk animation (4 directions)
- Interact animation
- Pick up animation

### NPCs
- Friendly: Happy, colorful, recognizable (animals, objects)
- Unfriendly: Cartoonish monsters with angry faces

## 🐛 Common Issues

### Letters not spawning
- Check LetterSpawner has Letter Prefab assigned
- Call `LetterSpawner.Instance.StartSpawning()`
- Verify spawn area is reasonable

### Player can't move
- Check Rigidbody2D Gravity Scale = 0
- Verify Input System package is installed
- Check InputManager is in scene

### No sound
- Verify AudioManager exists in scene
- Check audio clips are assigned
- Check volume settings not at 0

### Collisions not working
- Verify collision matrix in Physics 2D settings
- Check colliders exist and proper layers assigned
- For triggers, ensure "Is Trigger" is checked

### UI not updating
- Check UIManager references are assigned
- Verify EventManager exists and events are subscribed
- Check Canvas is active

## 📁 File Locations

### Scripts
`Assets/Scripts/[Category]/[ScriptName].cs`

### Prefabs
`Assets/Prefabs/[Type]/[PrefabName].prefab`

### Sprites
`Assets/Sprites/[Type]/[SpriteName].png`

### Audio
`Assets/Audio/[Music|SFX]/[FileName].wav|.mp3`

### Word Data
`Assets/ScriptableObjects/Words/[WordName].asset`
or `Assets/Resources/Words/[WordName].asset` (for runtime loading)

### Scenes
`Assets/Scenes/[SceneName].unity`

## 🚀 Performance Tips

1. Use object pooling for frequently spawned objects
2. Limit active letters to 15-20 max
3. Use sprite atlases for batching
4. Enable sprite packing in Build Settings
5. Use coroutines instead of Update() where possible
6. Cache component references (don't use GetComponent in Update)
7. Use layer-based collision filtering

## 📞 Quick Help

- **Full Setup**: See SETUP_GUIDE.md
- **Architecture**: See PROJECT_STRUCTURE.md
- **Requirements**: See REQUIREMENTS.md
- **Tasks**: See TASKS.md
