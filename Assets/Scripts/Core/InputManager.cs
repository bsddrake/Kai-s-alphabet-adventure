using UnityEngine;
using UnityEngine.InputSystem;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Manages player input from keyboard and controller.
    /// Supports: REQ-2.2.1, REQ-2.2.2, REQ-2.2.3, REQ-1.1.3
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerInputActions inputActions;

        // Input values
        private Vector2 moveInput;
        private bool interactPressed;
        private bool pausePressed;

        public Vector2 MoveInput => moveInput;
        public bool InteractPressed => interactPressed;
        public bool PausePressed => pausePressed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize input actions
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            inputActions.Enable();

            // Subscribe to input events
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            inputActions.Player.Interact.performed += OnInteractPerformed;
            inputActions.Player.Pause.performed += OnPausePerformed;
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.Player.Interact.performed -= OnInteractPerformed;
            inputActions.Player.Pause.performed -= OnPausePerformed;

            inputActions.Disable();
        }

        private void Update()
        {
            // Reset one-time presses
            interactPressed = false;
            pausePressed = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            interactPressed = true;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            pausePressed = true;
        }

        public void EnableGameplayInput()
        {
            inputActions.Player.Enable();
        }

        public void DisableGameplayInput()
        {
            inputActions.Player.Disable();
        }

        public void EnableUIInput()
        {
            inputActions.UI.Enable();
        }

        public void DisableUIInput()
        {
            inputActions.UI.Disable();
        }
    }
}
