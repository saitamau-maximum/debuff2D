using UnityEngine;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{
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

    private bool goalHandled = false;

    private void HandleGoal()
    {
        if(goalHandled) return;
        goalHandled = true;
        // Inspector から設定できるイベントを発火
        onGoal?.Invoke();
        //リザルト画面に移行
        ScreenManager.Instance.ChangeScene(ScreenManager.SceneType.Result);
    }
}