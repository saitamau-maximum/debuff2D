using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Health health;

    private float startTime;
    private bool goaled;
    private bool timeUpHandled;
    public float remainTime { get ; private set;}
    public static TimerManager Instance{ get ; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetTimer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(!goaled){
            float elapsed = Time.time - startTime;
            remainTime = Mathf.Max(0f, timeLimit - elapsed);

            timeText.text = "Time: " + Mathf.CeilToInt(remainTime);

            if (remainTime <= 0f && !timeUpHandled)
            {
                timeUpHandled = true;
                TimeUp();
            }
        }
    }

    public void StopTimer()
    {
        if(goaled) return;
        else
        {
            goaled = true;
        }
    }

    public void ResetTimer()
    {
        startTime = Time.time;
        timeUpHandled = false;
        goaled = false;
    }

    private void TimeUp()
    {
        Debug.Log("時間切れ！");
        health.Kill();
    }
}
