using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// RAYCAST INTERACTOR
/// Attach ONCE to the Player GameObject.
///
/// Controls:
///   LMB hold    → pick up and carry item
///   LMB release → drop item (place it on a table)
///   RMB click   → throw item forward
///
/// Crosshair dot:
///   White  = nothing targeted
///   Yellow = hovering over a carriable item
///   Green  = currently carrying an item
///
/// Items must be on the "Carriable" layer and have a Rigidbody.
/// </summary>
public class RaycastInteractor : MonoBehaviour
{
    [Header("Raycast")]
    public float interactRange = 3f;
    public LayerMask carriableMask;         // Set to "Carriable" layer

    [Header("Carry")]
    public float carryDistance = 2.5f;    // How far in front of camera item floats
    public float carrySmoothing = 15f;     // Higher = snappier movement

    [Header("Throw")]
    public float throwForce = 12f;

    [Header("Crosshair Dot")]
    public float dotSize = 10f;
    public Color colorNormal = Color.white;
    public Color colorHover = Color.yellow;
    public Color colorCarry = new Color(0.2f, 1f, 0.4f);

    // ── Private ──────────────────────────────────────────────────────────────
    private Camera cam;
    private Rigidbody carriedRb;
    private GameObject carriedObj;
    private bool isCarrying;
    private bool isHovering;
    private Texture2D dotTex;

    void Start()
    {
        cam = Camera.main;
        dotTex = MakeDotTexture(32);
    }

    void Update()
    {
        if (isCarrying) HandleCarry();
        else HandleScan();
    }

    // ── SCAN ─────────────────────────────────────────────────────────────────
    void HandleScan()
    {
        // Cast a ray from the exact center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        isHovering = Physics.Raycast(ray, out RaycastHit hit, interactRange, carriableMask);

        if (!isHovering) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Don't pick up items that are already locked to a table
            ColorItem ci = hit.collider.GetComponent<ColorItem>();
            if (ci != null && ci.isPlaced) return;

            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null) PickUp(rb);
        }
    }

    // ── CARRY ────────────────────────────────────────────────────────────────
    void HandleCarry()
    {
        if (carriedObj == null) { ResetCarry(); return; }

        // Float item in front of camera using velocity
        Vector3 target = cam.transform.position + cam.transform.forward * carryDistance;
        carriedRb.linearVelocity = (target - carriedObj.transform.position) * carrySmoothing;
        carriedRb.angularVelocity = Vector3.zero;

        // LMB release = drop (item falls onto table below)
        if (Mouse.current.leftButton.wasReleasedThisFrame) { Drop(); return; }

        // RMB = throw
        if (Mouse.current.rightButton.wasPressedThisFrame) { Throw(); return; }
    }

    // ── ACTIONS ──────────────────────────────────────────────────────────────
    void PickUp(Rigidbody rb)
    {
        carriedRb = rb;
        carriedObj = rb.gameObject;
        carriedRb.useGravity = false;
        carriedRb.linearDamping = 10f;
        isCarrying = true;
    }

    void Drop()
    {
        carriedRb.useGravity = true;
        carriedRb.linearDamping = 1f;
        carriedRb.linearVelocity = Vector3.zero; // Let gravity pull it down naturally
        ResetCarry();
    }

    void Throw()
    {
        carriedRb.useGravity = true;
        carriedRb.linearDamping = 1f;
        carriedRb.linearVelocity = cam.transform.forward * throwForce;
        ResetCarry();
    }

    void ResetCarry()
    {
        carriedRb = null;
        carriedObj = null;
        isCarrying = false;
    }

    // ── PUBLIC (called by ItemTable on snap) ──────────────────────────────────
    public GameObject GetCarriedObject() => carriedObj;

    public void ForceRelease()
    {
        if (carriedRb != null)
        {
            carriedRb.useGravity = true;
            carriedRb.linearDamping = 1f;
            carriedRb.linearVelocity = Vector3.zero;
        }
        ResetCarry();
    }

    // ── CROSSHAIR DOT ─────────────────────────────────────────────────────────
    void OnGUI()
    {
        Color c = isCarrying ? colorCarry : isHovering ? colorHover : colorNormal;
        GUI.color = c;
        GUI.DrawTexture(
            new Rect(Screen.width / 2f - dotSize / 2f, Screen.height / 2f - dotSize / 2f, dotSize, dotSize),
            dotTex
        );
        GUI.color = Color.white;
    }

    Texture2D MakeDotTexture(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float ctr = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(ctr, ctr));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, d < ctr ? 1f : 0f));
            }
        tex.Apply();
        return tex;
    }
}