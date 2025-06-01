using UnityEngine;

namespace Bloomblat.Input
{
    /// <summary>
    /// Interface for input control managers that handle different input methods
    /// (mobile touch controls, keyboard, gamepad, etc.) in a unified way.
    /// </summary>
    public interface IControlsManager
    {
        /// <summary>
        /// Gets the current joystick/movement input as a normalized Vector2.
        /// Values range from -1 to 1 on both axes.
        /// </summary>
        Vector2 JoystickInput { get; }
        
        /// <summary>
        /// Gets the state of a specific action button by index.
        /// </summary>
        /// <param name="index">The button index (0-based)</param>
        /// <returns>True if the button is currently pressed</returns>
        bool GetButton(int index);
        
        /// <summary>
        /// Gets whether a button was pressed this frame.
        /// </summary>
        /// <param name="index">The button index (0-based)</param>
        /// <returns>True if the button was pressed this frame</returns>
        bool GetButtonDown(int index);
        
        /// <summary>
        /// Gets whether a button was released this frame.
        /// </summary>
        /// <param name="index">The button index (0-based)</param>
        /// <returns>True if the button was released this frame</returns>
        bool GetButtonUp(int index);
        
        /// <summary>
        /// Gets the horizontal movement input (-1 to 1).
        /// </summary>
        float Horizontal { get; }
        
        /// <summary>
        /// Gets the vertical movement input (-1 to 1).
        /// </summary>
        float Vertical { get; }
        
        /// <summary>
        /// Indicates whether this controls manager shows visual controls (like virtual joysticks).
        /// </summary>
        bool HasVisualControls { get; }
        
        /// <summary>
        /// Indicates whether this controls manager is currently active and handling input.
        /// </summary>
        bool IsActive { get; }
        
        /// <summary>
        /// Initializes the controls manager. Called after dependency injection.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Updates the input state. Called each frame by the system.
        /// </summary>
        void UpdateInput();
        
        /// <summary>
        /// Enables or disables the controls manager.
        /// </summary>
        /// <param name="enabled">Whether to enable the controls</param>
        void SetEnabled(bool enabled);
        
        /// <summary>
        /// Cleans up resources when the controls manager is no longer needed.
        /// </summary>
        void Cleanup();
    }
}
