using UnityEngine;
using UnityEngine.InputSystem;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Input Actions wrapper for Unity Input System.
    /// This is a placeholder - the actual Input Actions asset should be created in Unity Editor.
    /// Supports: REQ-2.2.1, REQ-2.2.2, REQ-2.2.3
    /// </summary>
    public class PlayerInputActions
    {
        // This is a simplified version - In Unity, create an Input Actions asset with:
        // - Player Action Map: Move (Vector2), Interact (Button), Pause (Button)
        // - UI Action Map: Navigate (Vector2), Submit (Button), Cancel (Button)
        //
        // Bindings:
        // Move: WASD, Arrow Keys, Left Stick (Gamepad)
        // Interact: E, Space, Button South (Gamepad)
        // Pause: Escape, Start (Gamepad)

        public PlayerActionMap Player { get; private set; }
        public UIActionMap UI { get; private set; }

        public PlayerInputActions()
        {
            Player = new PlayerActionMap();
            UI = new UIActionMap();
        }

        public void Enable()
        {
            Player.Enable();
            UI.Enable();
        }

        public void Disable()
        {
            Player.Disable();
            UI.Disable();
        }

        public class PlayerActionMap
        {
            public InputAction Move { get; private set; }
            public InputAction Interact { get; private set; }
            public InputAction Pause { get; private set; }

            public PlayerActionMap()
            {
                // Note: In a real Unity project, these would be loaded from the Input Actions asset
                Move = new InputAction("Move", InputActionType.Value);
                Interact = new InputAction("Interact", InputActionType.Button);
                Pause = new InputAction("Pause", InputActionType.Button);

                // Setup bindings programmatically as fallback
                Move.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/w")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/s")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/a")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/d")
                    .With("Right", "<Keyboard>/rightArrow");

                Move.AddBinding("<Gamepad>/leftStick");

                Interact.AddBinding("<Keyboard>/e");
                Interact.AddBinding("<Keyboard>/space");
                Interact.AddBinding("<Gamepad>/buttonSouth");

                Pause.AddBinding("<Keyboard>/escape");
                Pause.AddBinding("<Gamepad>/start");
            }

            public void Enable()
            {
                Move.Enable();
                Interact.Enable();
                Pause.Enable();
            }

            public void Disable()
            {
                Move.Disable();
                Interact.Disable();
                Pause.Disable();
            }
        }

        public class UIActionMap
        {
            public InputAction Navigate { get; private set; }
            public InputAction Submit { get; private set; }
            public InputAction Cancel { get; private set; }

            public UIActionMap()
            {
                Navigate = new InputAction("Navigate", InputActionType.Value);
                Submit = new InputAction("Submit", InputActionType.Button);
                Cancel = new InputAction("Cancel", InputActionType.Button);

                Navigate.AddBinding("<Gamepad>/dpad");
                Navigate.AddBinding("<Gamepad>/leftStick");

                Submit.AddBinding("<Keyboard>/enter");
                Submit.AddBinding("<Gamepad>/buttonSouth");

                Cancel.AddBinding("<Keyboard>/escape");
                Cancel.AddBinding("<Gamepad>/buttonEast");
            }

            public void Enable()
            {
                Navigate.Enable();
                Submit.Enable();
                Cancel.Enable();
            }

            public void Disable()
            {
                Navigate.Disable();
                Submit.Disable();
                Cancel.Disable();
            }
        }
    }
}
