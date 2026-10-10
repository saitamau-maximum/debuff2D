using UnityEngine;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{
    private bool handledGoal;
    [SerializeField] private StageClearUI resultUI;
    [Header("イベント")]
    [InspectorName("ゴール時")]
    [SerializeField] private UnityEvent onGoal;
    public static GoalManager Instance { get ; private set;}
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // GoalArea から呼ばれる静的メソッド
    public static void NotifyGoalEntered()
    {
        if (Instance != null)
        {
            Instance.HandleGoal();
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
        int totalScore = StageScoreCalculator.TotalScoreCalculate(ScoreManager.Instance.CoinCount, TimerManager.Instance.remainTime);
        int coinScore = StageScoreCalculator.CoinScoreCalculate(ScoreManager.Instance.CoinCount);
        int timeScore = StageScoreCalculator.TimeScoreCalcurate(TimerManager.Instance.remainTime);
        ScoreManager.Instance.FinishStage();
        TimerManager.Instance.StopTimer();
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
            isNewBest = HighScoreStore.SaveIfHigher(playerId, floor, totalScore);
        }
        catch (System.Exception exception)
        {
            saveFailed = true;
            Debug.LogError("Failed to save the stage high score.", this);
            Debug.LogException(exception, this);
        }
        resultUI.Show(coinScore, timeScore, totalScore, previousBestScore, isNewBest, saveFailed);
        // Inspector から設定できるイベントを発火
        onGoal?.Invoke();
    }
}
