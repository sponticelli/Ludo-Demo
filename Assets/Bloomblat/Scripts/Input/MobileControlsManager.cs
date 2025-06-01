using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Bloomblat.Camera;
using Bloomblat.Input;
using Ludo.UnityInject;

namespace Bloomblat.UI
{
    /// <summary>
    /// Manages virtual controls for mobile devices, positioning them in the reserved area
    /// below the game screen. Handles joystick and button inputs.
    /// </summary>
    public class MobileControlsManager : MonoBehaviour, IControlsManager
    {
        [Header("Control References")]
        [SerializeField] private VirtualJoystick virtualJoystick;
        [SerializeField] private Button[] actionButtons;
        
        [Header("Layout Settings")]
        [SerializeField] private float joystickSize = 120f;
        [SerializeField] private float buttonSize = 60f;
        [SerializeField] private float edgeMargin = 20f;
        [SerializeField] private float buttonSpacing = 10f;
        
        [Header("Visual Settings")]
        [SerializeField] private Color joystickBackgroundColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color joystickHandleColor = new Color(1f, 1f, 1f, 0.8f);
        [SerializeField] private Color buttonColor = new Color(1f, 1f, 1f, 0.5f);

        // Injected dependencies
        [Inject] private PixelPerfectCameraController cameraController;

        private Canvas controlsCanvas;
        private RectTransform canvasRect;

        // Input values
        private Vector2 joystickInput;
        private bool[] buttonStates;
        private bool[] buttonDownStates;
        private bool[] buttonUpStates;
        private bool isInitialized;
        private bool isEnabled = true;

        // IControlsManager implementation
        public Vector2 JoystickInput => joystickInput;
        public float Horizontal => joystickInput.x;
        public float Vertical => joystickInput.y;
        public bool HasVisualControls => true;
        public bool IsActive => isEnabled && gameObject.activeInHierarchy;

        public bool GetButton(int index) => index >= 0 && index < buttonStates.Length && buttonStates[index];
        public bool GetButtonDown(int index) => index >= 0 && index < buttonDownStates.Length && buttonDownStates[index];
        public bool GetButtonUp(int index) => index >= 0 && index < buttonUpStates.Length && buttonUpStates[index];
        
        private void Awake()
        {
            controlsCanvas = GetComponent<Canvas>();
            canvasRect = GetComponent<RectTransform>();
            InitializeButtonArrays();
        }

        private void Start()
        {
            // Initialize will be called by the dependency injection system
            if (!isInitialized)
            {
                Initialize();
            }
        }

        private void Update()
        {
            if (isInitialized && isEnabled)
            {
                UpdateInput();
            }
        }

        // IControlsManager interface implementation
        public void Initialize()
        {
            if (isInitialized) return;

            SetupControlsCanvas();
            CreateVirtualControls();
            UpdateControlsVisibility();
            isInitialized = true;
        }

        public void UpdateInput()
        {
            // Clear previous frame's button events
            ClearButtonEvents();

            UpdateControlsVisibility();
            UpdateInputValues();
        }

        public void SetEnabled(bool enabled)
        {
            isEnabled = enabled;
            if (gameObject != null)
            {
                gameObject.SetActive(enabled && ShouldShowControls());
            }
        }

        public void Cleanup()
        {
            if (virtualJoystick != null)
            {
                virtualJoystick.ResetJoystick();
            }

            joystickInput = Vector2.zero;
            ClearAllButtonStates();
        }
        
        private void SetupControlsCanvas()
        {
            if (controlsCanvas == null)
            {
                controlsCanvas = gameObject.AddComponent<Canvas>();
            }
            
            controlsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            controlsCanvas.sortingOrder = 100; // Ensure controls are on top
            
            // Add GraphicRaycaster if not present
            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        
        private void CreateVirtualControls()
        {
            if (virtualJoystick == null)
            {
                CreateVirtualJoystick();
            }
            
            if (actionButtons == null || actionButtons.Length == 0)
            {
                CreateActionButtons();
            }
            
            PositionControls();
        }
        
        private void CreateVirtualJoystick()
        {
            // Create joystick container
            GameObject joystickObj = new GameObject("Virtual Joystick");
            joystickObj.transform.SetParent(transform);
            
            RectTransform joystickRect = joystickObj.AddComponent<RectTransform>();
            joystickRect.sizeDelta = new Vector2(joystickSize, joystickSize);
            
            // Create background
            GameObject backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(joystickObj.transform);
            
            RectTransform bgRect = backgroundObj.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(joystickSize, joystickSize);
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = backgroundObj.AddComponent<Image>();
            bgImage.color = joystickBackgroundColor;
            bgImage.sprite = CreateCircleSprite();
            
            // Create handle
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(joystickObj.transform);
            
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(joystickSize * 0.6f, joystickSize * 0.6f);
            handleRect.anchoredPosition = Vector2.zero;
            
            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = joystickHandleColor;
            handleImage.sprite = CreateCircleSprite();
            
            // Add VirtualJoystick component
            virtualJoystick = joystickObj.AddComponent<VirtualJoystick>();
            virtualJoystick.Initialize(bgRect, handleRect);
        }
        
        private void CreateActionButtons()
        {
            actionButtons = new Button[2]; // Create 2 action buttons by default
            
            for (int i = 0; i < actionButtons.Length; i++)
            {
                GameObject buttonObj = new GameObject($"Action Button {i + 1}");
                buttonObj.transform.SetParent(transform);
                
                RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonSize, buttonSize);
                
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = buttonColor;
                buttonImage.sprite = CreateCircleSprite();
                
                Button button = buttonObj.AddComponent<Button>();
                actionButtons[i] = button;
                
                // Add button events
                int buttonIndex = i;
                button.onClick.AddListener(() => OnButtonClick(buttonIndex));
                
                // Add event triggers for press/release
                EventTrigger trigger = buttonObj.AddComponent<EventTrigger>();
                
                EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                pointerDown.eventID = EventTriggerType.PointerDown;
                pointerDown.callback.AddListener((data) => OnButtonDown(buttonIndex));
                trigger.triggers.Add(pointerDown);
                
                EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                pointerUp.eventID = EventTriggerType.PointerUp;
                pointerUp.callback.AddListener((data) => OnButtonUp(buttonIndex));
                trigger.triggers.Add(pointerUp);
            }
        }
        
        private void PositionControls()
        {
            if (cameraController == null) return;
            
            float controlsAreaHeight = cameraController.IsMobile ? 200f : 0f;
            
            // Position joystick on the left
            if (virtualJoystick != null)
            {
                RectTransform joystickRect = virtualJoystick.GetComponent<RectTransform>();
                joystickRect.anchorMin = new Vector2(0, 0);
                joystickRect.anchorMax = new Vector2(0, 0);
                joystickRect.anchoredPosition = new Vector2(
                    edgeMargin + joystickSize / 2f,
                    edgeMargin + joystickSize / 2f
                );
            }
            
            // Position action buttons on the right
            if (actionButtons != null)
            {
                for (int i = 0; i < actionButtons.Length; i++)
                {
                    RectTransform buttonRect = actionButtons[i].GetComponent<RectTransform>();
                    buttonRect.anchorMin = new Vector2(1, 0);
                    buttonRect.anchorMax = new Vector2(1, 0);
                    
                    float xPos = -edgeMargin - buttonSize / 2f;
                    float yPos = edgeMargin + buttonSize / 2f + i * (buttonSize + buttonSpacing);
                    
                    buttonRect.anchoredPosition = new Vector2(xPos, yPos);
                }
            }
        }
        
        private void UpdateControlsVisibility()
        {
            bool shouldShow = ShouldShowControls();
            gameObject.SetActive(shouldShow && isEnabled);
        }

        private bool ShouldShowControls()
        {
            return cameraController != null && cameraController.IsMobile;
        }

        private void InitializeButtonArrays()
        {
            int buttonCount = actionButtons?.Length ?? 2; // Default to 2 buttons
            buttonStates = new bool[buttonCount];
            buttonDownStates = new bool[buttonCount];
            buttonUpStates = new bool[buttonCount];
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
        
        private void UpdateInputValues()
        {
            if (virtualJoystick != null)
            {
                joystickInput = virtualJoystick.GetInputVector();
            }
        }
        
        private void OnButtonDown(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < buttonStates.Length)
            {
                if (!buttonStates[buttonIndex])
                {
                    buttonDownStates[buttonIndex] = true;
                }
                buttonStates[buttonIndex] = true;
            }
        }

        private void OnButtonUp(int buttonIndex)
        {
            if (buttonIndex >= 0 && buttonIndex < buttonStates.Length)
            {
                if (buttonStates[buttonIndex])
                {
                    buttonUpStates[buttonIndex] = true;
                }
                buttonStates[buttonIndex] = false;
            }
        }
        
        private void OnButtonClick(int buttonIndex)
        {
            Debug.Log($"Action Button {buttonIndex + 1} clicked!");
        }
        
        private Sprite CreateCircleSprite()
        {
            // Create a simple circle texture
            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f - 2f;
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    
                    if (distance <= radius)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
    }
}
