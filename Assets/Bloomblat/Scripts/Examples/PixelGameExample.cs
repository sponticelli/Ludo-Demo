using UnityEngine;
using Bloomblat.Camera;
using Bloomblat.Core;
using Bloomblat.Input;
using Ludo.UnityInject;

namespace Bloomblat.Examples
{
    /// <summary>
    /// Example script demonstrating how to use the pixel-perfect camera system
    /// and controls manager with dependency injection in a simple game scenario.
    /// </summary>
    public class PixelGameExample : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private Transform player;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private bool usePixelMovement = true;

        [Header("Camera Settings")]
        [SerializeField] private bool followPlayer = true;
        [SerializeField] private float cameraFollowSpeed = 5f;
        [SerializeField] private Vector2 cameraOffset = Vector2.zero;
        [SerializeField] private PixelPerfectCameraController cameraController;
        
        // Injected dependencies
        [Inject] private IControlsManager controlsManager;
        [Inject] private IPixelDataProvider pixelDataProvider;

        private Vector2 inputVector;
        private Vector3 targetCameraPosition;
        
        private void Start()
        {
            // Dependencies should be injected by now
            if (cameraController == null)
            {
                Debug.LogError("PixelPerfectCameraController not injected! Please ensure dependency injection is set up correctly.");
                return;
            }

            if (controlsManager == null)
            {
                Debug.LogError("IControlsManager not injected! Please ensure dependency injection is set up correctly.");
                return;
            }

            // Initialize the controls manager
            controlsManager.Initialize();

            // Create a simple player if none exists
            if (player == null)
            {
                CreateSimplePlayer();
            }

            // Initialize camera position
            if (followPlayer && player != null)
            {
                targetCameraPosition = player.position + (Vector3)cameraOffset;
                cameraController.transform.position = targetCameraPosition;
            }

            Debug.Log("Pixel Game Example initialized with dependency injection!");
            LogSystemInfo();
        }
        
        private void Update()
        {
            // Update controls manager input
            if (controlsManager != null && controlsManager.IsActive)
            {
                controlsManager.UpdateInput();
            }

            HandleInput();
            MovePlayer();
            UpdateCamera();
            HandleDebugInput();
        }

        private void HandleInput()
        {
            if (controlsManager == null || !controlsManager.IsActive)
            {
                inputVector = Vector2.zero;
                return;
            }

            // Get input from the controls manager (works for both mobile and desktop)
            inputVector = controlsManager.JoystickInput;

            // Handle action buttons
            if (controlsManager.GetButtonDown(0))
            {
                OnActionButton1();
            }

            if (controlsManager.GetButtonDown(1))
            {
                OnActionButton2();
            }
        }
        
        private void MovePlayer()
        {
            if (player == null) return;
            
            Vector3 movement = new Vector3(inputVector.x, inputVector.y, 0) * moveSpeed * Time.deltaTime;
            
            if (usePixelMovement)
            {
                // Snap movement to pixel boundaries
                movement = SnapToPixel(movement);
            }
            
            player.position += movement;
            
            // Keep player within world bounds
            Bounds worldBounds = cameraController.GetWorldBounds();
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(player.position.x, worldBounds.min.x + 0.5f, worldBounds.max.x - 0.5f),
                Mathf.Clamp(player.position.y, worldBounds.min.y + 0.5f, worldBounds.max.y - 0.5f),
                player.position.z
            );
            
            player.position = clampedPosition;
        }
        
        private void UpdateCamera()
        {
            if (!followPlayer || player == null) return;
            
            // Calculate target camera position
            Vector3 desiredPosition = player.position + (Vector3)cameraOffset;
            
            // Smooth camera following
            targetCameraPosition = Vector3.Lerp(
                targetCameraPosition,
                desiredPosition,
                cameraFollowSpeed * Time.deltaTime
            );
            
            // Apply pixel-perfect positioning
            if (usePixelMovement)
            {
                targetCameraPosition = SnapToPixel(targetCameraPosition);
            }
            
            cameraController.transform.position = new Vector3(
                targetCameraPosition.x,
                targetCameraPosition.y,
                cameraController.transform.position.z
            );
        }
        
        private Vector3 SnapToPixel(Vector3 position)
        {
            // Use the injected pixel data provider for pixel snapping
            if (pixelDataProvider != null)
            {
                return pixelDataProvider.SnapToPixel(position);
            }

            // Fallback to hardcoded value if provider is not available
            float pixelsPerUnit = 16f;
            return new Vector3(
                Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit,
                Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit,
                position.z
            );
        }
        
        private void CreateSimplePlayer()
        {
            // Create a simple colored square as the player
            GameObject playerObj = new GameObject("Player");
            player = playerObj.transform;
            
            SpriteRenderer renderer = playerObj.AddComponent<SpriteRenderer>();
            
            // Create a simple 16x16 sprite
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            
            // Fill with a solid color
            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.cyan;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            renderer.sprite = sprite;
            
            // Position at center
            player.position = Vector3.zero;
            
            Debug.Log("Created simple player sprite.");
        }
        
        private void OnActionButton1()
        {
            Debug.Log("Action Button 1 pressed!");
            
            // Example: Change player color
            if (player != null)
            {
                SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = Random.ColorHSV();
                }
            }
        }
        
        private void OnActionButton2()
        {
            Debug.Log("Action Button 2 pressed!");
            
            // Example: Reset player position
            if (player != null)
            {
                player.position = Vector3.zero;
            }
        }
        
        private void HandleDebugInput()
        {
            // Debug controls (PC only)
            if (!cameraController.IsMobile)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                {
                    LogSystemInfo();
                }
                
                if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
                {
                    TogglePixelMovement();
                }
                
                if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
                {
                    ToggleCameraFollow();
                }
            }
        }
        
        private void TogglePixelMovement()
        {
            usePixelMovement = !usePixelMovement;
            Debug.Log($"Pixel movement: {(usePixelMovement ? "ON" : "OFF")}");
        }
        
        private void ToggleCameraFollow()
        {
            followPlayer = !followPlayer;
            Debug.Log($"Camera follow: {(followPlayer ? "ON" : "OFF")}");
        }
        
        private void LogSystemInfo()
        {
            Debug.Log("=== Pixel Game System Info ===");
            Debug.Log($"Screen Resolution: {Screen.width}x{Screen.height}");
            Debug.Log($"Game Resolution: {cameraController.GameWidth}x{cameraController.GameHeight}");
            Debug.Log($"Game Area Size: {cameraController.GameAreaSize}");
            Debug.Log($"Game Area Position: {cameraController.GameAreaPosition}");
            Debug.Log($"Is Mobile: {cameraController.IsMobile}");
            Debug.Log($"Controls Manager: {controlsManager?.GetType().Name ?? "None"}");
            Debug.Log($"Controls Manager Active: {controlsManager?.IsActive ?? false}");
            Debug.Log($"Has Visual Controls: {controlsManager?.HasVisualControls ?? false}");

            Bounds worldBounds = cameraController.GetWorldBounds();
            Debug.Log($"World Bounds: {worldBounds.size} at {worldBounds.center}");
        }
        
        private void OnGUI()
        {
            if (!cameraController.IsMobile)
            {
                // Show debug info on PC
                GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 120));
                GUILayout.Label("Debug Controls (PC only):");
                GUILayout.Label("WASD/Arrows: Move player");
                GUILayout.Label("Space/X: Action buttons");
                GUILayout.Label("F1: Log system info");
                GUILayout.Label("F2: Toggle pixel movement");
                GUILayout.Label("F3: Toggle camera follow");
                GUILayout.EndArea();
            }
        }
    }
}