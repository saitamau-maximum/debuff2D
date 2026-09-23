using UnityEngine;
using System;

public class GoalArea : MonoBehaviour
{

    [SerializeField] private GameObject player;

    private bool goalTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (goalTriggered) return;

        // プレイヤーがゴール範囲に触れたら通知
        if (collision.gameObject == player)
        {
            goalTriggered = true;
            GoalManager.NotifyGoalEntered();
        }
    }
}
