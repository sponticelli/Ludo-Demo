using UnityEngine;

namespace Bloomblat.Core
{
    /// <summary>
    /// Interface for providing pixel-perfect rendering data and utilities.
    /// Centralizes access to game resolution, pixels per unit, and conversion methods.
    /// </summary>
    public interface IPixelDataProvider
    {
        /// <summary>
        /// The fixed game width in pixels.
        /// </summary>
        int GameWidth { get; }
        
        /// <summary>
        /// The fixed game height in pixels.
        /// </summary>
        int GameHeight { get; }
        
        /// <summary>
        /// The game resolution as a Vector2Int.
        /// </summary>
        Vector2Int GameResolution { get; }
        
        /// <summary>
        /// The game resolution as a Vector2.
        /// </summary>
        Vector2 GameResolutionFloat { get; }
        
        /// <summary>
        /// The number of pixels per Unity unit used for sprites and assets.
        /// </summary>
        int PixelsPerUnit { get; }
        
        /// <summary>
        /// The number of pixels per Unity unit as a float.
        /// </summary>
        float PixelsPerUnitFloat { get; }
        
        /// <summary>
        /// Converts Unity units to pixels.
        /// </summary>
        /// <param name="units">The value in Unity units.</param>
        /// <returns>The value in pixels.</returns>
        float UnitsToPixels(float units);
        
        /// <summary>
        /// Converts pixels to Unity units.
        /// </summary>
        /// <param name="pixels">The value in pixels.</param>
        /// <returns>The value in Unity units.</returns>
        float PixelsToUnits(float pixels);
        
        /// <summary>
        /// Snaps a position to pixel boundaries.
        /// </summary>
        /// <param name="position">The position to snap.</param>
        /// <returns>The position snapped to pixel boundaries.</returns>
        Vector3 SnapToPixel(Vector3 position);
        
        /// <summary>
        /// Snaps a Vector2 position to pixel boundaries.
        /// </summary>
        /// <param name="position">The position to snap.</param>
        /// <returns>The position snapped to pixel boundaries.</returns>
        Vector2 SnapToPixel(Vector2 position);
        
        /// <summary>
        /// Snaps a float value to pixel boundaries.
        /// </summary>
        /// <param name="value">The value to snap.</param>
        /// <returns>The value snapped to pixel boundaries.</returns>
        float SnapToPixel(float value);
    }
}
