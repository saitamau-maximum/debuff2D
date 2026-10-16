using UnityEngine;

/// <summary>攻撃方法に依存しない、高度を維持した左右往復移動。</summary>
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FlyingPatrol : MonoBehaviour
{
    public enum InitialDirection { Left = -1, Right = 1 }

    [SerializeField] private InitialDirection _initialDirection = InitialDirection.Right;
    [SerializeField, Min(0.01f)] private float _patrolSpeed = 2f;
    [Tooltip("配置位置から片側の距離。")]
    [SerializeField, Min(0.01f)] private float _patrolHalfWidth = 3f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField, Min(0.01f)] private float _wallCheckDistance = 0.2f;

    private readonly RaycastHit2D[] _wallHits = new RaycastHit2D[8];
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ContactFilter2D _groundFilter;
    private Vector2 _patrolCenter;
    private int _direction;

    public float Speed => Mathf.Max(0.01f, _patrolSpeed);
    public int Direction => _direction;
    public bool IsPaused { get; private set; }

    private void Reset()
    {
        _groundMask = LayerMask.GetMask("Ground");
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;
        _rigidbody.linearDamping = 0f;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        if (_groundMask.value == 0)
            _groundMask = LayerMask.GetMask("Ground");
        _groundFilter = new ContactFilter2D();
        _groundFilter.SetLayerMask(_groundMask);
        _groundFilter.useTriggers = false;
    }

    private void OnEnable()
    {
        _patrolCenter = _rigidbody.position;
        _direction = (int)_initialDirection;
        ResumePatrol();
    }

    /// <summary>攻撃側に移動制御を渡す。中心位置と進行方向は維持する。</summary>
    public void PausePatrol()
    {
        IsPaused = true;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>攻撃前の高度・範囲・方向で徘徊を再開する。</summary>
    public void ResumePatrol()
    {
        IsPaused = false;
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.position = new Vector2(_rigidbody.position.x, _patrolCenter.y);
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation
            | RigidbodyConstraints2D.FreezePositionY;
    }

    private void OnDisable()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (IsPaused) return;

        float halfWidth = Mathf.Max(0.01f, _patrolHalfWidth);
        float leftEdge = _patrolCenter.x - halfWidth;
        float rightEdge = _patrolCenter.x + halfWidth;
        bool reachedEdge = _direction > 0
            ? _rigidbody.position.x >= rightEdge
            : _rigidbody.position.x <= leftEdge;
        float checkDistance = _wallCheckDistance + Speed * Time.fixedDeltaTime;
        if (reachedEdge || _collider.Cast(Vector2.right * _direction, _groundFilter,
                _wallHits, checkDistance) > 0)
            _direction = -_direction;

        _rigidbody.position = new Vector2(_rigidbody.position.x, _patrolCenter.y);
        float distanceToEdge = _direction > 0
            ? rightEdge - _rigidbody.position.x
            : _rigidbody.position.x - leftEdge;
        float speed = Mathf.Min(Speed, Mathf.Max(0f, distanceToEdge) / Time.fixedDeltaTime);
        _rigidbody.linearVelocity = new Vector2(_direction * speed, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying
            ? new Vector3(_patrolCenter.x, _patrolCenter.y, transform.position.z)
            : transform.position;
        Vector3 halfRange = Vector3.right * Mathf.Max(0.01f, _patrolHalfWidth);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(center - halfRange, center + halfRange);
        Gizmos.DrawLine(center - halfRange + Vector3.up * 0.2f, center - halfRange - Vector3.up * 0.2f);
        Gizmos.DrawLine(center + halfRange + Vector3.up * 0.2f, center + halfRange - Vector3.up * 0.2f);
    }
}
