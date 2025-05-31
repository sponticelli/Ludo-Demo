using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Bloomblat.Camera
{
    /// <summary>
    /// Helper class to configure URP settings for pixel-perfect rendering.
    /// Addresses common URP issues like render texture depth buffer requirements.
    /// </summary>
    public static class URPPixelPerfectHelper
    {
        /// <summary>
        /// Configures a camera for optimal pixel-perfect rendering with URP.
        /// </summary>
        public static void ConfigurePixelPerfectCamera(UnityEngine.Camera camera)
        {
            if (camera == null) return;
            
            // Ensure camera doesn't use a render texture to avoid depth buffer issues
            camera.targetTexture = null;
            
            // Get URP camera data
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                ConfigureURPCameraData(cameraData);
            }
            
            // Configure basic camera settings
            ConfigureBasicCameraSettings(camera);
        }
        
        /// <summary>
        /// Configures URP-specific camera data for pixel-perfect rendering.
        /// Only uses properties that are guaranteed to exist in Unity 6 URP.
        /// </summary>
        private static void ConfigureURPCameraData(UniversalAdditionalCameraData cameraData)
        {
            // Set as base camera
            cameraData.renderType = CameraRenderType.Base;

            // Disable post-processing (important for pixel art)
            cameraData.renderPostProcessing = false;

            // Disable anti-aliasing (critical for pixel art)
            cameraData.antialiasing = AntialiasingMode.None;
            cameraData.antialiasingQuality = AntialiasingQuality.Low;

            // Set camera stack settings
            cameraData.cameraStack.Clear(); // Remove any overlay cameras

            // Configure additional settings safely
            try
            {
                // These properties might not exist in all URP versions
                cameraData.requiresColorOption = CameraOverrideOption.Off;
                cameraData.requiresDepthOption = CameraOverrideOption.Off;
            }
            catch (System.Exception)
            {
                // Ignore if properties don't exist
            }
        }
        
        /// <summary>
        /// Configures basic camera settings for pixel-perfect rendering.
        /// </summary>
        private static void ConfigureBasicCameraSettings(UnityEngine.Camera camera)
        {
            // Ensure orthographic projection
            camera.orthographic = true;
            
            // Set appropriate clipping planes
            camera.nearClipPlane = -10f;
            camera.farClipPlane = 10f;
            
            // Disable HDR
            camera.allowHDR = false;
            
            // Disable MSAA
            camera.allowMSAA = false;
            
            // Set clear flags
            camera.clearFlags = CameraClearFlags.SolidColor;
        }
        
        /// <summary>
        /// Creates a render texture with proper depth buffer settings for URP.
        /// Use this if you need a render texture for special effects.
        /// </summary>
        public static RenderTexture CreatePixelPerfectRenderTexture(int width, int height)
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
        /// Validates URP settings for pixel-perfect rendering.
        /// </summary>
        public static bool ValidateURPSettings(UnityEngine.Camera camera)
        {
            if (camera == null) return false;
            
            bool isValid = true;
            
            // Check basic camera settings
            if (!camera.orthographic)
            {
                Debug.LogWarning($"Camera '{camera.name}' should be orthographic for pixel-perfect rendering.");
                isValid = false;
            }
            
            if (camera.allowHDR)
            {
                Debug.LogWarning($"Camera '{camera.name}' has HDR enabled. Disable for pixel-perfect rendering.");
                isValid = false;
            }
            
            if (camera.allowMSAA)
            {
                Debug.LogWarning($"Camera '{camera.name}' has MSAA enabled. Disable for pixel-perfect rendering.");
                isValid = false;
            }
            
            // Check URP settings
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData != null)
            {
                if (cameraData.antialiasing != AntialiasingMode.None)
                {
                    Debug.LogWarning($"Camera '{camera.name}' has anti-aliasing enabled. Disable for pixel-perfect rendering.");
                    isValid = false;
                }
                
                if (cameraData.renderPostProcessing)
                {
                    Debug.LogWarning($"Camera '{camera.name}' has post-processing enabled. Consider disabling for pixel-perfect rendering.");
                }

                // Note: renderScale property doesn't exist in Unity 6 URP
                // The render scale is controlled by the URP asset instead
            }
            
            // Check for render texture issues
            if (camera.targetTexture != null)
            {
                var rt = camera.targetTexture;
                if (rt.depthStencilFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
                {
                    Debug.LogError($"Camera '{camera.name}' uses a render texture without depth buffer. This will cause URP errors.");
                    isValid = false;
                }
                
                if (rt.filterMode != FilterMode.Point)
                {
                    Debug.LogWarning($"Camera '{camera.name}' render texture should use Point filtering for pixel-perfect rendering.");
                }
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Fixes common URP render texture depth buffer issues.
        /// </summary>
        public static void FixRenderTextureDepthBuffer(RenderTexture renderTexture)
        {
            if (renderTexture == null) return;
            
            // If the render texture doesn't have a depth buffer, recreate it with one
            if (renderTexture.depthStencilFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None)
            {
                Debug.LogWarning("Fixing render texture depth buffer for URP compatibility.");
                
                // Store original settings
                int width = renderTexture.width;
                int height = renderTexture.height;
                RenderTextureFormat format = renderTexture.format;
                FilterMode filterMode = renderTexture.filterMode;
                
                // Release the old texture
                renderTexture.Release();
                
                // Recreate with depth buffer
                renderTexture.width = width;
                renderTexture.height = height;
                renderTexture.format = format;
                renderTexture.filterMode = filterMode;
                renderTexture.depth = 24; // Add 24-bit depth buffer
                renderTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
                
                // Recreate the texture
                renderTexture.Create();
            }
        }
        
        /// <summary>
        /// Gets the current URP asset and validates it for pixel-perfect rendering.
        /// </summary>
        public static bool ValidateURPAsset()
        {
            var urpAsset = UniversalRenderPipeline.asset;
            if (urpAsset == null)
            {
                Debug.LogError("No URP asset found. Make sure Universal Render Pipeline is properly configured.");
                return false;
            }
            
            bool isValid = true;
            
            // Check MSAA settings
            if (urpAsset.msaaSampleCount > 1)
            {
                Debug.LogWarning("URP Asset has MSAA enabled. Consider disabling for pixel-perfect rendering.");
            }
            
            // Check HDR settings
            if (urpAsset.supportsHDR)
            {
                Debug.LogWarning("URP Asset has HDR enabled. Consider disabling for pixel-perfect rendering.");
            }
            
            return isValid;
        }
    }
}
