using UnityEngine;

namespace Bloomblat.Core
{
    /// <summary>
    /// Provides pixel-perfect rendering data and utilities for the 128x128 pixel art game.
    /// Centralizes access to game resolution, pixels per unit, and conversion methods.
    /// Can be injected using UnityInject's [Inject] attribute.
    /// </summary>
    [CreateAssetMenu(fileName = "PixelDataProvider", menuName = "Bloomblat/Settings/PixelDataProvider")]
    public class PixelDataProvider : ScriptableObject, IPixelDataProvider
    {
        [Header("Pixel Perfect Settings")]
        [SerializeField] private int gameWidth = 128;
        [SerializeField] private int gameHeight = 128;
        [SerializeField] private int pixelsPerUnit = 16;
        
        
        /// <inheritdoc />
        public int GameWidth => gameWidth;
        
        /// <inheritdoc />
        public int GameHeight => gameHeight;
        
        /// <inheritdoc />
        public Vector2Int GameResolution => new Vector2Int(gameWidth, gameHeight);
        
        /// <inheritdoc />
        public Vector2 GameResolutionFloat => new Vector2(gameWidth, gameHeight);
        
        /// <inheritdoc />
        public int PixelsPerUnit => pixelsPerUnit;
        
        /// <inheritdoc />
        public float PixelsPerUnitFloat => pixelsPerUnit;
        
        /// <inheritdoc />
        public float UnitsToPixels(float units)
        {
            return units * pixelsPerUnit;
        }
        
        /// <inheritdoc />
        public float PixelsToUnits(float pixels)
        {
            return pixels / pixelsPerUnit;
        }
        
        /// <inheritdoc />
        public Vector3 SnapToPixel(Vector3 position)
        {
            return new Vector3(
                SnapToPixel(position.x),
                SnapToPixel(position.y),
                position.z
            );
        }
        
        /// <inheritdoc />
        public Vector2 SnapToPixel(Vector2 position)
        {
            return new Vector2(
                SnapToPixel(position.x),
                SnapToPixel(position.y)
            );
        }
        
        /// <inheritdoc />
        public float SnapToPixel(float value)
        {
            return Mathf.Round(value * pixelsPerUnit) / pixelsPerUnit;
        }
    }
}