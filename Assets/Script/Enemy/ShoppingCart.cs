using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ShoppingCart : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool isMovingRight = true;

    [Header("ダメージ設定")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldownTime = 2.0f;

    [Header("レイヤー設定")]
    [SerializeField] private string groundLayerName = "Ground";

    private float currentCooldown = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        int groundLayer = LayerMask.NameToLayer(groundLayerName);

        // シーン内の全コライダーを取得し、Ground以外のレイヤーはすべて物理衝突を無視
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D other in allColliders)
        {
            if (other != col && other.gameObject.layer != groundLayer)
            {
                Physics2D.IgnoreCollision(col, other, true);
            }
        }
    }

    private void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        CheckDamage();
    }

    private void FixedUpdate()
    {
        float direction = isMovingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void CheckDamage()
    {
        if (currentCooldown > 0f) return;

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapCollider(col, filter, hits);

        foreach (var hit in hits)
        {
            Health playerHealth = hit.GetComponentInParent<Health>();

            if (playerHealth != null && playerHealth.gameObject.CompareTag("Player"))
            {
                if (!playerHealth.IsDead && !playerHealth.IsInvincible)
                {
                    playerHealth.TakeDamage(damage);
                    currentCooldown = cooldownTime;
                    break;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int groundLayer = LayerMask.NameToLayer(groundLayerName);

        // 後からスポーン・接触したオブジェクトが Ground 以外なら即座に衝突無視
        if (collision.gameObject.layer != groundLayer)
        {
            Physics2D.IgnoreCollision(col, collision.collider, true);
            return;
        }

        // Ground（壁や段差）への衝突時、横方向からのヒットなら進行方向を反転
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                isMovingRight = !isMovingRight;
                break;
            }
        }
    }
}