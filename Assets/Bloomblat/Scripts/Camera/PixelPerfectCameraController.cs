using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Bloomblat.Camera
{
    /// <summary>
    /// Controls pixel-perfect rendering for a 128x128 game resolution across all platforms.
    /// Handles aspect ratio management and ensures crisp pixel art rendering.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    [RequireComponent(typeof(PixelPerfectCamera))]
    public class PixelPerfectCameraController : MonoBehaviour
    {
        [Header("Game Resolution")]
        [SerializeField] private int gameWidth = 128;
        [SerializeField] private int gameHeight = 128;
        
        [Header("Pixel Perfect Settings")]
        [SerializeField] private int pixelsPerUnit = 16;
        [SerializeField] private PixelPerfectCamera.GridSnapping gridSnapping = PixelPerfectCamera.GridSnapping.None;
        [SerializeField] private PixelPerfectCamera.CropFrame cropFrame = PixelPerfectCamera.CropFrame.None;
        
        [Header("Mobile Settings")]
        [SerializeField] private bool isMobile = false;
        [SerializeField] private float mobileControlsHeight = 200f; // Height reserved for controls in pixels
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private Color debugBorderColor = Color.red;
        
        private UnityEngine.Camera mainCamera;
        private PixelPerfectCamera pixelPerfectCamera;
        private AspectRatioHandler aspectRatioHandler;
        
        // Calculated values
        private float targetAspectRatio;
        private Vector2 gameAreaSize;
        private Vector2 gameAreaPosition;
        
        public int GameWidth => gameWidth;
        public int GameHeight => gameHeight;
        public Vector2 GameAreaSize => gameAreaSize;
        public Vector2 GameAreaPosition => gameAreaPosition;
        public bool IsMobile => isMobile;
        public bool ShowDebugInfo => showDebugInfo;
        
        private void Awake()
        {
            mainCamera = GetComponent<UnityEngine.Camera>();
            pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
            aspectRatioHandler = GetComponent<AspectRatioHandler>();

            // Add missing components if needed
            if (mainCamera == null)
            {
                Debug.LogError($"No Camera component found on {gameObject.name}. PixelPerfectCameraController requires a Camera component.");
                return;
            }

            if (pixelPerfectCamera == null)
            {
                Debug.LogWarning($"No PixelPerfectCamera component found on {gameObject.name}. Adding one automatically.");
                pixelPerfectCamera = gameObject.AddComponent<PixelPerfectCamera>();
            }

            if (aspectRatioHandler == null)
            {
                aspectRatioHandler = gameObject.AddComponent<AspectRatioHandler>();
            }

            targetAspectRatio = (float)gameWidth / gameHeight;

            // Detect mobile platform
            #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
            #endif

            SetupPixelPerfectCamera();
        }
        
        private void Start()
        {
            if (mainCamera != null && pixelPerfectCamera != null)
            {
                UpdateCameraSettings();

                if (aspectRatioHandler != null)
                {
                    aspectRatioHandler.Initialize(this);
                }
            }
        }
        
        private void Update()
        {
            // Update camera settings if screen size changes
            if (aspectRatioHandler != null &&
                (Screen.width != aspectRatioHandler.LastScreenWidth ||
                Screen.height != aspectRatioHandler.LastScreenHeight))
            {
                UpdateCameraSettings();
            }
        }
        
        private void SetupPixelPerfectCamera()
        {
            // Ensure components are not null
            if (pixelPerfectCamera == null)
            {
                Debug.LogWarning($"PixelPerfectCamera component is null on {gameObject.name}. Cannot setup pixel perfect camera.");
                return;
            }

            if (mainCamera == null)
            {
                Debug.LogWarning($"Main Camera component is null on {gameObject.name}. Cannot setup pixel perfect camera.");
                return;
            }

            // Configure the Pixel Perfect Camera component
            pixelPerfectCamera.assetsPPU = pixelsPerUnit;
            pixelPerfectCamera.refResolutionX = gameWidth;
            pixelPerfectCamera.refResolutionY = gameHeight;
            pixelPerfectCamera.gridSnapping = gridSnapping;
            pixelPerfectCamera.cropFrame = cropFrame;

            // Set camera to orthographic
            mainCamera.orthographic = true;
            mainCamera.nearClipPlane = -10f;
            mainCamera.farClipPlane = 10f;

            // Ensure camera doesn't use a render texture (fixes URP depth buffer issue)
            mainCamera.targetTexture = null;

            // Configure URP-specific settings
            SetupURPCameraSettings();
        }

        private void SetupURPCameraSettings()
        {
            // Use the simple helper to fix depth buffer issues and configure URP
            SimpleURPHelper.FixDepthBufferIssue(mainCamera);

            // Validate the setup
            if (showDebugInfo)
            {
                SimpleURPHelper.LogCameraConfiguration(mainCamera);
                bool isFixed = SimpleURPHelper.IsDepthBufferIssueFixed(mainCamera);
                Debug.Log($"Depth buffer issue fixed: {isFixed}");
            }
        }

        public void UpdateCameraSettings()
        {
            if (aspectRatioHandler != null)
            {
                aspectRatioHandler.UpdateAspectRatio();
            }
            CalculateGameArea();

            // Update orthographic size based on the pixel perfect camera
            if (mainCamera != null)
            {
                mainCamera.orthographicSize = gameHeight / 2f / pixelsPerUnit;
            }
        }
        
        private void CalculateGameArea()
        {
            float screenAspect = (float)Screen.width / Screen.height;
            
            if (isMobile)
            {
                // For mobile, reserve space at the bottom for controls
                float availableHeight = Screen.height - mobileControlsHeight;
                float availableAspect = (float)Screen.width / availableHeight;
                
                if (availableAspect >= targetAspectRatio)
                {
                    // Screen is wider than target - fit height
                    gameAreaSize.y = availableHeight;
                    gameAreaSize.x = availableHeight * targetAspectRatio;
                    gameAreaPosition.x = (Screen.width - gameAreaSize.x) / 2f;
                    gameAreaPosition.y = mobileControlsHeight;
                }
                else
                {
                    // Screen is taller than target - fit width
                    gameAreaSize.x = Screen.width;
                    gameAreaSize.y = Screen.width / targetAspectRatio;
                    gameAreaPosition.x = 0f;
                    gameAreaPosition.y = mobileControlsHeight + (availableHeight - gameAreaSize.y) / 2f;
                }
            }
            else
            {
                // For PC/WebGL, use full screen with letterboxing
                if (screenAspect >= targetAspectRatio)
                {
                    // Screen is wider than target - fit height
                    gameAreaSize.y = Screen.height;
                    gameAreaSize.x = Screen.height * targetAspectRatio;
                    gameAreaPosition.x = (Screen.width - gameAreaSize.x) / 2f;
                    gameAreaPosition.y = 0f;
                }
                else
                {
                    // Screen is taller than target - fit width
                    gameAreaSize.x = Screen.width;
                    gameAreaSize.y = Screen.width / targetAspectRatio;
                    gameAreaPosition.x = 0f;
                    gameAreaPosition.y = (Screen.height - gameAreaSize.y) / 2f;
                }
            }
            
            // Update camera viewport
            Rect viewport = new Rect(
                gameAreaPosition.x / Screen.width,
                gameAreaPosition.y / Screen.height,
                gameAreaSize.x / Screen.width,
                gameAreaSize.y / Screen.height
            );
            
            if (mainCamera != null)
            {
                mainCamera.rect = viewport;
            }
        }
        
        /// <summary>
        /// Converts screen coordinates to world coordinates within the game area
        /// </summary>
        public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            // Adjust screen point to camera viewport
            Vector2 viewportPoint = new Vector2(
                (screenPoint.x - gameAreaPosition.x) / gameAreaSize.x,
                (screenPoint.y - gameAreaPosition.y) / gameAreaSize.y
            );
            
            return mainCamera.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, mainCamera.nearClipPlane));
        }
        
        /// <summary>
        /// Gets the world bounds of the game area
        /// </summary>
        public Bounds GetWorldBounds()
        {
            if (mainCamera == null)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

            Vector3 center = (bottomLeft + topRight) / 2f;
            Vector3 size = topRight - bottomLeft;

            return new Bounds(center, size);
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;
            
            // Draw game area bounds in scene view
            Bounds bounds = GetWorldBounds();
            Gizmos.color = debugBorderColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && mainCamera != null && pixelPerfectCamera != null)
            {
                SetupPixelPerfectCamera();
                UpdateCameraSettings();
            }
        }
        #endif
    }
}