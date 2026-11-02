using System;
using UnityEngine;
using System.Collections;

public class DebuffAddGimmic : MonoBehaviour
{
    [SerializeField] private DebuffSystem debuffSystem;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private int addValue;
    [SerializeField] private float deltaTime;
    [SerializeField] private DebuffType type;

    private bool isPlayerInside;
    private bool isLoopRunning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != playerCollider) return;

        isPlayerInside = true;
        if(!isLoopRunning){
            StartCoroutine(DebuffLoop());
        }
    }
    private void OnDisable()
    {
        isPlayerInside = false;
        isLoopRunning = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != playerCollider) return;

        isPlayerInside = false;
    }

    private IEnumerator DebuffLoop()
    {
        isLoopRunning = true;
        while (isPlayerInside)
        {
            debuffSystem.AddTemporaryDebuff(
                addValue,
                type,
                deltaTime);

            yield return new WaitForSeconds(deltaTime);
        }
        isLoopRunning = false;
    }
}