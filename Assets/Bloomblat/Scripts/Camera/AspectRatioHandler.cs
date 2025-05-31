using UnityEngine;

namespace Bloomblat.Camera
{
    /// <summary>
    /// Handles aspect ratio management and letterboxing for pixel-perfect rendering.
    /// Works in conjunction with PixelPerfectCameraController to maintain proper game area.
    /// </summary>
    public class AspectRatioHandler : MonoBehaviour
    {
        [Header("Letterbox Settings")]
        [SerializeField] private Color letterboxColor = Color.black;
        [SerializeField] private bool showLetterbox = true;
        
        private PixelPerfectCameraController cameraController;
        private UnityEngine.Camera letterboxCamera;
        private int lastScreenWidth;
        private int lastScreenHeight;
        
        public int LastScreenWidth => lastScreenWidth;
        public int LastScreenHeight => lastScreenHeight;
        
        public void Initialize(PixelPerfectCameraController controller)
        {
            cameraController = controller;
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            
            if (showLetterbox)
            {
                CreateLetterboxCamera();
            }
        }
        
        private void CreateLetterboxCamera()
        {
            // Create a background camera for letterboxing
            GameObject letterboxObj = new GameObject("Letterbox Camera");
            letterboxObj.transform.SetParent(transform);
            
            letterboxCamera = letterboxObj.AddComponent<UnityEngine.Camera>();
            letterboxCamera.depth = cameraController.GetComponent<UnityEngine.Camera>().depth - 1;
            letterboxCamera.clearFlags = CameraClearFlags.SolidColor;
            letterboxCamera.backgroundColor = letterboxColor;
            letterboxCamera.cullingMask = 0; // Don't render anything
            letterboxCamera.orthographic = true;
            letterboxCamera.orthographicSize = 1;
            letterboxCamera.nearClipPlane = -1;
            letterboxCamera.farClipPlane = 1;
        }
        
        public void UpdateAspectRatio()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            
            // The main camera viewport is handled by PixelPerfectCameraController
            // This class just tracks changes and manages letterboxing
        }
        
        /// <summary>
        /// Gets the current screen aspect ratio
        /// </summary>
        public float GetCurrentAspectRatio()
        {
            return (float)Screen.width / Screen.height;
        }
        
        /// <summary>
        /// Gets the target aspect ratio for the game
        /// </summary>
        public float GetTargetAspectRatio()
        {
            return (float)cameraController.GameWidth / cameraController.GameHeight;
        }
        
        /// <summary>
        /// Checks if the current aspect ratio matches the target (within tolerance)
        /// </summary>
        public bool IsAspectRatioMatched(float tolerance = 0.01f)
        {
            return Mathf.Abs(GetCurrentAspectRatio() - GetTargetAspectRatio()) < tolerance;
        }
        
        /// <summary>
        /// Gets the letterbox dimensions (black bars)
        /// </summary>
        public Vector4 GetLetterboxDimensions()
        {
            Vector2 gameAreaPos = cameraController.GameAreaPosition;
            Vector2 gameAreaSize = cameraController.GameAreaSize;
            
            // Returns: left, right, bottom, top letterbox sizes
            return new Vector4(
                gameAreaPos.x, // left
                Screen.width - (gameAreaPos.x + gameAreaSize.x), // right
                gameAreaPos.y, // bottom
                Screen.height - (gameAreaPos.y + gameAreaSize.y) // top
            );
        }
        
        /// <summary>
        /// Checks if a screen point is within the game area
        /// </summary>
        public bool IsPointInGameArea(Vector2 screenPoint)
        {
            Vector2 gameAreaPos = cameraController.GameAreaPosition;
            Vector2 gameAreaSize = cameraController.GameAreaSize;
            
            return screenPoint.x >= gameAreaPos.x &&
                   screenPoint.x <= gameAreaPos.x + gameAreaSize.x &&
                   screenPoint.y >= gameAreaPos.y &&
                   screenPoint.y <= gameAreaPos.y + gameAreaSize.y;
        }
        
        /// <summary>
        /// Converts a screen point to normalized coordinates within the game area
        /// </summary>
        public Vector2 ScreenToGameAreaNormalized(Vector2 screenPoint)
        {
            Vector2 gameAreaPos = cameraController.GameAreaPosition;
            Vector2 gameAreaSize = cameraController.GameAreaSize;
            
            return new Vector2(
                (screenPoint.x - gameAreaPos.x) / gameAreaSize.x,
                (screenPoint.y - gameAreaPos.y) / gameAreaSize.y
            );
        }
        
        private void OnGUI()
        {
            if (!Application.isPlaying || cameraController == null || !cameraController.ShowDebugInfo) return;
            
            // Draw debug information
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Screen: {Screen.width}x{Screen.height}");
            GUILayout.Label($"Game Area: {cameraController.GameAreaSize.x:F0}x{cameraController.GameAreaSize.y:F0}");
            GUILayout.Label($"Game Position: {cameraController.GameAreaPosition.x:F0}, {cameraController.GameAreaPosition.y:F0}");
            GUILayout.Label($"Current Aspect: {GetCurrentAspectRatio():F3}");
            GUILayout.Label($"Target Aspect: {GetTargetAspectRatio():F3}");
            GUILayout.Label($"Aspect Matched: {IsAspectRatioMatched()}");
            
            Vector4 letterbox = GetLetterboxDimensions();
            GUILayout.Label($"Letterbox L/R/B/T: {letterbox.x:F0}/{letterbox.y:F0}/{letterbox.z:F0}/{letterbox.w:F0}");
            
            GUILayout.EndArea();
        }
    }
}
