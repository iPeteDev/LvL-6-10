using UnityEngine;

/// <summary>
/// ITEM TABLE
/// Attach to each table in the scene.
/// The table detects when a ColorItem is placed on it using a trigger collider.
///
/// Setup:
///   - Add Component → ItemTable
///   - Set colorID to match the item it accepts (1=Blue 2=Yellow 3=Green 4=Purple)
///   - The table needs TWO colliders:
///       [1] BoxCollider (Is Trigger = FALSE) → the solid surface items rest on
///       [2] BoxCollider (Is Trigger = TRUE)  → the detection zone above the table
///           (Add a 2nd Box Collider component and check Is Trigger on that one only)
///           Size it to cover the top area of the table surface.
///
/// What happens:
///   - Item placed on table → trigger fires → checks colorID
///   - ID match  → both table AND item glow, item snaps and locks
///   - ID wrong  → item stays on table but nothing happens (player can pick it up again)
///   - All 4 tables matched → door opens
/// </summary>
public class ItemTable : MonoBehaviour
{
    [Header("Color Identity")]
    [Tooltip("1=Blue  2=Yellow  3=Green  4=Purple — must match the ColorItem it accepts")]
    public int colorID = 1;

    // Is this table already matched?
    [HideInInspector] public bool isMatched = false;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Show a dim version of the target color so the player knows what goes here
        if (rend != null)
            rend.material.color = ColorData.DimColor(colorID);
    }

    // ── TRIGGER DETECTION ────────────────────────────────────────────────────
    // Unity calls this when any collider enters the trigger zone on this table
    void OnTriggerEnter(Collider other)
    {
        if (isMatched) return; // already confirmed — ignore

        // Is the object that entered a ColorItem?
        ColorItem item = other.GetComponent<ColorItem>();
        if (item == null)  return; // not a color item, ignore
        if (item.isPlaced) return; // already locked to another table

        // ── CHECK COLOR ID ───────────────────────────────────────────────────
        if (item.colorID == colorID)
        {
            ConfirmMatch(item); // ✓ ID matches
        }
        else
        {
            // Wrong item — do nothing, player can pick it up and try again
            Debug.Log($"[ColorMatch] Wrong item on table {colorID}. Item ID = {item.colorID}");
        }
    }

    // ── CONFIRM MATCH ─────────────────────────────────────────────────────────
    void ConfirmMatch(ColorItem item)
    {
        isMatched    = true;
        item.isPlaced = true;

        // If item is still in the player's hand, force-release it first
        RaycastInteractor interactor = FindAnyObjectByType<RaycastInteractor>();
        if (interactor != null && interactor.GetCarriedObject() == item.gameObject)
            interactor.ForceRelease();

        // Snap item to center of the table
        item.transform.position = transform.position + Vector3.up * 0.5f;
        item.transform.rotation = Quaternion.identity;

        // Freeze item physics — stays on the table permanently
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic     = true;
        }

        // ── GLOW: both table and item light up ───────────────────────────────
        GlowTable();
        item.Glow();

        Debug.Log($"[ColorMatch] ✓ Table {colorID} confirmed! Item {item.colorID} matched.");

        // Tell the manager — if all 4 are confirmed, door opens
        ColorMatchManager.Instance?.RegisterMatch();
    }

    // Make the table itself glow
    void GlowTable()
    {
        if (rend == null) return;
        rend.material.color = ColorData.SolidColor[colorID]; // restore full solid color
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", ColorData.GlowColor[colorID]);
    }
}
