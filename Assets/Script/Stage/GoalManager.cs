using UnityEngine;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{
    [Header("イベント")]
    [InspectorName("ゴール時")]
    [SerializeField] private UnityEvent onGoal;

    private void OnEnable()
    {
        GoalArea.OnGoalReached += HandleGoal;
    }

    private void OnDisable()
    {
        GoalArea.OnGoalReached -= HandleGoal;
    }

    private void HandleGoal()
    {
        // Inspector から設定できるイベントを発火
        onGoal?.Invoke();
        //リザルト画面に移行
        ScreenManager.Instance.ChangeScene(ScreenManager.SceneType.Result);
    }
}