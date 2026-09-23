using UnityEngine;

public class Projectile : MonoBehaviour // クラス名を Projectile に変更
{
    [Header("弾の設定")]
    [SerializeField] private float speed = 10f;          // 速度
    [SerializeField] private float maxDistance = 15f;    // 射程距離（進める最大距離）

    private Vector2 startPosition;
    private float moveDirection = 1f; // 飛ぶ方向

    // 外部（PlayerShooter）から呼ばれて、飛ぶ方向を決定するメソッド
    public void SetDirection(float direction)
    {
        moveDirection = direction;

        // 弾自体の見た目も左向きのときに反転させたい場合
        if (moveDirection < 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime * moveDirection);

        float currentDistance = Vector2.Distance(startPosition, transform.position);
        if (currentDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        // 2. 床（Ground）に当たった場合
        // ※もし床のオブジェクトのレイヤーが "Ground" なら消滅する
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}