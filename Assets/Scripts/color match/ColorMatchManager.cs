using UnityEngine;
using System.Collections;

/// <summary>
/// COLOR MATCH MANAGER
/// Place on an empty GameObject called "GameManager".
/// Tracks how many tables are confirmed and opens the door when all match.
///
/// Setup:
///   - Create Empty → rename "GameManager" → Add Component → ColorMatchManager
///   - Drag your Door GameObject into the Door field
///   - Set Slide Direction (0,1,0)=Up | (1,0,0)=Right | (0,0,1)=Forward etc.
///   - Set Slide Distance and Duration
///   - Set Next Level Name (the exact name of the next scene in Build Settings)
/// </summary>
public class ColorMatchManager : MonoBehaviour
{
    public static ColorMatchManager Instance { get; private set; }

    [Header("Door")]
    [Tooltip("Drag the door GameObject here")]
    public GameObject door;

    [Tooltip("Direction the door slides\n(0,1,0)=Up | (0,-1,0)=Down | (1,0,0)=Right | (-1,0,0)=Left | (0,0,1)=Forward")]
    public Vector3 slideDirection = Vector3.up;

    [Tooltip("How many Unity units the door slides")]
    public float slideDistance = 3f;

    [Tooltip("Animation time in seconds")]
    public float slideDuration = 1.5f;

    [Header("Next Level")]
    [Tooltip("Exact scene name to load after door opens (must be added in Build Settings)")]
    public string nextLevelName = "";

    [Tooltip("Delay in seconds before loading next level (0 = no auto-load)")]
    public float nextLevelDelay = 0f;

    [Header("Puzzle")]
    [Tooltip("Total number of tables that need to be matched")]
    public int totalTables = 4;

    // ── Private ──────────────────────────────────────────────────────────────
    private int matchedCount = 0;
    private bool doorOpened = false;

    void Awake()
    {
        Instance = this;
    }

    // Called by ItemTable every time a correct item is confirmed
    public void RegisterMatch()
    {
        matchedCount++;
        Debug.Log($"[ColorMatch] Tables confirmed: {matchedCount}/{totalTables}");

        if (matchedCount >= totalTables && !doorOpened)
        {
            doorOpened = true;
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        if (door == null)
        {
            Debug.LogWarning("[ColorMatch] No door assigned in ColorMatchManager!");
            yield break;
        }

        Debug.Log("[ColorMatch] All tables matched — opening door!");

        Vector3 startPos = door.transform.position;
        Vector3 endPos = startPos + slideDirection.normalized * slideDistance;
        float elapsed = 0f;

        // Slide animation with smooth ease-in/out
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            door.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Snap to exact final position — door stays open permanently
        door.transform.position = endPos;
        Debug.Log("[ColorMatch] Door open! Puzzle complete.");

        // Optionally load next level after a delay
        if (!string.IsNullOrEmpty(nextLevelName) && nextLevelDelay > 0f)
        {
            yield return new WaitForSeconds(nextLevelDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        }
    }

    // Shows where the door will slide in Scene view (cyan arrow)
    void OnDrawGizmosSelected()
    {
        if (door == null) return;
        Gizmos.color = Color.cyan;
        Vector3 from = door.transform.position;
        Vector3 to = from + slideDirection.normalized * slideDistance;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(to, 0.15f);
    }
}