using System.Collections.Generic;
using UnityEngine;

/// <summary>移動方法に依存しない敵用射撃。FlyingPatrol と併用可能。</summary>
[DisallowMultipleComponent]
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [Tooltip("未設定なら敵の中心から発射します。")]
    [SerializeField] private Transform firePoint;
    [Tooltip("未設定なら Player タグから検索します。")]
    [SerializeField] private Transform playerTarget;
    [SerializeField, Min(0.01f)] private float detectionRadius = 7f;
    [SerializeField, Min(0.01f)] private float fireInterval = 1.5f;
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField] private LayerMask obstacleMask;

    private float nextFireTime;
    private Health targetHealth;
    private readonly List<RaycastHit2D> sightHits = new List<RaycastHit2D>();

    private void Reset() => obstacleMask = LayerMask.GetMask("Ground");

    private void Awake()
    {
        if (obstacleMask.value == 0) obstacleMask = LayerMask.GetMask("Ground");
    }

    private void OnEnable() => nextFireTime = Time.time;

    private void Start()
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
        if (playerTarget != null) targetHealth = playerTarget.GetComponentInParent<Health>();
        if (projectilePrefab == null)
            Debug.LogWarning("EnemyShooter: 弾Prefabを設定してください。", this);
    }

    private void Update()
    {
        if (projectilePrefab == null || playerTarget == null || Time.time < nextFireTime) return;
        if (targetHealth != null && targetHealth.IsDead) return;
        if (((Vector2)(playerTarget.position - transform.position)).sqrMagnitude > detectionRadius * detectionRadius) return;

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 offset = playerTarget.position - origin;
        if (offset.sqrMagnitude < 0.0001f) return;

        // 自分も Ground レイヤーにいるため、自身は視線判定から除外する。
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(obstacleMask);
        filter.useTriggers = false;
        Physics2D.Raycast(origin, offset.normalized, filter, sightHits, offset.magnitude);
        foreach (RaycastHit2D hit in sightHits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;
            if (targetHealth != null && hit.collider.GetComponentInParent<Health>() == targetHealth) continue;
            return;
        }

        Projectile bullet = Instantiate(projectilePrefab, origin, Quaternion.identity);
        bullet.Initialize(offset, Projectile.TargetType.Player, transform, damage);
        nextFireTime = Time.time + Mathf.Max(0.01f, fireInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
