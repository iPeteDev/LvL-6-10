using UnityEngine;

/// <summary>
/// SHARED COLOR DATA
/// Static utility — do NOT attach to any GameObject.
/// Single source of truth for all color IDs used across all scripts.
///
/// ID 1 = Blue
/// ID 2 = Yellow
/// ID 3 = Green
/// ID 4 = Purple
/// </summary>
public static class ColorData
{
    // Base solid colors (applied on Start)
    public static readonly Color[] SolidColor = new Color[]
    {
        Color.white,                          // 0 = unused (IDs start at 1)
        new Color(0.10f, 0.35f, 1.00f),      // 1 = Blue
        new Color(1.00f, 0.90f, 0.05f),      // 2 = Yellow
        new Color(0.10f, 0.85f, 0.20f),      // 3 = Green
        new Color(0.65f, 0.05f, 1.00f),      // 4 = Purple
    };

    // Glow/emission colors (applied on match — brighter version of solid)
    public static readonly Color[] GlowColor = new Color[]
    {
        Color.white,                          // 0 = unused
        new Color(0.20f, 0.60f, 4.00f),      // 1 = Blue glow
        new Color(4.00f, 3.50f, 0.10f),      // 2 = Yellow glow
        new Color(0.20f, 4.00f, 0.50f),      // 3 = Green glow
        new Color(2.50f, 0.10f, 4.00f),      // 4 = Purple glow
    };

    // Dimmed color shown on tables before they are matched
    public static Color DimColor(int id)
    {
        if (id < 1 || id >= SolidColor.Length) return Color.grey;
        Color c = SolidColor[id] * 0.25f;
        c.a = 1f;
        return c;
    }
}