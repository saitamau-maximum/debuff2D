using UnityEngine;

/// FlyingPatrol と組み合わせて使う、検知・突進・帰還・接触攻撃。
/// 接触ダメージを内蔵しているため、ContactDamage の追加は不要です。
[DisallowMultipleComponent]
[RequireComponent(typeof(FlyingPatrol))]
public class FlyingChargeEnemy : MonoBehaviour
{
    [Header("検知・突進")]
    [Tooltip("未設定の場合は Player タグで検索します。")]
    [SerializeField] private Transform _playerTarget;
    [SerializeField, Min(0.01f)] private float _detectionRadius = 5f;
    [SerializeField, Min(0.01f)] private float _chargeSpeed = 12f;
    [SerializeField, Min(1)] private int _contactDamage = 1;
    [Tooltip("帰還後、次の突進が可能になるまでの時間。")]
    [SerializeField, Min(0f)] private float _chargeCooldown = 1f;

    private Rigidbody2D _rigidbody;
    private FlyingPatrol _patrol;
    private bool _isCharging;
    private bool _isReturning;
    private bool _hasHitPlayer;
    private Vector2 _chargeDirection;
    private Vector2 _chargeStartPosition;
    private Vector2 _chargeTargetPosition;
    private float _chargeDeadline;
    private float _nextChargeTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _patrol = GetComponent<FlyingPatrol>();
    }

    private void OnEnable()
    {
        _isCharging = false;
        _isReturning = false;
        _nextChargeTime = 0f;
        _hasHitPlayer = false;
    }

    private void OnDisable()
    {
        if (_patrol != null && _patrol.isActiveAndEnabled && (_isCharging || _isReturning))
            _patrol.ResumePatrol();
    }

    private void Start()
    {
        if (_playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTarget = player.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_hasHitPlayer)
        {
            return;
        }

        if (!_patrol.isActiveAndEnabled) return;

        if (_isReturning)
        {
            ReturnToChargeStart();
            return;
        }

        if (!_isCharging && Time.time >= _nextChargeTime && _playerTarget != null)
        {
            Vector2 offset = (Vector2)_playerTarget.position - _rigidbody.position;
            if (offset.sqrMagnitude <= _detectionRadius * _detectionRadius)
            {
                // 検知した瞬間の方向を固定する。以後は追尾しない。
                _chargeStartPosition = _rigidbody.position;
                _chargeTargetPosition = _playerTarget.position;
                _chargeDirection = offset.sqrMagnitude > 0.0001f
                    ? offset.normalized
                    : Vector2.right * _patrol.Direction;
                _isCharging = true;
                // 壁に阻まれて到達できない場合にも帰還する。
                _chargeDeadline = Time.time + offset.magnitude / Mathf.Max(0.01f, _chargeSpeed) + 0.5f;
                _patrol.PausePatrol();
            }
        }

        if (_isCharging)
        {
            Vector2 remaining = _chargeTargetPosition - _rigidbody.position;
            if (Vector2.Dot(remaining, _chargeDirection) <= 0f || Time.time >= _chargeDeadline)
            {
                _isCharging = false;
                _isReturning = true;
                ReturnToChargeStart();
                return;
            }

            _rigidbody.linearVelocity = _chargeDirection * _chargeSpeed;
            return;
        }
    }

    private void ReturnToChargeStart()
    {
        Vector2 offset = _chargeStartPosition - _rigidbody.position;
        if (offset.sqrMagnitude <= 0.0001f)
        {
            _rigidbody.position = _chargeStartPosition;
            _rigidbody.linearVelocity = Vector2.zero;
            _patrol.ResumePatrol();
            _isReturning = false;
            _nextChargeTime = Time.time + _chargeCooldown;
            return;
        }

        // 残り距離に合わせて減速し、開始位置を行き過ぎないようにする。
        float speed = Mathf.Min(_patrol.Speed, offset.magnitude / Time.fixedDeltaTime);
        _rigidbody.linearVelocity = offset.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHitPlayer(other);
    }

    private void TryHitPlayer(Collider2D other)
    {
        if (_hasHitPlayer)
        {
            return;
        }

        Health health = other.GetComponentInParent<Health>();
        if (health == null)
        {
            return;
        }

        // 複数のプレイヤーコライダーに同時接触しても一度だけ処理する。
        _hasHitPlayer = true;
        _rigidbody.linearVelocity = Vector2.zero;
        health.TakeDamage(_contactDamage);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        if (Application.isPlaying && _isCharging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, (Vector3)_chargeDirection * 3f);
        }
    }
}
