# Kai's Alphabet Adventure - Setup Guide

## Prerequisites

- **Unity 6.2 or later** (Unity 2022.3 LTS or newer should work)
- **Unity Input System Package** (will be installed via Package Manager)
- **TextMeshPro** (should be included by default in Unity)

## Initial Setup

### 1. Open the Project in Unity

1. Launch Unity Hub
2. Click "Add" and navigate to this project folder
3. Select Unity 6.2 (or compatible version)
4. Click "Open"

### 2. Install Required Packages

Unity will automatically import the project. If the Input System package is missing:

1. Open **Window → Package Manager**
2. Search for "Input System"
3. Click **Install**
4. When prompted to restart, click **Yes**

### 3. Configure Project Settings

#### Input System Setup

1. Go to **Edit → Project Settings → Player**
2. Under "Other Settings", set **Active Input Handling** to "Input System Package (New)"
3. Restart Unity if prompted

#### Physics 2D Layers

Set up collision layers:

1. Go to **Edit → Project Settings → Tags and Layers**
2. Add the following layers:
   - Layer 6: `Player`
   - Layer 7: `Collectible`
   - Layer 8: `Interactable`
   - Layer 9: `Enemy`
   - Layer 10: `Environment`

3. Go to **Edit → Project Settings → Physics 2D**
4. Configure the Layer Collision Matrix:
   - Player collides with: Environment, Enemy, Interactable
   - Collectible collides with: Player, Environment
   - Enemy collides with: Player, Environment

#### Tags Setup

1. Go to **Edit → Project Settings → Tags and Layers**
2. Add the following tags:
   - `Player`
   - `Letter`
   - `FriendlyNPC`
   - `UnfriendlyNPC`

### 4. Create Input Actions Asset

1. Right-click in **Assets/** folder
2. Select **Create → Input Actions**
3. Name it `PlayerInputActions`
4. Double-click to edit
5. Create the following Action Maps:

**Player Action Map:**
- Move (Value, Vector2)
  - WASD Composite
  - Arrow Keys Composite
  - Left Stick (Gamepad)
- Interact (Button)
  - E (Keyboard)
  - Space (Keyboard)
  - Button South (Gamepad)
- Pause (Button)
  - Escape (Keyboard)
  - Start (Gamepad)

**UI Action Map:**
- Navigate (Value, Vector2)
- Submit (Button)
- Cancel (Button)

6. Click **Save Asset**
7. Click **Generate C# Class** (optional, as we have a fallback implementation)

### 5. Create Scenes

#### Main Menu Scene
1. **File → New Scene**
2. Save as `Assets/Scenes/MainMenu.unity`
3. Add the following GameObjects:
   - `GameManager` (with GameManager, EventManager, SaveManager scripts)
   - `AudioManager` (with AudioManager script)
   - `Canvas` with MainMenu UI
   - `EventSystem`

#### Game Scene
1. **File → New Scene**
2. Save as `Assets/Scenes/GameScene.unity`
3. Add the following GameObjects:
   - `GameManager` (drag from prefab if created)
   - `AudioManager` (drag from prefab if created)
   - `InputManager` (with InputManager script)
   - `UIManager` (with UIManager script)
   - `WordManager` (with WordManager script)
   - `InventoryManager` (with InventoryManager script)
   - `LetterSpawner` (with LetterSpawner script)
   - `Main Camera` (with CameraFollow script)
   - `Player` (see Player Setup below)
   - `Canvas` with HUD
   - `EventSystem`

### 6. Create Player Prefab

1. Create an empty GameObject named `Player`
2. Add tag: `Player`
3. Set layer: `Player`
4. Add components:
   - SpriteRenderer
   - Rigidbody2D (Gravity Scale: 0, Freeze Rotation: Z)
   - CircleCollider2D or BoxCollider2D
   - PlayerController script
   - PlayerAnimator script
   - PlayerInteraction script
5. Configure PlayerController:
   - Move Speed: 5
   - Acceleration: 10
   - Deceleration: 10
6. Drag to **Assets/Prefabs/Player/** to create prefab

### 7. Create Letter Prefab

1. Create an empty GameObject named `Letter`
2. Add tag: `Letter`
3. Set layer: `Collectible`
4. Add components:
   - SpriteRenderer (assign a letter sprite)
   - CircleCollider2D (Is Trigger: true)
   - Letter script
5. Configure Letter:
   - Letter Character: A (will be set dynamically)
   - Collect On Contact: true
   - Float Amplitude: 0.2
   - Float Speed: 2
6. Drag to **Assets/Prefabs/Letters/** to create prefab

### 8. Create NPC Prefabs

#### Friendly NPC
1. Create GameObject named `FriendlyNPC_Bear` (example)
2. Add tag: `FriendlyNPC`
3. Set layer: `Interactable`
4. Add components:
   - SpriteRenderer
   - CircleCollider2D (Is Trigger: true)
   - Animator
   - FriendlyNPC script
   - TextMeshPro child for name display
5. Configure FriendlyNPC:
   - NPC Name: BEAR
   - Missing Letter: A (example)
6. Drag to **Assets/Prefabs/NPCs/**

#### Unfriendly NPC
1. Create GameObject named `UnfriendlyNPC_Monster`
2. Add tag: `UnfriendlyNPC`
3. Set layer: `Enemy`
4. Add components:
   - SpriteRenderer
   - Rigidbody2D (Gravity Scale: 0)
   - CircleCollider2D
   - Animator
   - UnfriendlyNPC script
5. Configure UnfriendlyNPC:
   - Move Speed: 3
   - Chase Range: 10
   - Letters To Drop: 2
6. Drag to **Assets/Prefabs/NPCs/**

### 9. Create UI Prefabs

#### Letter Slot UI
1. In Canvas, create UI → Image named `LetterSlot`
2. Add TextMeshProUGUI as child for letter display
3. Add LetterSlotUI script component
4. Drag to **Assets/Prefabs/UI/**

### 10. Import or Create Art Assets

Place your sprites in the appropriate folders:
- **Assets/Sprites/Player/** - Player character sprites
- **Assets/Sprites/Letters/** - A-Z letter sprites (26 images)
- **Assets/Sprites/NPCs/** - Friendly and unfriendly NPC sprites
- **Assets/Sprites/Environment/** - Tiles, background, decorations
- **Assets/Sprites/UI/** - UI elements

For letters, name them `Letter_A.png`, `Letter_B.png`, etc.

### 11. Import or Create Audio Assets

Place your audio files in:
- **Assets/Audio/Music/** - Background music tracks
- **Assets/Audio/SFX/** - Sound effects

Required sound effects:
- Letter collect sound
- Word complete sound
- Footstep sound
- Button click sound
- Interaction sound
- NPC hit sound
- Letter drop sound

### 12. Create Word Data ScriptableObjects

1. Right-click in **Assets/ScriptableObjects/Words/**
2. Select **Create → Kai's Adventure → Word Data**
3. Configure the word:
   - Word: CAT
   - Difficulty: Easy
   - Category: Animals
   - Hint: "A furry pet that says meow"
4. Create at least 20 words (see REQUIREMENTS.md for suggestions)

### 13. Configure Managers in Scene

#### AudioManager
- Assign music clips (Main Menu, Gameplay, Victory)
- Assign SFX clips to corresponding fields

#### LetterSpawner
- Assign Letter Prefab
- Assign letter sprites array (A-Z)
- Configure spawn area and interval

#### WordManager
- If using Resources: Place WordData ScriptableObjects in `Assets/Resources/Words/`
- Or assign words directly to `All Words` list

#### UIManager
- Assign all UI panel references
- Assign HUD element references

### 14. Build Settings

1. Go to **File → Build Settings**
2. Add scenes in order:
   - MainMenu
   - GameScene
3. Select target platform (PC, Mac, Linux)
4. Configure player settings as needed

## Testing

1. Press **Play** in Unity Editor
2. Test basic movement (WASD/Arrow keys)
3. Test letter collection
4. Test word completion
5. Test NPC interactions
6. Test pause menu (ESC)
7. Test settings (audio controls)

## Troubleshooting

### Input System Not Working
- Verify Input System package is installed
- Check Player Settings → Active Input Handling is set to "Input System Package"
- Restart Unity

### Scripts Not Compiling
- Check for missing namespaces
- Verify all scripts are in correct folders
- Check Unity console for specific errors

### Objects Not Interacting
- Verify layer collision matrix is configured correctly
- Check that colliders are set to "Is Trigger" where appropriate
- Verify tags are assigned correctly

### Audio Not Playing
- Check AudioManager has clips assigned
- Verify AudioManager exists in scene and is not destroyed
- Check volume settings in Settings menu

## Next Steps

1. Create custom sprites for player, letters, and NPCs
2. Create animations for player and NPCs
3. Design and build game levels
4. Create particle effects for visual feedback
5. Compose or source music and sound effects
6. Playtest and iterate on gameplay
7. Build for target platforms

## Additional Resources

- See **REQUIREMENTS.md** for full feature requirements
- See **TASKS.md** for detailed development task breakdown
- Unity Input System Documentation: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- TextMeshPro Documentation: https://docs.unity3d.com/Manual/com.unity.textmeshpro.html
