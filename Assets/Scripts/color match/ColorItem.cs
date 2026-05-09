using UnityEngine;

/// <summary>
/// COLOR ITEM
/// Attach to each colored object the player can pick up and carry.
///
/// Setup:
///   - Add Component → ColorItem
///   - Set colorID (1=Blue, 2=Yellow, 3=Green, 4=Purple)
///   - Add Component → Rigidbody
///   - Set Layer → "Carriable"
///   - Collider is required (any type)
/// </summary>
public class ColorItem : MonoBehaviour
{
    [Header("Color Identity")]
    [Tooltip("1=Blue  2=Yellow  3=Green  4=Purple")]
    public int colorID = 1;

    // Set to true when successfully placed on a matching table
    [HideInInspector] public bool isPlaced = false;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        ApplySolidColor();
    }

    // Apply the flat solid color on start
    void ApplySolidColor()
    {
        if (rend == null) return;
        if (colorID < 1 || colorID >= ColorData.SolidColor.Length) return;
        rend.material.color = ColorData.SolidColor[colorID];
    }

    // Called by ItemTable when ID matches — makes this item glow
    public void Glow()
    {
        if (rend == null) return;
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", ColorData.GlowColor[colorID]);
    }
}