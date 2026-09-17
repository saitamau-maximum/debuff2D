using UnityEngine;

public class Projectile : MonoBehaviour // クラス名を Projectile に変更
{
    [Header("弾の設定")]
    [SerializeField] private float speed = 10f;          // 速度
    [SerializeField] private float maxDistance = 15f;    // 射程距離（進める最大距離）

    private Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

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
    }
}