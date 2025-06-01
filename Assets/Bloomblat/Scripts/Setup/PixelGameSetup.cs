using UnityEngine;
using UnityEngine.Rendering.Universal;
using Bloomblat.Camera;
using Bloomblat.UI;

namespace Bloomblat.Setup
{
    /// <summary>
    /// Utility script to automatically set up a pixel-perfect game scene with proper camera and UI configuration.
    /// This script can be run in the editor to quickly configure a scene for 128x128 pixel art games.
    /// </summary>
    public class PixelGameSetup : MonoBehaviour
    {
        [Header("Setup Configuration")]
        [SerializeField] private bool autoSetupOnStart = false;
        [SerializeField] private bool createMobileControls = true;
        [SerializeField] private bool createGameUI = true;
        
        [Header("Camera Settings")]
        [SerializeField] private int pixelsPerUnit = 16;
        
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupPixelPerfectScene();
            }
        }
        
        [ContextMenu("Setup Pixel Perfect Scene")]
        public void SetupPixelPerfectScene()
        {
            Debug.Log("Setting up pixel-perfect scene...");
            
            // Setup main camera
            SetupMainCamera();
            
            // Setup mobile controls if needed
            if (createMobileControls)
            {
                SetupMobileControls();
            }
            
            // Setup game UI canvas
            if (createGameUI)
            {
                SetupGameUI();
            }
            
            Debug.Log("Pixel-perfect scene setup complete!");
        }
        
        private void SetupMainCamera()
        {
            // Find or create main camera
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                mainCamera = cameraObj.AddComponent<UnityEngine.Camera>();
            }

            // Add Pixel Perfect Camera component
            PixelPerfectCamera pixelPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
            if (pixelPerfectCamera == null)
            {
                pixelPerfectCamera = mainCamera.gameObject.AddComponent<PixelPerfectCamera>();
            }

            // Add our custom controller
            PixelPerfectCameraController controller = mainCamera.GetComponent<PixelPerfectCameraController>();
            if (controller == null)
            {
                controller = mainCamera.gameObject.AddComponent<PixelPerfectCameraController>();
            }

            // Configure camera settings
            mainCamera.orthographic = true;
            mainCamera.backgroundColor = Color.black;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;

            // Configure URP settings to fix render texture depth buffer issues
            SimpleURPHelper.FixDepthBufferIssue(mainCamera);

            Debug.Log("Main camera configured with pixel-perfect settings.");
        }
        
        private void SetupMobileControls()
        {
            // Note: Mobile controls are now handled by dependency injection through ControlsManagerInstaller
            // The controls manager will be automatically instantiated based on platform and configuration
            Debug.Log("Mobile controls will be handled by dependency injection system.");
        }
        
        private void SetupGameUI()
        {
            // Find or create game UI canvas
            GameObject uiObj = GameObject.Find("Game UI");
            if (uiObj == null)
            {
                uiObj = new GameObject("Game UI");
            }
            
            // Add Canvas component
            Canvas canvas = uiObj.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = uiObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
            }
            
            // Add CanvasScaler
            UnityEngine.UI.CanvasScaler scaler = uiObj.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler == null)
            {
                scaler = uiObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            }
            
            // Add our custom pixel perfect scaler
            PixelPerfectCanvasScaler pixelScaler = uiObj.GetComponent<PixelPerfectCanvasScaler>();
            if (pixelScaler == null)
            {
                pixelScaler = uiObj.AddComponent<PixelPerfectCanvasScaler>();
            }
            
            // Add GraphicRaycaster
            UnityEngine.UI.GraphicRaycaster raycaster = uiObj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = uiObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            Debug.Log("Game UI canvas setup complete.");
        }
        
        [ContextMenu("Validate Scene Setup")]
        public void ValidateSceneSetup()
        {
            Debug.Log("Validating pixel-perfect scene setup...");

            bool isValid = true;

            // Check main camera
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found!");
                isValid = false;
            }
            else
            {
                PixelPerfectCamera pixelPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
                if (pixelPerfectCamera == null)
                {
                    Debug.LogError("Main camera missing PixelPerfectCamera component!");
                    isValid = false;
                }

                PixelPerfectCameraController controller = mainCamera.GetComponent<PixelPerfectCameraController>();
                if (controller == null)
                {
                    Debug.LogError("Main camera missing PixelPerfectCameraController component!");
                    isValid = false;
                }

                if (!mainCamera.orthographic)
                {
                    Debug.LogWarning("Main camera should be orthographic for pixel art!");
                }

                // Validate URP settings
                if (!SimpleURPHelper.IsDepthBufferIssueFixed(mainCamera))
                {
                    Debug.LogWarning("Camera may have depth buffer issues. Check render texture settings.");
                }
            }
            
            // Check for EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogWarning("No EventSystem found! UI interactions may not work properly.");
                
                // Create EventSystem
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("Created EventSystem automatically.");
            }
            
            if (isValid)
            {
                Debug.Log("Scene setup validation passed!");
            }
            else
            {
                Debug.LogError("Scene setup validation failed! Please fix the issues above.");
            }
        }
        
        [ContextMenu("Create Test Sprite")]
        public void CreateTestSprite()
        {
            // Create a simple test sprite to verify pixel-perfect rendering
            GameObject testObj = new GameObject("Test Sprite");
            SpriteRenderer renderer = testObj.AddComponent<SpriteRenderer>();
            
            // Create a simple 16x16 pixel sprite
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point; // Important for pixel art!
            
            // Create a checkerboard pattern
            Color[] pixels = new Color[16 * 16];
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool isWhite = (x + y) % 2 == 0;
                    pixels[y * 16 + x] = isWhite ? Color.white : Color.black;
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            renderer.sprite = sprite;
            
            Debug.Log("Created test sprite for pixel-perfect validation.");
        }
    }
}