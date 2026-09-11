using UnityEngine;
using System;

public class GoalArea : MonoBehaviour
{
    public static event Action OnGoalReached;

    [SerializeField] private GameObject player;

    private bool goalTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (goalTriggered) return;

        // プレイヤーがゴール範囲に触れたら発火
        if (collision.gameObject == player)
        {
            Debug.Log("Goal reached!");
            goalTriggered = true;
            OnGoalReached?.Invoke();
        }
    }
}
