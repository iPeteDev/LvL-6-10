using UnityEngine;
using System.Collections;

/// <summary>
/// BASKETBALL SCORER
/// Attach to an empty GameObject inside the ring (ScoreTrigger).
/// Requires: Collider with Is Trigger = TRUE on this object.
/// Ball must be tagged "Ball".
///
/// This script handles ONLY scoring logic and gate opening.
/// The score display is handled separately by BackboardScoreUI.
/// </summary>
public class BasketballScorer : MonoBehaviour
{
    [Header("Scoring")]
    [Tooltip("Tag on the basketball")]
    public string ballTag = "Ball";

    [Tooltip("Score needed to open the gate")]
    public int scoreToWin = 10;

    [Header("Gate")]
    public GameObject gate;

    [Tooltip("(0,1,0)=Up | (1,0,0)=Right | (0,0,1)=Forward")]
    public Vector3 slideDirection = Vector3.up;
    public float slideDistance = 4f;
    public float slideDuration = 1.5f;

    [Header("UI Reference")]
    [Tooltip("Drag the Backboard GameObject that has BackboardScoreUI attached")]
    public BasketballScoreUI scoreUI;

    // ── Private ──────────────────────────────────────────────────────────────
    private int score = 0;
    private bool gameWon = false;

    void Start()
    {
        if (scoreUI != null)
            scoreUI.UpdateScore(score, scoreToWin);
    }

    // ── TRIGGER ───────────────────────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (gameWon) return;
        if (!other.CompareTag(ballTag)) return;

        // Only count ball moving DOWNWARD through the ring (top to bottom)
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && rb.linearVelocity.y >= 0f) return;

        AddScore();
    }

    // ── SCORE ─────────────────────────────────────────────────────────────────
    void AddScore()
    {
        score++;
        Debug.Log($"[Basketball] {score}/{scoreToWin}");

        if (scoreUI != null)
            scoreUI.UpdateScore(score, scoreToWin);

        if (score >= scoreToWin && !gameWon)
        {
            gameWon = true;

            if (scoreUI != null)
                scoreUI.ShowWin();

            StartCoroutine(OpenGate());
        }
    }

    // ── GATE ──────────────────────────────────────────────────────────────────
    IEnumerator OpenGate()
    {
        if (gate == null)
        {
            Debug.LogWarning("[Basketball] No gate assigned!");
            yield break;
        }

        Vector3 startPos = gate.transform.position;
        Vector3 endPos = startPos + slideDirection.normalized * slideDistance;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            gate.transform.position = Vector3.Lerp(startPos, endPos,
                Mathf.SmoothStep(0f, 1f, elapsed / slideDuration));
            yield return null;
        }

        gate.transform.position = endPos;
        Debug.Log("[Basketball] Gate open!");
    }

    void OnDrawGizmosSelected()
    {
        if (gate == null) return;
        Gizmos.color = Color.cyan;
        Vector3 from = gate.transform.position;
        Vector3 to = from + slideDirection.normalized * slideDistance;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(to, 0.15f);
    }
}