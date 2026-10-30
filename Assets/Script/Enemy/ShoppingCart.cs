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

    private float currentCooldown = 0f;
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        // プレイヤーを検索して物理的な衝突（押し出し合い）を無視
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCol = player.GetComponent<Collider2D>();
            if (playerCol != null) Physics2D.IgnoreCollision(col, playerCol, true);
        }

        // 敵（Enemyタグ）を検索して物理的な衝突を無視
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCol = enemy.GetComponent<Collider2D>();
            if (enemyCol != null) Physics2D.IgnoreCollision(col, enemyCol, true);
        }

        // Bounceギミック（オレンジ色のブロック）を検索して物理的な衝突を無視
        BounceGimmick[] bounces = FindObjectsOfType<BounceGimmick>();
        foreach (BounceGimmick bounce in bounces)
        {
            Collider2D bounceCol = bounce.GetComponent<Collider2D>();
            if (bounceCol != null) Physics2D.IgnoreCollision(col, bounceCol, true);
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
        // 物理演算を使って移動（重力を維持したまま横へ動く）
        float direction = isMovingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void CheckDamage()
    {
        if (currentCooldown > 0f) return;

        // レイヤー制限などをすべて無視して、コライダーに重なっている全オブジェクトを取得
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapCollider(col, filter, hits);

        foreach (var hit in hits)
        {
            // 子オブジェクトのColliderに当たっても、親に付いているHealthを自動で取得する
            Health playerHealth = hit.GetComponentInParent<Health>();

            // Healthが見つかり、かつその大元がPlayerタグを持っていればダメージ処理へ
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

    // 物理的な衝突が発生した瞬間に呼ばれる
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したすべての接点を確認
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 法線（normal）のX成分を見て、横方向の衝突（壁や段差）か判定
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                // 進行方向を反転させてループを抜ける
                isMovingRight = !isMovingRight;
                break;
            }
        }
    }
}