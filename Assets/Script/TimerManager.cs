using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Transform player;

    private float remainingTime;
    private bool timeUpHandled = false;
    private Vector3 respawnPosition;

    void Start()
    {
        remainingTime = timeLimit;

        // ゲーム開始時のPlayerの位置を記録
        respawnPosition = player.position;

        UpdateTimeText();
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

        // Playerを最初の位置に戻す
        player.position = respawnPosition;

        // Playerの速度を止める
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}