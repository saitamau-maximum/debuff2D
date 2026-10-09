using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    [Header("移動経路（子要素のTransformを指定）")]
    [SerializeField] private Transform[] movePoints;
    [Header("速さ")][SerializeField] private float speed = 1.0f;
    [Header("待機時間（秒）")][SerializeField] private float waitTime = 1.0f;

    private Rigidbody2D rb;
    private int nowPoint = 0;
    private bool returnPoint = false;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    private Vector2[] worldPoints;

    // 外部から床の速度を取得するためのプロパティ
    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 previousPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (movePoints == null || movePoints.Length == 0 || rb == null) return;

        worldPoints = new Vector2[movePoints.Length];
        for (int i = 0; i < movePoints.Length; i++)
        {
            if (movePoints[i] != null)
            {
                worldPoints[i] = movePoints[i].position;
            }
        }

        rb.position = worldPoints[0];
        previousPosition = rb.position; // ★ 初期位置を記録
    }

    private void FixedUpdate()
    {
        // ★ 毎フレームの移動量から実際の速度（Vector2）を算出する
        CurrentVelocity = (rb.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = rb.position;

        if (movePoints == null || movePoints.Length <= 1 || rb == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            // 待機中は速度をゼロにする
            CurrentVelocity = Vector2.zero;
            previousPosition = rb.position; // 待機中の位置ズレを防ぐ
            return;
        }

        int nextPoint = nowPoint + (returnPoint ? -1 : 1);
        Vector2 targetPos = worldPoints[nextPoint];

        if (Vector2.Distance(rb.position, targetPos) > 0.05f)
        {
            Vector2 toVector = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
            rb.MovePosition(toVector);
        }
        else
        {
            rb.MovePosition(targetPos);
            StartWait();

            if (!returnPoint)
            {
                nowPoint++;
                if (nowPoint >= movePoints.Length - 1)
                {
                    returnPoint = true;
                }
            }
            else
            {
                nowPoint--;
                if (nowPoint <= 0)
                {
                    returnPoint = false;
                }
            }
        }
    }

    private void StartWait()
    {
        if (waitTime > 0f)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
}