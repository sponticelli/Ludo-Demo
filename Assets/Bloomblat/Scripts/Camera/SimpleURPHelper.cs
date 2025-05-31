using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Bloomblat.Camera
{
    /// <summary>
    /// Simplified URP helper that focuses on fixing the depth buffer issue
    /// and essential pixel-perfect settings. Uses only guaranteed-to-exist properties.
    /// </summary>
    public static class SimpleURPHelper
    {
        /// <summary>
        /// Fixes the main URP depth buffer issue and configures essential pixel-perfect settings.
        /// </summary>
        public static void FixDepthBufferIssue(UnityEngine.Camera camera)
        {
            if (camera == null) return;
            
            // PRIMARY FIX: Remove render texture to avoid depth buffer issues
            camera.targetTexture = null;
            
            // Configure basic camera settings for pixel art
            camera.orthographic = true;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.clearFlags = CameraClearFlags.SolidColor;
            
            // Configure URP camera data with only essential settings
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                ConfigureEssentialURPSettings(cameraData);
            }
        }
        
        /// <summary>
        /// Configures only the essential URP settings that are guaranteed to exist.
        /// </summary>
        private static void ConfigureEssentialURPSettings(UniversalAdditionalCameraData cameraData)
        {
            // Essential settings that exist in all URP versions
            cameraData.renderType = CameraRenderType.Base;
            cameraData.renderPostProcessing = false;
            cameraData.antialiasing = AntialiasingMode.None;
            cameraData.antialiasingQuality = AntialiasingQuality.Low;
            
            // Clear camera stack to avoid overlay issues
            cameraData.cameraStack.Clear();
        }
        
        /// <summary>
        /// Creates a render texture with proper depth buffer for URP (if needed for special effects).
        /// </summary>
        public static RenderTexture CreateSafeRenderTexture(int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 24); // 24-bit depth buffer
            rt.filterMode = FilterMode.Point; // Critical for pixel art
            rt.format = RenderTextureFormat.ARGB32;
            rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
            rt.antiAliasing = 1; // No anti-aliasing
            rt.useMipMap = false;
            rt.autoGenerateMips = false;
            rt.wrapMode = TextureWrapMode.Clamp;
            
            return rt;
        }
        
        /// <summary>
        /// Quick validation to check if the depth buffer issue is resolved.
        /// </summary>
        public static bool IsDepthBufferIssueFixed(UnityEngine.Camera camera)
        {
            if (camera == null) return false;
            
            // Main check: camera should not have a render texture
            if (camera.targetTexture != null)
            {
                Debug.LogWarning($"Camera '{camera.name}' still has a render texture assigned. This may cause depth buffer issues.");
                return false;
            }
            
            // Check if camera is properly configured
            if (!camera.orthographic)
            {
                Debug.LogWarning($"Camera '{camera.name}' should be orthographic for pixel-perfect rendering.");
            }
            
            return true;
        }
        
        /// <summary>
        /// Logs the current camera configuration for debugging.
        /// </summary>
        public static void LogCameraConfiguration(UnityEngine.Camera camera)
        {
            if (camera == null) return;
            
            Debug.Log($"=== Camera Configuration: {camera.name} ===");
            Debug.Log($"Target Texture: {(camera.targetTexture == null ? "None (Good)" : "Assigned (Potential Issue)")}");
            Debug.Log($"Orthographic: {camera.orthographic}");
            Debug.Log($"Allow HDR: {camera.allowHDR}");
            Debug.Log($"Allow MSAA: {camera.allowMSAA}");
            Debug.Log($"Clear Flags: {camera.clearFlags}");
            
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                Debug.Log($"Render Type: {cameraData.renderType}");
                Debug.Log($"Anti-aliasing: {cameraData.antialiasing}");
                Debug.Log($"Post-processing: {cameraData.renderPostProcessing}");
                Debug.Log($"Camera Stack Count: {cameraData.cameraStack.Count}");
            }
        }
        
        /// <summary>
        /// Quick setup method that applies all essential fixes.
        /// </summary>
        public static void QuickPixelPerfectSetup(UnityEngine.Camera camera)
        {
            FixDepthBufferIssue(camera);
            
            // Additional pixel-perfect settings
            camera.nearClipPlane = -10f;
            camera.farClipPlane = 10f;
            
            Debug.Log($"Applied quick pixel-perfect setup to camera: {camera.name}");
        }
    }
}
