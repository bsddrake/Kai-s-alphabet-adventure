# Word Data ScriptableObjects

This folder contains WordData ScriptableObject assets for the game's word challenges.

## Creating a New Word

1. Right-click in this folder
2. Select **Create → Kai's Adventure → Word Data**
3. Name the file (e.g., `Word_Cat.asset`)
4. Configure the word:
   - **Word**: The word to spell (e.g., CAT)
   - **Difficulty**: Easy (3 letters), Medium (4-5 letters), or Hard (6+ letters)
   - **Category**: Animals, Colors, Objects, Nature, Food, Toys, Family, or Actions
   - **Hint**: Optional hint text for players
   - **Point Value**: Points awarded (default: 10)

## Example Words by Difficulty

### Easy (3 letters)
- CAT, DOG, SUN, BAT, HAT, BEE, FOX, BUG, ANT, RAT

### Medium (4-5 letters)
- BEAR, BIRD, FROG, STAR, TREE, MOON, FISH, DUCK, BLUE, LION

### Hard (6+ letters)
- TIGER, SNAKE, FLOWER, RABBIT, TURTLE, YELLOW, ORANGE, PURPLE

## Usage

### Option 1: Resources Folder
Place word assets in `Assets/Resources/Words/` for automatic loading by WordManager.

### Option 2: Direct Assignment
Assign words directly to WordManager's `All Words` list in the Inspector.

## Categories

- **Animals**: CAT, DOG, BEAR, BIRD, FROG, FISH, etc.
- **Colors**: RED, BLUE, GREEN, YELLOW, etc.
- **Objects**: HAT, BALL, BOOK, CHAIR, etc.
- **Nature**: TREE, FLOWER, SUN, STAR, MOON, etc.
- **Food**: APPLE, BREAD, CAKE, etc.
- **Toys**: BALL, DOLL, KITE, etc.
- **Family**: MOM, DAD, KID, etc.
- **Actions**: RUN, JUMP, PLAY, etc.

## Progressive Difficulty

The WordManager will automatically progress through difficulties:
1. Start with Easy words
2. After completing 3 Easy words, unlock Medium
3. After completing 3 Medium words, unlock Hard

Configure this in WordManager's Inspector:
- **Progressive Difficulty**: true/false
- **Words Per Difficulty**: Number before advancing (default: 3)
