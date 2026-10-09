using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform player;
    [SerializeField] private Health health;

    private float remainingTime;
    private bool timeUpHandled;

    void Start()
    {
        InitializeTimerValiable();

        UpdateTimeText();
    }
    void InitializeTimerValiable()
    {
        remainingTime = timeLimit;
        timeUpHandled = false;
    }

    void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimeText();

            if (!timeUpHandled)
            {
                timeUpHandled = true;
                TimeUp();
            }

            return;
        }

        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        timeText.text = "Time: " + Mathf.CeilToInt(remainingTime);
    }

    private void TimeUp()
    {
        Debug.Log("時間切れ！");
        //死亡＆リスポーン処理
        health.Kill();
        //再初期化
        InitializeTimerValiable();
    }
}