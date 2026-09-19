using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootAttack : MonoBehaviour
{
    public AudioClip sound;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float bounceForce = 5f; // 踏みつけたときの跳ね返り力

    private Rigidbody2D playerRb;

    private void Awake()
    {
        // プレイヤーの Rigidbody2D を取得しておく
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        RaycastHit2D hit2d = Physics2D.Raycast(transform.position, Vector2.down, 0.6f);

        if(hit2d.collider != null)
        {
            if(hit2d.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit2d.collider.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }

                // 踏みつけ成功時に上方向へ跳ね返す
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                }

                if (sound != null)
                {
                    AudioSource.PlayClipAtPoint(sound, transform.position);
                }
            }
        }

        Debug.DrawRay(transform.position, Vector3.down, Color.yellow, 0.6f);
    }
}