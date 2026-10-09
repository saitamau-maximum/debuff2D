using TMPro;
using UnityEngine;

public class ResultListRow : MonoBehaviour
{
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private TMP_Text scoreText;

    public void SetScore(HighScoreStore.FloorScore score)
    {
        floorText.text = "Floor " + score.Floor;
        scoreText.text = score.Score.ToString("N0");
    }
}
