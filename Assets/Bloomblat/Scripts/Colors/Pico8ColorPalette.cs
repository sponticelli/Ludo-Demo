using UnityEngine;

namespace Bloomblat.Colors
{

    [CreateAssetMenu(fileName = "Pico8Palette", menuName = "Bloomblat/Palettes/Pico-8 Palette")]
    public class Pico8ColorPalette : ScriptableObject
    {
        [Header("PICO-8 16 Color Palette")] public Color[] colors = new Color[16]
        {
            new Color(0.0f, 0.0f, 0.0f, 1.0f), // 0 - Black
            new Color(0.114f, 0.169f, 0.325f, 1.0f), // 1 - Dark Blue
            new Color(0.494f, 0.145f, 0.325f, 1.0f), // 2 - Dark Purple
            new Color(0.0f, 0.529f, 0.318f, 1.0f), // 3 - Dark Green
            new Color(0.671f, 0.322f, 0.212f, 1.0f), // 4 - Brown
            new Color(0.373f, 0.341f, 0.310f, 1.0f), // 5 - Dark Grey
            new Color(0.761f, 0.765f, 0.780f, 1.0f), // 6 - Light Grey
            new Color(1.0f, 0.945f, 0.910f, 1.0f), // 7 - White
            new Color(1.0f, 0.0f, 0.302f, 1.0f), // 8 - Red
            new Color(1.0f, 0.639f, 0.0f, 1.0f), // 9 - Orange
            new Color(1.0f, 0.925f, 0.153f, 1.0f), // 10 - Yellow
            new Color(0.0f, 0.894f, 0.212f, 1.0f), // 11 - Green
            new Color(0.161f, 0.678f, 1.0f, 1.0f), // 12 - Blue
            new Color(0.514f, 0.463f, 0.612f, 1.0f), // 13 - Indigo
            new Color(1.0f, 0.467f, 0.659f, 1.0f), // 14 - Pink
            new Color(1.0f, 0.800f, 0.667f, 1.0f) // 15 - Peach
        };

        public Color GetColor(int index)
        {
            if (index >= 0 && index < colors.Length)
                return colors[index];
            return Color.magenta; // Error color
        }

        public static Color Black => new Color(0.0f, 0.0f, 0.0f, 1.0f);
        public static Color DarkBlue => new Color(0.114f, 0.169f, 0.325f, 1.0f);
        public static Color DarkPurple => new Color(0.494f, 0.145f, 0.325f, 1.0f);
        public static Color DarkGreen => new Color(0.0f, 0.529f, 0.318f, 1.0f);
        public static Color Brown => new Color(0.671f, 0.322f, 0.212f, 1.0f);
        public static Color DarkGrey => new Color(0.373f, 0.341f, 0.310f, 1.0f);
        public static Color LightGrey => new Color(0.761f, 0.765f, 0.780f, 1.0f);
        public static Color White => new Color(1.0f, 0.945f, 0.910f, 1.0f);
        public static Color Red => new Color(1.0f, 0.0f, 0.302f, 1.0f);
        public static Color Orange => new Color(1.0f, 0.639f, 0.0f, 1.0f);
        public static Color Yellow => new Color(1.0f, 0.925f, 0.153f, 1.0f);
        public static Color Green => new Color(0.0f, 0.894f, 0.212f, 1.0f);
        public static Color Blue => new Color(0.161f, 0.678f, 1.0f, 1.0f);
        public static Color Indigo => new Color(0.514f, 0.463f, 0.612f, 1.0f);
        public static Color Pink => new Color(1.0f, 0.467f, 0.659f, 1.0f);
        public static Color Peach => new Color(1.0f, 0.800f, 0.667f, 1.0f);
    }
}