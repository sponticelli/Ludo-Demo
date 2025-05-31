# URP Render Texture Depth Buffer Troubleshooting

## The Error

```
In the render graph API, the output Render Texture must have a depth buffer. When you select a Render Texture in any camera's Output Texture property, the Depth Stencil Format property of the texture must be set to a value other than None.
```

## What This Means

This error occurs in Unity 6 with URP when:
1. A camera is trying to render to a Render Texture
2. That Render Texture doesn't have a depth buffer configured
3. URP's Render Graph API requires depth buffers for proper rendering

## How Our System Fixes It

### 1. **Automatic Camera Configuration**
Our `URPPixelPerfectHelper` automatically:
- Sets `camera.targetTexture = null` to avoid render texture issues
- Configures URP camera data for pixel-perfect rendering
- Disables unnecessary features that can cause conflicts

### 2. **Proper Render Texture Creation**
If you need render textures for special effects, use:
```csharp
RenderTexture rt = URPPixelPerfectHelper.CreatePixelPerfectRenderTexture(128, 128);
```

### 3. **Validation and Debugging**
The system includes validation to catch common issues:
```csharp
URPPixelPerfectHelper.ValidateURPSettings(camera);
URPPixelPerfectHelper.ValidateURPAsset();
```

## Manual Fixes (If Needed)

### Fix 1: Remove Render Texture from Camera
```csharp
Camera.main.targetTexture = null;
```

### Fix 2: Configure Render Texture Properly
If you must use a render texture:
```csharp
RenderTexture rt = new RenderTexture(width, height, 24); // 24-bit depth
rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
rt.filterMode = FilterMode.Point; // For pixel art
```

### Fix 3: URP Camera Settings
```csharp
var cameraData = camera.GetUniversalAdditionalCameraData();
cameraData.renderType = CameraRenderType.Base;
cameraData.antialiasing = AntialiasingMode.None;
cameraData.renderPostProcessing = false;
```

## Project Settings for Pixel Art

### Graphics Settings
- **Scriptable Render Pipeline**: Use your URP asset
- **Camera-relative Culling**: Enabled

### Quality Settings
- **Anti Aliasing**: Disabled
- **Anisotropic Textures**: Disabled
- **Texture Quality**: Full Res

### URP Asset Settings
- **MSAA**: Disabled (or 1x)
- **HDR**: Disabled for pixel art
- **Render Scale**: 1.0

## Common Scenarios

### Scenario 1: Main Camera Error
**Problem**: Main camera shows depth buffer error
**Solution**: Our system automatically sets `targetTexture = null`

### Scenario 2: Multiple Cameras
**Problem**: Overlay cameras causing issues
**Solution**: Use only one base camera for pixel art

### Scenario 3: Post-Processing
**Problem**: Post-processing effects requiring depth
**Solution**: Disable post-processing for pixel art

### Scenario 4: Custom Render Textures
**Problem**: Custom effects need render textures
**Solution**: Use `URPPixelPerfectHelper.CreatePixelPerfectRenderTexture()`

## Verification Steps

1. **Check Camera Settings**:
   ```csharp
   Debug.Log($"Target Texture: {Camera.main.targetTexture}"); // Should be null
   Debug.Log($"Orthographic: {Camera.main.orthographic}"); // Should be true
   ```

2. **Check URP Settings**:
   ```csharp
   var cameraData = Camera.main.GetUniversalAdditionalCameraData();
   Debug.Log($"Render Type: {cameraData.renderType}"); // Should be Base
   Debug.Log($"Anti-aliasing: {cameraData.antialiasing}"); // Should be None
   ```

3. **Validate Setup**:
   ```csharp
   // Use our validation
   URPPixelPerfectHelper.ValidateURPSettings(Camera.main);
   ```

## Prevention

### Use Our Setup Scripts
1. **Automatic Setup**: Use `PixelGameSetup` component
2. **Editor Window**: Use `Bloomblat > Pixel Perfect Setup`
3. **Manual Setup**: Call `URPPixelPerfectHelper.ConfigurePixelPerfectCamera(camera)`

### Best Practices
- Always use our helper scripts for camera setup
- Avoid manually setting render textures on main camera
- Use validation methods to catch issues early
- Keep URP settings simple for pixel art

## If Error Persists

1. **Clear Camera Target Texture**:
   ```csharp
   Camera.main.targetTexture = null;
   ```

2. **Reset URP Camera Data**:
   ```csharp
   var cameraData = Camera.main.GetUniversalAdditionalCameraData();
   cameraData.renderType = CameraRenderType.Base;
   ```

3. **Check URP Asset**:
   - Ensure URP asset is assigned in Graphics settings
   - Verify URP asset settings are appropriate for pixel art

4. **Restart Unity**:
   - Sometimes URP needs a restart to apply changes properly

## Contact

If you continue experiencing issues:
1. Check Unity Console for additional error details
2. Verify all components are properly configured
3. Use our validation tools to identify specific problems
4. Ensure you're using Unity 6 with URP 17.0.3+

The system is designed to automatically handle these URP complexities, so most users shouldn't encounter this error when using our setup scripts.
