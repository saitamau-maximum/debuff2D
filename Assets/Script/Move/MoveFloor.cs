using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloor : MonoBehaviour
{
    [Header("移動経路（子要素のTransformを指定）")]
    [SerializeField] private Transform[] movePoints; // GameObjectからTransformに変更すると便利です
    [Header("速さ")][SerializeField] private float speed = 1.0f;
    [Header("待機時間（秒）")][SerializeField] private float waitTime = 1.0f;

    private Rigidbody2D rb;
    private int nowPoint = 0;
    private bool returnPoint = false;

    private float waitTimer = 0f;
    private bool isWaiting = false;

    // ワールド座標での目標位置を保持する配列
    private Vector2[] worldPoints;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (movePoints == null || movePoints.Length == 0 || rb == null) return;

        // ポイントの初期位置（ワールド座標）を記憶する
        worldPoints = new Vector2[movePoints.Length];
        for (int i = 0; i < movePoints.Length; i++)
        {
            if (movePoints[i] != null)
            {
                worldPoints[i] = movePoints[i].position;
            }
        }

        rb.position = worldPoints[0];
    }

    private void FixedUpdate()
    {
        if (movePoints == null || movePoints.Length <= 1 || rb == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }

        int nextPoint = nowPoint + (returnPoint ? -1 : 1);
        Vector2 targetPos = worldPoints[nextPoint];

        // ターゲットに向かって移動
        if (Vector2.Distance(rb.position, targetPos) > 0.05f)
        {
            Vector2 toVector = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
            rb.MovePosition(toVector);
        }
        else
        {
            rb.MovePosition(targetPos);
            StartWait();

            // 次のポイントへ進む・戻るの切り替え
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