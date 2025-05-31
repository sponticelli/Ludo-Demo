#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using Bloomblat.Camera;
using Bloomblat.UI;
using Bloomblat.Setup;

namespace Bloomblat.Editor
{
    /// <summary>
    /// Editor window for setting up pixel-perfect games with 128x128 resolution.
    /// Provides a user-friendly interface for configuring all necessary components.
    /// </summary>
    public class PixelPerfectSetupEditor : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        
        // Setup options
        private int gameWidth = 128;
        private int gameHeight = 128;
        private int pixelsPerUnit = 16;
        private bool createMobileControls = true;
        private bool createGameUI = true;
        private bool setupEventSystem = true;
        private bool createTestSprite = false;
        
        // Advanced settings
        private Color backgroundColor = Color.black;
        private Color letterboxColor = Color.black;
        private float mobileControlsHeight = 200f;
        
        [MenuItem("Bloomblat/Pixel Perfect Setup")]
        public static void ShowWindow()
        {
            PixelPerfectSetupEditor window = GetWindow<PixelPerfectSetupEditor>("Pixel Perfect Setup");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            DrawBasicSettings();
            DrawSetupOptions();
            DrawAdvancedSettings();
            DrawActionButtons();
            DrawValidationInfo();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 18;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField("Pixel Perfect Game Setup", headerStyle);
            EditorGUILayout.LabelField("Configure your 128x128 pixel art game", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        
        private void DrawBasicSettings()
        {
            EditorGUILayout.LabelField("Basic Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            gameWidth = EditorGUILayout.IntField("Game Width", gameWidth);
            gameHeight = EditorGUILayout.IntField("Game Height", gameHeight);
            pixelsPerUnit = EditorGUILayout.IntField("Pixels Per Unit", pixelsPerUnit);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Recommended: 128x128 resolution with 16 pixels per unit for PICO-8 style games.", MessageType.Info);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
        
        private void DrawSetupOptions()
        {
            EditorGUILayout.LabelField("Setup Options", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            createMobileControls = EditorGUILayout.Toggle("Create Mobile Controls", createMobileControls);
            createGameUI = EditorGUILayout.Toggle("Create Game UI Canvas", createGameUI);
            setupEventSystem = EditorGUILayout.Toggle("Setup Event System", setupEventSystem);
            createTestSprite = EditorGUILayout.Toggle("Create Test Sprite", createTestSprite);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);
            
            if (showAdvancedSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
                letterboxColor = EditorGUILayout.ColorField("Letterbox Color", letterboxColor);
                mobileControlsHeight = EditorGUILayout.FloatField("Mobile Controls Height", mobileControlsHeight);
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space(10);
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (GUILayout.Button("Setup Complete Scene", GUILayout.Height(30)))
            {
                SetupCompleteScene();
            }
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Setup Camera Only"))
            {
                SetupCamera();
            }
            
            if (GUILayout.Button("Setup UI Only"))
            {
                SetupUI();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Validate Setup"))
            {
                ValidateSetup();
            }
            
            if (GUILayout.Button("Create Test Sprite"))
            {
                CreateTestSprite();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
        
        private void DrawValidationInfo()
        {
            EditorGUILayout.LabelField("Current Scene Status", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Check main camera
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            bool hasMainCamera = mainCamera != null;
            bool hasPixelPerfectCamera = hasMainCamera && mainCamera.GetComponent<PixelPerfectCamera>() != null;
            bool hasController = hasMainCamera && mainCamera.GetComponent<PixelPerfectCameraController>() != null;
            
            DrawStatusLine("Main Camera", hasMainCamera);
            DrawStatusLine("Pixel Perfect Camera", hasPixelPerfectCamera);
            DrawStatusLine("Camera Controller", hasController);
            
            // Check UI
            MobileControlsManager mobileControls = FindFirstObjectByType<MobileControlsManager>();
            PixelPerfectCanvasScaler canvasScaler = FindFirstObjectByType<PixelPerfectCanvasScaler>();
            UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            
            DrawStatusLine("Mobile Controls", mobileControls != null);
            DrawStatusLine("Pixel Perfect UI", canvasScaler != null);
            DrawStatusLine("Event System", eventSystem != null);
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawStatusLine(string label, bool status)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(150));
            
            GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
            statusStyle.normal.textColor = status ? Color.green : Color.red;
            
            EditorGUILayout.LabelField(status ? "✓ Ready" : "✗ Missing", statusStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        private void SetupCompleteScene()
        {
            if (EditorUtility.DisplayDialog("Setup Complete Scene", 
                "This will set up a complete pixel-perfect scene with camera, UI, and mobile controls. Continue?", 
                "Yes", "Cancel"))
            {
                SetupCamera();
                
                if (createGameUI)
                {
                    SetupUI();
                }
                
                if (createMobileControls)
                {
                    SetupMobileControls();
                }
                
                if (setupEventSystem)
                {
                    SetupEventSystem();
                }
                
                if (createTestSprite)
                {
                    CreateTestSprite();
                }
                
                Debug.Log("Complete pixel-perfect scene setup finished!");
                EditorUtility.DisplayDialog("Setup Complete", "Pixel-perfect scene setup completed successfully!", "OK");
            }
        }
        
        private void SetupCamera()
        {
            // Find or create main camera
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                mainCamera = cameraObj.AddComponent<UnityEngine.Camera>();
            }
            
            // Configure camera
            mainCamera.orthographic = true;
            mainCamera.backgroundColor = backgroundColor;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            
            // Add Pixel Perfect Camera
            PixelPerfectCamera pixelPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
            if (pixelPerfectCamera == null)
            {
                pixelPerfectCamera = mainCamera.gameObject.AddComponent<PixelPerfectCamera>();
            }
            
            // Configure pixel perfect settings
            pixelPerfectCamera.assetsPPU = pixelsPerUnit;
            pixelPerfectCamera.refResolutionX = gameWidth;
            pixelPerfectCamera.refResolutionY = gameHeight;
            pixelPerfectCamera.upscaleRT = false;
            pixelPerfectCamera.pixelSnapping = true;
            pixelPerfectCamera.cropFrameX = false;
            pixelPerfectCamera.cropFrameY = false;
            pixelPerfectCamera.stretchFill = false;
            
            // Add our controller
            PixelPerfectCameraController controller = mainCamera.GetComponent<PixelPerfectCameraController>();
            if (controller == null)
            {
                controller = mainCamera.gameObject.AddComponent<PixelPerfectCameraController>();
            }

            // Configure URP settings to fix render texture depth buffer issues
            SimpleURPHelper.FixDepthBufferIssue(mainCamera);

            EditorUtility.SetDirty(mainCamera.gameObject);
            Debug.Log("Camera setup complete!");
        }
        
        private void SetupUI()
        {
            // Create Game UI Canvas
            GameObject uiObj = GameObject.Find("Game UI");
            if (uiObj == null)
            {
                uiObj = new GameObject("Game UI");
            }
            
            Canvas canvas = uiObj.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = uiObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
            }
            
            UnityEngine.UI.CanvasScaler scaler = uiObj.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler == null)
            {
                scaler = uiObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            }
            
            PixelPerfectCanvasScaler pixelScaler = uiObj.GetComponent<PixelPerfectCanvasScaler>();
            if (pixelScaler == null)
            {
                pixelScaler = uiObj.AddComponent<PixelPerfectCanvasScaler>();
            }
            
            UnityEngine.UI.GraphicRaycaster raycaster = uiObj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = uiObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            EditorUtility.SetDirty(uiObj);
            Debug.Log("UI setup complete!");
        }
        
        private void SetupMobileControls()
        {
            GameObject controlsObj = GameObject.Find("Mobile Controls");
            if (controlsObj == null)
            {
                controlsObj = new GameObject("Mobile Controls");
            }
            
            Canvas canvas = controlsObj.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = controlsObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
            }
            
            MobileControlsManager controlsManager = controlsObj.GetComponent<MobileControlsManager>();
            if (controlsManager == null)
            {
                controlsManager = controlsObj.AddComponent<MobileControlsManager>();
            }
            
            EditorUtility.SetDirty(controlsObj);
            Debug.Log("Mobile controls setup complete!");
        }
        
        private void SetupEventSystem()
        {
            UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                EditorUtility.SetDirty(eventSystemObj);
                Debug.Log("EventSystem created!");
            }
        }
        
        private void CreateTestSprite()
        {
            PixelGameSetup setup = FindFirstObjectByType<PixelGameSetup>();
            if (setup == null)
            {
                GameObject setupObj = new GameObject("Pixel Game Setup");
                setup = setupObj.AddComponent<PixelGameSetup>();
            }
            
            setup.CreateTestSprite();
        }
        
        private void ValidateSetup()
        {
            PixelGameSetup setup = FindFirstObjectByType<PixelGameSetup>();
            if (setup == null)
            {
                GameObject setupObj = new GameObject("Pixel Game Setup");
                setup = setupObj.AddComponent<PixelGameSetup>();
            }
            
            setup.ValidateSceneSetup();
        }
    }
}
#endif
