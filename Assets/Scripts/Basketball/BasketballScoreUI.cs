using UnityEngine;
using TMPro;

/// <summary>
/// BASKETBALL SCORE UI
/// Attach this anywhere (e.g. your Canvas or a GameManager object).
/// Then drag your TextMeshPro Text elements into the fields below.
/// </summary>
public class BasketballScoreUI : MonoBehaviour
{
    [Header("Assign your Canvas Text (TMP) elements here")]
    [Tooltip("The TMP Text that shows the score  e.g.  0/10")]
    public TMP_Text scoreText;

    [Tooltip("The TMP Text shown when player wins  e.g.  LEVEL CLEAR!")]
    public TMP_Text winText;

    void Start()
    {
        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    public void UpdateScore(int current, int total)
    {
        if (scoreText != null)
            scoreText.text = $"{current}/{total}";
    }

    public void ShowWin()
    {
        if (winText != null)
            winText.gameObject.SetActive(true);
    }
}