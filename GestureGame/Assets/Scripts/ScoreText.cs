using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI Scoretext;  // This is your TMP UI text
    private int score = 0;

    void Start()
    {
        UpdateScoreUI();
    }

    // Call this to increase score
    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        Scoretext.text = "Score: " + score;
    }
}