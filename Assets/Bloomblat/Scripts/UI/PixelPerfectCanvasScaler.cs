using UnityEngine;
using UnityEngine.UI;
using Bloomblat.Camera;

namespace Bloomblat.UI
{
    /// <summary>
    /// Custom canvas scaler that works with the pixel-perfect camera system.
    /// Ensures UI elements scale properly with the 128x128 game resolution.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class PixelPerfectCanvasScaler : MonoBehaviour
    {
        [Header("Scaling Settings")]
        [SerializeField] private bool autoScale = true;
        [SerializeField] private float referencePixelsPerUnit = 16f;
        [SerializeField] private float fallbackScreenDPI = 96f;
        [SerializeField] private float defaultSpriteDPI = 96f;
        
        [Header("UI Scaling")]
        [SerializeField] private ScalingMode scalingMode = ScalingMode.ConstantPixelSize;
        [SerializeField] private float scaleFactor = 1f;
        [SerializeField] private Vector2 referenceResolution = new Vector2(128, 128);
        [SerializeField] private CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        [SerializeField] private float matchWidthOrHeight = 0.5f;
        
        public enum ScalingMode
        {
            ConstantPixelSize,
            ScaleWithScreenSize,
            ConstantPhysicalSize
        }
        
        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private PixelPerfectCameraController cameraController;
        
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvasScaler = GetComponent<CanvasScaler>();
            cameraController = FindFirstObjectByType<PixelPerfectCameraController>();

            if (canvasScaler == null)
            {
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
            }

            SetupCanvasScaler();
        }
        
        private void Start()
        {
            UpdateScaling();
        }
        
        private void Update()
        {
            if (autoScale)
            {
                UpdateScaling();
            }
        }
        
        private void SetupCanvasScaler()
        {
            // Ensure canvasScaler is not null
            if (canvasScaler == null)
            {
                Debug.LogWarning($"CanvasScaler is null on {gameObject.name}. Cannot setup canvas scaler.");
                return;
            }

            // Configure the canvas scaler based on our scaling mode
            switch (scalingMode)
            {
                case ScalingMode.ConstantPixelSize:
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    canvasScaler.scaleFactor = scaleFactor;
                    canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
                    break;

                case ScalingMode.ScaleWithScreenSize:
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.referenceResolution = referenceResolution;
                    canvasScaler.screenMatchMode = screenMatchMode;
                    canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
                    canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
                    break;

                case ScalingMode.ConstantPhysicalSize:
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                    canvasScaler.physicalUnit = CanvasScaler.Unit.Points;
                    canvasScaler.fallbackScreenDPI = fallbackScreenDPI;
                    canvasScaler.defaultSpriteDPI = defaultSpriteDPI;
                    canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
                    break;
            }
        }
        
        private void UpdateScaling()
        {
            if (cameraController == null || canvasScaler == null) return;

            // Update scaling based on the camera's pixel perfect settings
            switch (scalingMode)
            {
                case ScalingMode.ConstantPixelSize:
                    UpdateConstantPixelSize();
                    break;

                case ScalingMode.ScaleWithScreenSize:
                    UpdateScaleWithScreenSize();
                    break;

                case ScalingMode.ConstantPhysicalSize:
                    // Physical size scaling doesn't need updates
                    break;
            }
        }
        
        private void UpdateConstantPixelSize()
        {
            if (cameraController == null || canvasScaler == null) return;

            // Calculate scale factor based on the game area size vs reference resolution
            Vector2 gameAreaSize = cameraController.GameAreaSize;
            float scaleX = gameAreaSize.x / referenceResolution.x;
            float scaleY = gameAreaSize.y / referenceResolution.y;

            // Use the smaller scale to ensure UI fits
            float calculatedScale = Mathf.Min(scaleX, scaleY);

            // Apply minimum scale to ensure readability
            calculatedScale = Mathf.Max(calculatedScale, 0.5f);

            canvasScaler.scaleFactor = calculatedScale;
        }
        
        private void UpdateScaleWithScreenSize()
        {
            if (cameraController == null || canvasScaler == null) return;

            // Update reference resolution to match the game resolution
            referenceResolution = new Vector2(cameraController.GameWidth, cameraController.GameHeight);
            canvasScaler.referenceResolution = referenceResolution;
        }
        
        /// <summary>
        /// Sets the scaling mode and updates the canvas scaler
        /// </summary>
        public void SetScalingMode(ScalingMode mode)
        {
            scalingMode = mode;
            SetupCanvasScaler();
            UpdateScaling();
        }
        
        /// <summary>
        /// Sets the scale factor for constant pixel size mode
        /// </summary>
        public void SetScaleFactor(float factor)
        {
            scaleFactor = factor;
            if (scalingMode == ScalingMode.ConstantPixelSize && canvasScaler != null)
            {
                canvasScaler.scaleFactor = scaleFactor;
            }
        }
        
        /// <summary>
        /// Sets the reference resolution for scale with screen size mode
        /// </summary>
        public void SetReferenceResolution(Vector2 resolution)
        {
            referenceResolution = resolution;
            if (scalingMode == ScalingMode.ScaleWithScreenSize && canvasScaler != null)
            {
                canvasScaler.referenceResolution = referenceResolution;
            }
        }
        
        /// <summary>
        /// Gets the current effective scale factor
        /// </summary>
        public float GetEffectiveScaleFactor()
        {
            if (canvasScaler == null) return 1f;

            switch (scalingMode)
            {
                case ScalingMode.ConstantPixelSize:
                    return canvasScaler.scaleFactor;

                case ScalingMode.ScaleWithScreenSize:
                    // Calculate effective scale based on screen size and reference resolution
                    Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                    Vector2 scaleRatio = screenSize / referenceResolution;

                    return screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight
                        ? Mathf.Lerp(scaleRatio.x, scaleRatio.y, matchWidthOrHeight)
                        : Mathf.Min(scaleRatio.x, scaleRatio.y);

                case ScalingMode.ConstantPhysicalSize:
                    return Screen.dpi / defaultSpriteDPI;

                default:
                    return 1f;
            }
        }
        
        /// <summary>
        /// Converts a pixel size to UI units based on the current scaling
        /// </summary>
        public float PixelsToUIUnits(float pixels)
        {
            return pixels / referencePixelsPerUnit / GetEffectiveScaleFactor();
        }
        
        /// <summary>
        /// Converts UI units to pixel size based on the current scaling
        /// </summary>
        public float UIUnitsToPixels(float uiUnits)
        {
            return uiUnits * referencePixelsPerUnit * GetEffectiveScaleFactor();
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && canvasScaler != null)
            {
                SetupCanvasScaler();
                UpdateScaling();
            }
        }
        #endif
    }
}
