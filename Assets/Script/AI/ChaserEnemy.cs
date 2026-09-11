using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ChaserEnemy : MonoBehaviour
{
    [SerializeField] private Transform _playerTarget;
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _stopDistance = 0.6f;

    [Header("Environment")]
    [SerializeField] private LayerMask _environmentMask;
    [SerializeField] private float _groundCheckDistance = 0.15f;

    [Header("Step Jump")]
    [SerializeField] private float _stepCheckDistance = 0.5f;
    [SerializeField] private float _stepCheckHeight = 0.8f;
    [SerializeField] private float _jumpVelocity = 6f;
    [SerializeField] private float _jumpCooldown = 0.25f;

    [Header("Wall Check")]
    [SerializeField] private float _wallCheckDistance = 0.08f;
    [SerializeField] private float _wallPushBackSpeed = 0.3f;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private float _nextJumpTime;

    // 同じ段差に対する連続ジャンプを防ぐ
    private bool _stepJumpLocked;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        if (_playerTarget == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                _playerTarget = playerObject.transform;
            }
        }

        if (_playerTarget == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: Player target is not set."
            );
        }
    }

    private void FixedUpdate()
    {
        if (_playerTarget == null)
        {
            SetHorizontalVelocity(0f);
            return;
        }

        float horizontalDifference =
            _playerTarget.position.x - transform.position.x;

        float horizontalDistance = Mathf.Abs(horizontalDifference);

        if (horizontalDistance <= _stopDistance)
        {
            SetHorizontalVelocity(0f);
            return;
        }

        float direction = Mathf.Sign(horizontalDifference);

        bool grounded = IsGrounded();
        bool stepDetected = grounded && IsStepAhead(direction);

        // 段差を通過した後、次のジャンプを許可する
        if (grounded && !stepDetected)
        {
            _stepJumpLocked = false;
        }

        if (!_stepJumpLocked &&
            grounded &&
            stepDetected &&
            Time.time >= _nextJumpTime)
        {
            Jump();

            _stepJumpLocked = true;
            _nextJumpTime = Time.time + _jumpCooldown;
        }

        bool wallAhead = IsWallAhead(direction);

        float horizontalVelocity = direction * _moveSpeed;

        // 空中で壁へ押し付け続けない
        if (!grounded && wallAhead)
        {
            horizontalVelocity =
                -direction * _wallPushBackSpeed;
        }

        SetHorizontalVelocity(horizontalVelocity);
    }

    private void Jump()
    {
        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.y = _jumpVelocity;
        _rigidbody.linearVelocity = velocity;
    }

    private void SetHorizontalVelocity(float horizontalVelocity)
    {
        _rigidbody.linearVelocity = new Vector2(
            horizontalVelocity,
            _rigidbody.linearVelocity.y
        );
    }

    private bool IsStepAhead(float direction)
    {
        Bounds bounds = _collider.bounds;

        float frontX = direction > 0f
            ? bounds.max.x + 0.01f
            : bounds.min.x - 0.01f;

        Vector2 forward = Vector2.right * direction;

        // 段差の下側を調べる
        Vector2 lowerOrigin = new Vector2(
            frontX,
            bounds.min.y + 0.15f
        );

        RaycastHit2D lowerHit = Physics2D.Raycast(
            lowerOrigin,
            forward,
            _stepCheckDistance,
            _environmentMask
        );

        if (lowerHit.collider == null)
        {
            return false;
        }

        // 段差上部に通れる空間があるか調べる
        Vector2 upperOrigin = new Vector2(
            frontX,
            bounds.min.y + _stepCheckHeight
        );

        RaycastHit2D upperHit = Physics2D.Raycast(
            upperOrigin,
            forward,
            _stepCheckDistance,
            _environmentMask
        );

        return upperHit.collider == null;
    }

    private bool IsWallAhead(float direction)
    {
        Bounds bounds = _collider.bounds;

        float frontX = direction > 0f
            ? bounds.max.x + 0.01f
            : bounds.min.x - 0.01f;

        Vector2 forward = Vector2.right * direction;

        // 敵の胴体付近で壁を検出する
        Vector2 origin = new Vector2(
            frontX,
            bounds.center.y
        );

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            forward,
            _wallCheckDistance,
            _environmentMask
        );

        return hit.collider != null;
    }

    private bool IsGrounded()
    {
        Bounds bounds = _collider.bounds;

        Vector2 boxCenter = new Vector2(
            bounds.center.x,
            bounds.min.y - _groundCheckDistance / 2f
        );

        Vector2 boxSize = new Vector2(
            bounds.size.x * 0.8f,
            _groundCheckDistance
        );

        Collider2D hit = Physics2D.OverlapBox(
            boxCenter,
            boxSize,
            0f,
            _environmentMask
        );

        return hit != null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Collider2D currentCollider = GetComponent<Collider2D>();

        if (currentCollider == null)
        {
            return;
        }

        Bounds bounds = currentCollider.bounds;

        float direction = 1f;

        if (_playerTarget != null)
        {
            float difference =
                _playerTarget.position.x - transform.position.x;

            if (Mathf.Abs(difference) > 0.01f)
            {
                direction = Mathf.Sign(difference);
            }
        }

        float frontX = direction > 0f
            ? bounds.max.x + 0.01f
            : bounds.min.x - 0.01f;

        Vector3 forward =
            Vector3.right * direction;

        Vector3 lowerOrigin = new Vector3(
            frontX,
            bounds.min.y + 0.15f,
            transform.position.z
        );

        Vector3 upperOrigin = new Vector3(
            frontX,
            bounds.min.y + _stepCheckHeight,
            transform.position.z
        );

        Vector3 wallOrigin = new Vector3(
            frontX,
            bounds.center.y,
            transform.position.z
        );

        // 段差下側
        Gizmos.DrawLine(
            lowerOrigin,
            lowerOrigin + forward * _stepCheckDistance
        );

        // 段差上側
        Gizmos.DrawLine(
            upperOrigin,
            upperOrigin + forward * _stepCheckDistance
        );

        // 壁判定
        Gizmos.DrawLine(
            wallOrigin,
            wallOrigin + forward * _wallCheckDistance
        );

        // 接地判定
        Vector3 groundCenter = new Vector3(
            bounds.center.x,
            bounds.min.y - _groundCheckDistance / 2f,
            transform.position.z
        );

        Vector3 groundSize = new Vector3(
            bounds.size.x * 0.8f,
            _groundCheckDistance,
            0f
        );

        Gizmos.DrawWireCube(groundCenter, groundSize);
    }
#endif
}