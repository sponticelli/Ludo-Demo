# Pixel Perfect 128x128 Game Setup Guide

This guide explains how to set up a pixel-perfect 128x128 game in Unity 6 with cross-platform support (PC, Mobile, WebGL).

## Overview

The system consists of several components working together:
- **PixelPerfectCameraController**: Main camera management for 128x128 resolution
- **AspectRatioHandler**: Manages different screen aspect ratios and letterboxing
- **MobileControlsManager**: Virtual controls for mobile devices
- **VirtualJoystick**: Touch-based joystick input
- **PixelPerfectCanvasScaler**: UI scaling that works with pixel-perfect rendering

## Quick Setup

1. **Automatic Setup**: Add the `PixelGameSetup` component to any GameObject and click "Setup Pixel Perfect Scene" in the context menu.

2. **Manual Setup**: Follow the detailed steps below.

## Detailed Setup Instructions

### 1. Camera Setup

#### Main Camera Configuration:
```csharp
// Camera settings
Camera.main.orthographic = true;
Camera.main.backgroundColor = Color.black;
Camera.main.clearFlags = CameraClearFlags.SolidColor;
```

#### Add Required Components:
1. **PixelPerfectCamera** (Unity's built-in component)
2. **PixelPerfectCameraController** (our custom component)
3. **AspectRatioHandler** (automatically added)

#### Recommended Settings:
- **Assets PPU**: 16 (pixels per unit)
- **Reference Resolution**: 128x128
- **Upscale Render Texture**: false (for crisp pixels)
- **Pixel Snapping**: true
- **Crop Frame X/Y**: false
- **Stretch Fill**: false

### 2. Mobile Controls Setup

For mobile devices, the system automatically:
- Reserves 200 pixels at the bottom for controls
- Positions the 128x128 game area at the top
- Creates virtual joystick and action buttons

#### Mobile Controls Components:
1. Create a Canvas with `MobileControlsManager`
2. Virtual joystick is created automatically
3. Action buttons are positioned on the right side

### 3. UI Canvas Setup

#### Game UI Canvas:
```csharp
Canvas canvas = gameObject.AddComponent<Canvas>();
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
canvas.sortingOrder = 10;
```

#### Add Components:
1. **CanvasScaler** (Unity's built-in)
2. **PixelPerfectCanvasScaler** (our custom component)
3. **GraphicRaycaster** (for UI interactions)

### 4. Platform-Specific Considerations

#### PC/WebGL:
- Full-screen letterboxing
- Maintains 1:1 aspect ratio
- Keyboard/mouse input

#### Mobile (Portrait):
- Game area at top (128x128)
- Virtual controls at bottom
- Touch input handling

#### Mobile (Landscape):
- Automatic orientation handling
- Responsive control positioning

## Component Details

### PixelPerfectCameraController

Main component that manages the camera and ensures pixel-perfect rendering.

**Key Features:**
- Automatic aspect ratio handling
- Mobile/desktop detection
- Viewport management
- World coordinate conversion

**Public Methods:**
```csharp
Vector3 ScreenToWorldPoint(Vector2 screenPoint)
Bounds GetWorldBounds()
void UpdateCameraSettings()
```

### AspectRatioHandler

Manages different screen aspect ratios and provides letterboxing.

**Key Features:**
- Letterbox calculation
- Aspect ratio validation
- Screen point validation

**Public Methods:**
```csharp
bool IsPointInGameArea(Vector2 screenPoint)
Vector2 ScreenToGameAreaNormalized(Vector2 screenPoint)
Vector4 GetLetterboxDimensions()
```

### MobileControlsManager

Handles virtual controls for mobile devices.

**Key Features:**
- Automatic mobile detection
- Virtual joystick creation
- Action button management
- Input value tracking

**Public Properties:**
```csharp
Vector2 JoystickInput { get; }
bool GetButton(int index)
```

### VirtualJoystick

Touch-based joystick implementation.

**Key Features:**
- Normalized input values (-1 to 1)
- Dead zone support
- 4-way and 8-way input modes
- Smooth return to center

**Public Methods:**
```csharp
Vector2 GetInputVector()
Vector2 GetDirectionalInput()
Vector2 Get8WayInput()
float GetAngle()
float GetMagnitude()
```

## Best Practices

### Sprite Import Settings:
- **Texture Type**: Sprite (2D and UI)
- **Sprite Mode**: Single
- **Pixels Per Unit**: 16 (or your chosen PPU)
- **Filter Mode**: Point (no filter)
- **Compression**: None
- **Generate Mip Maps**: false

### Art Guidelines:
- Create sprites at exact pixel dimensions
- Use consistent pixel-per-unit ratio
- Avoid sub-pixel positioning
- Design for 128x128 viewport

### Performance Tips:
- Use sprite atlases for better batching
- Minimize overdraw in UI
- Use object pooling for dynamic sprites
- Optimize texture sizes

## Troubleshooting

### Common Issues:

1. **Blurry Sprites**:
   - Check Filter Mode is set to Point
   - Verify Pixels Per Unit matches camera settings
   - Ensure sprites are positioned on pixel boundaries

2. **UI Scaling Issues**:
   - Use PixelPerfectCanvasScaler instead of regular CanvasScaler
   - Check reference resolution matches game resolution
   - Verify Canvas render mode is correct

3. **Mobile Controls Not Showing**:
   - Ensure platform detection is working
   - Check Canvas sorting order
   - Verify EventSystem exists in scene

4. **Aspect Ratio Problems**:
   - Check camera viewport settings
   - Verify letterboxing is working
   - Test on different screen resolutions

### Debug Information:

Enable debug info in PixelPerfectCameraController to see:
- Current screen resolution
- Game area size and position
- Aspect ratio information
- Letterbox dimensions

## Testing

Test your setup on:
- Different PC resolutions
- Various mobile devices
- WebGL in different browsers
- Portrait and landscape orientations

## Unity Packages Required

Ensure these packages are installed:
- **Universal RP** (com.unity.render-pipelines.universal)
- **2D Feature** (com.unity.feature.2d)
- **2D Pixel Perfect** (com.unity.2d.pixel-perfect)
- **Input System** (com.unity.inputsystem) - optional but recommended

## Example Scene Hierarchy

```
Scene
├── Main Camera (PixelPerfectCameraController, PixelPerfectCamera)
├── Game UI (Canvas with PixelPerfectCanvasScaler)
├── Mobile Controls (Canvas with MobileControlsManager)
├── EventSystem
└── Game Objects
    ├── Player
    ├── Environment
    └── Effects
```

This setup ensures your 128x128 pixel art game will look crisp and consistent across all target platforms while providing appropriate controls for each platform type.

## Additional Resources

### Recommended Unity Settings:

**Project Settings > Graphics:**
- Scriptable Render Pipeline: UniversalRP asset
- Camera-relative Culling: enabled

**Project Settings > Quality:**
- Anti Aliasing: Disabled (for pixel art)
- Anisotropic Textures: Disabled
- Texture Quality: Full Res

**Project Settings > Player:**
- Default Orientation: Portrait (for mobile)
- Allowed Orientations: Portrait, Portrait Upside Down (mobile)

### Performance Optimization:

1. **Sprite Atlas Usage:**
   ```csharp
   // Create sprite atlases for better batching
   // Group sprites by usage frequency
   // Use compression appropriate for pixel art
   ```

2. **Camera Optimization:**
   ```csharp
   // Set appropriate culling distances
   camera.farClipPlane = 10f;
   camera.nearClipPlane = -10f;
   ```

3. **UI Optimization:**
   ```csharp
   // Use Canvas Groups for batch UI updates
   // Minimize Layout Group usage
   // Cache UI references
   ```
