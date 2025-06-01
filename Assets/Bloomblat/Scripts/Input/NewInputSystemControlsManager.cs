using UnityEngine;
using UnityEngine.InputSystem;
using Bloomblat.Camera;
using Ludo.UnityInject;

namespace Bloomblat.Input
{
    /// <summary>
    /// Controls manager implementation using Unity's new Input System.
    /// Supports keyboard, gamepad, and touch inputs through a unified interface.
    /// </summary>
    public class NewInputSystemControlsManager : IControlsManager
    {
        // Injected dependencies
        [Inject] private PixelPerfectCameraController cameraController;
        
        // Input actions
        private InputAction moveAction;
        private InputAction[] buttonActions;
        
        // Input state
        private Vector2 joystickInput;
        private bool[] buttonStates;
        private bool[] buttonDownStates;
        private bool[] buttonUpStates;
        private bool isInitialized;
        private bool isEnabled = true;
        
        // Configuration
        private const int ButtonCount = 2;
        
        // IControlsManager implementation
        public Vector2 JoystickInput => joystickInput;
        public float Horizontal => joystickInput.x;
        public float Vertical => joystickInput.y;
        public bool HasVisualControls => false; // No visual controls for new input system
        public bool IsActive => isEnabled && isInitialized;
        
        public bool GetButton(int index) => index >= 0 && index < buttonStates.Length && buttonStates[index];
        public bool GetButtonDown(int index) => index >= 0 && index < buttonDownStates.Length && buttonDownStates[index];
        public bool GetButtonUp(int index) => index >= 0 && index < buttonUpStates.Length && buttonUpStates[index];
        
        public void Initialize()
        {
            if (isInitialized) return;
            
            SetupInputActions();
            InitializeButtonArrays();
            EnableInputActions();
            
            isInitialized = true;
            Debug.Log("[NewInputSystemControlsManager] Initialized with Unity Input System");
        }
        
        public void UpdateInput()
        {
            if (!isInitialized || !isEnabled) return;
            
            // Clear previous frame's button events
            ClearButtonEvents();
            
            // Update movement input
            joystickInput = moveAction.ReadValue<Vector2>();
            
            // Update button states
            UpdateButtonStates();
        }
        
        public void SetEnabled(bool enabled)
        {
            isEnabled = enabled;
            
            if (isInitialized)
            {
                if (enabled)
                {
                    EnableInputActions();
                }
                else
                {
                    DisableInputActions();
                }
            }
        }
        
        public void Cleanup()
        {
            DisableInputActions();
            DisposeInputActions();
            
            joystickInput = Vector2.zero;
            ClearAllButtonStates();
            
            isInitialized = false;
        }
        
        private void SetupInputActions()
        {
            // Create movement input action
            moveAction = new InputAction("Move", InputActionType.Value, binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            
            // Add touch support for mobile
            #if UNITY_ANDROID || UNITY_IOS
            // For mobile, we can add touch controls here if needed
            // This would require more complex setup for virtual joystick simulation
            #endif
            
            // Create button input actions
            buttonActions = new InputAction[ButtonCount];
            
            // Action Button 1 (Space, Gamepad South, Touch)
            buttonActions[0] = new InputAction("Action1", InputActionType.Button);
            buttonActions[0].AddBinding("<Keyboard>/space");
            buttonActions[0].AddBinding("<Gamepad>/buttonSouth");
            #if UNITY_ANDROID || UNITY_IOS
            buttonActions[0].AddBinding("<Touchscreen>/primaryTouch/tap");
            #endif
            
            // Action Button 2 (X, Gamepad East)
            buttonActions[1] = new InputAction("Action2", InputActionType.Button);
            buttonActions[1].AddBinding("<Keyboard>/x");
            buttonActions[1].AddBinding("<Gamepad>/buttonEast");
        }
        
        private void EnableInputActions()
        {
            moveAction?.Enable();
            
            if (buttonActions != null)
            {
                foreach (var action in buttonActions)
                {
                    action?.Enable();
                }
            }
        }
        
        private void DisableInputActions()
        {
            moveAction?.Disable();
            
            if (buttonActions != null)
            {
                foreach (var action in buttonActions)
                {
                    action?.Disable();
                }
            }
        }
        
        private void DisposeInputActions()
        {
            moveAction?.Dispose();
            moveAction = null;
            
            if (buttonActions != null)
            {
                foreach (var action in buttonActions)
                {
                    action?.Dispose();
                }
                buttonActions = null;
            }
        }
        
        private void InitializeButtonArrays()
        {
            buttonStates = new bool[ButtonCount];
            buttonDownStates = new bool[ButtonCount];
            buttonUpStates = new bool[ButtonCount];
        }
        
        private void UpdateButtonStates()
        {
            for (int i = 0; i < buttonActions.Length; i++)
            {
                bool currentState = buttonActions[i].IsPressed();
                bool previousState = buttonStates[i];
                
                // Detect button down (was not pressed, now is pressed)
                if (currentState && !previousState)
                {
                    buttonDownStates[i] = true;
                }
                
                // Detect button up (was pressed, now is not pressed)
                if (!currentState && previousState)
                {
                    buttonUpStates[i] = true;
                }
                
                buttonStates[i] = currentState;
            }
        }
        
        private void ClearButtonEvents()
        {
            for (int i = 0; i < buttonDownStates.Length; i++)
            {
                buttonDownStates[i] = false;
                buttonUpStates[i] = false;
            }
        }
        
        private void ClearAllButtonStates()
        {
            for (int i = 0; i < buttonStates.Length; i++)
            {
                buttonStates[i] = false;
                buttonDownStates[i] = false;
                buttonUpStates[i] = false;
            }
        }
    }
}
