using System.Collections.Generic;
using UnityEngine;

// プレイヤーと敵が共用する直進弾。SetDirection(float) との互換性も維持する。
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public enum TargetType { Enemy, Player }

    [Header("弾の設定")]
    [SerializeField, Min(0.01f)] private float speed = 10f;
    [SerializeField, Min(0.01f)] private float maxDistance = 15f;
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField] private TargetType targetType = TargetType.Enemy;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();
    private Rigidbody2D body;
    private Collider2D projectileCollider;
    private Transform owner;
    private Vector2 direction = Vector2.right;
    private float travelled;
    private bool spent;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable()
    {
        travelled = 0f;
        spent = false;
    }

    public void Initialize(Vector2 fireDirection, TargetType target, Transform shooter, int hitDamage = 1)
    {
        targetType = target;
        owner = shooter;
        damage = Mathf.Max(1, hitDamage);
        SetDirection(fireDirection);
    }

    public void SetDirection(float facingDirection)
    {
        SetDirection(Vector2.right * (facingDirection < 0f ? -1f : 1f));
    }

    public void SetDirection(Vector2 fireDirection)
    {
        direction = fireDirection.sqrMagnitude > 0f ? fireDirection.normalized : Vector2.right;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction.x < 0f ? -1f : 1f);
        transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (spent) return;
        float distance = Mathf.Min(Mathf.Max(0.01f, speed) * Time.fixedDeltaTime,
            Mathf.Max(0f, maxDistance - travelled));

        // 移動区間全体を調べ、高速の弾が薄い壁や標的をすり抜けるのを防ぐ。
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        projectileCollider.Cast(direction, filter, hits, distance);
        hits.Sort((a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit2D hit in hits)
        {
            if (TryHit(hit.collider)) return;
        }

        body.MovePosition(body.position + direction * distance);
        travelled += distance;
        if (travelled >= maxDistance) Consume();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);

    private bool TryHit(Collider2D other)
    {
        if (spent || other == null || other == projectileCollider) return false;
        if (owner != null && (other.transform == owner || other.transform.IsChildOf(owner))) return false;
        if (other.GetComponentInParent<Projectile>() != null) return false;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        Health player = other.GetComponentInParent<Health>();
        if (targetType == TargetType.Player && player != null)
        {
            Consume();
            player.TakeDamage(damage);
            return true;
        }
        if (targetType == TargetType.Enemy && enemy != null)
        {
            Consume();
            enemy.TakeDamage(damage);
            return true;
        }

        // 敵が Ground レイヤーでも味方の弾は消さない。
        if (enemy != null || player != null) return false;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Consume();
            return true;
        }
        return false;
    }

    private void Consume()
    {
        spent = true;
        Destroy(gameObject);
    }
}
