using UnityEngine;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{
    private bool handledGoal;
    [SerializeField] private StageClearUI resultUI;
    [Header("イベント")]
    [InspectorName("ゴール時")]
    [SerializeField] private UnityEvent onGoal;
    private static GoalManager instance;
    private void Awake()
    {
        instance = this;
    }
    // GoalArea から呼ばれる静的メソッド
    public static void NotifyGoalEntered()
    {
        if (instance != null)
        {
            instance.HandleGoal();
        }
    }

    private void HandleGoal()
    {
        if (handledGoal) return;
        if (resultUI == null || !resultUI.IsConfigured || ScoreManager.Instance == null || ScreenManager.Instance == null)
        {
            Debug.LogError("Assign Result UI and place ScoreManager / ScreenManager in the scene.", this);
            return;
        }
        handledGoal = true;
        int score = StageScoreCalculator.Calculate(ScoreManager.Instance.CoinCount);
        ScoreManager.Instance.FinishStage();
        int? previousBestScore = null;
        bool isNewBest = false;
        bool saveFailed = false;
        try
        {
            string playerId = HighScoreStore.CurrentPlayerId;
            int floor = ScreenManager.Instance.CurrentFloor;
            if (HighScoreStore.TryGetBest(playerId, floor, out int previousBest))
                previousBestScore = previousBest;
            // 保存は更新するが、表示には今回のクリア前の最高値を使う。
            isNewBest = HighScoreStore.SaveIfHigher(playerId, floor, score);
        }
        catch (System.Exception exception)
        {
            saveFailed = true;
            Debug.LogError("Failed to save the stage high score.", this);
            Debug.LogException(exception, this);
        }
        resultUI.Show(score, previousBestScore, isNewBest, saveFailed);
        // Inspector から設定できるイベントを発火
        onGoal?.Invoke();
    }
}
