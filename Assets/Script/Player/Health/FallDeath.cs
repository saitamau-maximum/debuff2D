using UnityEngine;

public class FallDeath : MonoBehaviour
{
    [SerializeField] private float fallThresholdY = -20f;
    [SerializeField] private Health playerHealth;

    private void Reset()
    {
        // プレイヤーにアタッチされている前提なら自動セット
        playerHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("FallDeath: Health が設定されていません");
            return;
        }

        if (transform.position.y < fallThresholdY)
        {
            playerHealth.Kill();
        }
    }
}

