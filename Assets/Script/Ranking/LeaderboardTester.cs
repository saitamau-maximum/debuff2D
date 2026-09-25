using UnityEngine;

public class LeaderboardTester : MonoBehaviour
{
    [SerializeField]
    private LeaderboardManager leaderboardManager;

    [SerializeField]
    private double testScore = 1000;

    [SerializeField, Min(1)]
    private int topScoresCount = 10;

    private bool HasManager()
    {
        if (leaderboardManager != null)
        {
            return true;
        }

        Debug.LogError(
            "LeaderboardTesterにLeaderboardManagerが設定されていません。",
            this);
        return false;
    }

    public async void SubmitTestScore()
    {
        if (!HasManager()) return;

        await leaderboardManager.SubmitScoreAsync(testScore);
    }

    public async void ShowTopScores()
    {
        if (!HasManager()) return;

        await leaderboardManager.GetTopScoresAsync(topScoresCount);
    }

    public async void ShowMyScore()
    {
        if (!HasManager()) return;

        await leaderboardManager.GetMyScoreAsync();
    }
}
