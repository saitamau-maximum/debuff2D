using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SimpleEnemy : MonoBehaviour
{
    public enum InitialDirection
    {
        Left = -1,
        Right = 1
    }

    [SerializeField] private InitialDirection _initialDirection = InitialDirection.Right;
    [SerializeField] private float _moveSpeed = 2.5f;

    [Header("Environment")]
    [SerializeField] private LayerMask _environmentMask;
    [SerializeField] private float _groundCheckDistance = 0.15f;

    [Header("Step Handling")]
    [SerializeField] private bool _canJumpOverSteps = true;
    [SerializeField] private float _stepCheckDistance = 0.5f;
    [SerializeField] private float _stepCheckHeight = 0.8f;
    [SerializeField] private float _jumpVelocity = 6f;
    [SerializeField] private float _jumpCooldown = 0.25f;

    [Header("Drop Handling")]
    [SerializeField] private bool _canDropDown = true;

    // 進行方向のどのくらい先を見るか
    [SerializeField] private float _dropCheckDistance = 0.15f;

    // この深さまで地面を探す
    [SerializeField] private float _dropCheckDepth = 1.0f;

    // ジャンプ後、この時間だけ崖判定を無効化する
    [SerializeField] private float _dropCheckGraceTimeAfterJump = 0.6f;

    [Header("Wall Check")]
    [SerializeField] private float _wallCheckDistance = 0.08f;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    private float _nextJumpTime;
    private float _ignoreDropCheckUntil;

    private bool _stepJumpLocked;
    private int _facingDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _facingDirection = (int)_initialDirection;
    }

    private void FixedUpdate()
    {
        bool grounded = IsGrounded();

        // 同じフレーム内で何度もRaycastしないように保存
        bool stepAhead = grounded && IsStepAhead(_facingDirection);

        // -------------------------
        // ジャンプロック解除
        // -------------------------
        if (grounded && !stepAhead)
        {
            _stepJumpLocked = false;
        }

        // -------------------------
        // 上り段差
        // -------------------------
        if (_canJumpOverSteps &&
            !_stepJumpLocked &&
            grounded &&
            stepAhead &&
            Time.time >= _nextJumpTime)
        {
            Jump();

            _stepJumpLocked = true;
            _nextJumpTime = Time.time + _jumpCooldown;

            // ジャンプ中も横方向へ進む
            SetHorizontalVelocity(_facingDirection * _moveSpeed);

            return;
        }

        // -------------------------
        // 下り段差・崖
        // -------------------------
        // ・崖を降りない設定
        // ・上り段差ではない
        // ・ジャンプ直後の猶予時間ではない
        // ・前方に地面がない
        //
        // この条件が揃ったときだけ反転する
        if (grounded &&
            !stepAhead &&
            !_canDropDown &&
            Time.time >= _ignoreDropCheckUntil &&
            !HasGroundAhead(_facingDirection))
        {
            TurnAround();
            return;
        }

        // -------------------------
        // 壁
        // -------------------------
        if (IsWallAhead(_facingDirection) &&
            !_canJumpOverSteps)
        {
            TurnAround();
            return;
        }

        // -------------------------
        // 通常移動
        // -------------------------
        SetHorizontalVelocity(_facingDirection * _moveSpeed);
    }


    // 進行方向を反転する
    private void TurnAround()
    {
        _facingDirection = -_facingDirection;

        SetHorizontalVelocity(
            _facingDirection * _moveSpeed
        );
    }


    /// ジャンプする
    private void Jump()
    {
        Vector2 velocity = _rigidbody.linearVelocity;

        velocity.y = _jumpVelocity;

        _rigidbody.linearVelocity = velocity;

        // ジャンプした直後は、
        // 段差上で崖判定が誤作動しないようにする
        _ignoreDropCheckUntil =
            Time.time + _dropCheckGraceTimeAfterJump;
    }


    // 横方向の速度を設定する
    private void SetHorizontalVelocity(float horizontalVelocity)
    {
        _rigidbody.linearVelocity =
            new Vector2(
                horizontalVelocity,
                _rigidbody.linearVelocity.y
            );
    }


    // 前方に上り段差があるか
    private bool IsStepAhead(float direction)
    {
        Bounds bounds = _collider.bounds;

        float frontX =
            direction > 0f
                ? bounds.max.x + 0.01f
                : bounds.min.x - 0.01f;

        Vector2 forward =
            Vector2.right * direction;

        // -------------------------
        // 足元付近を見るRay
        // -------------------------
        Vector2 lowerOrigin =
            new Vector2(
                frontX,
                bounds.min.y + 0.15f
            );

        RaycastHit2D lowerHit =
            Physics2D.Raycast(
                lowerOrigin,
                forward,
                _stepCheckDistance,
                _environmentMask
            );

        // 足元付近に何もない
        // → 上り段差ではない
        if (lowerHit.collider == null)
        {
            return false;
        }

        // -------------------------
        // 上側を見るRay
        // -------------------------
        Vector2 upperOrigin =
            new Vector2(
                frontX,
                bounds.min.y + _stepCheckHeight
            );

        RaycastHit2D upperHit =
            Physics2D.Raycast(
                upperOrigin,
                forward,
                _stepCheckDistance,
                _environmentMask
            );

        // 下だけ塞がっていて、
        // 上が空いている
        //
        // → ジャンプ可能な段差
        return upperHit.collider == null;
    }

    /// <summary>
    /// 進行方向の少し先に地面が存在するか
    /// </summary>
    private bool HasGroundAhead(float direction)
    {
        Bounds bounds = _collider.bounds;

        float frontX =
            direction > 0f
                ? bounds.max.x + _dropCheckDistance
                : bounds.min.x - _dropCheckDistance;

        Vector2 origin =
            new Vector2(
                frontX,
                bounds.min.y + 0.05f
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                origin,
                Vector2.down,
                _dropCheckDepth,
                _environmentMask
            );

        return hit.collider != null;
    }

    // 前方に壁があるか
    private bool IsWallAhead(float direction)
    {
        Bounds bounds = _collider.bounds;

        float frontX =
            direction > 0f
                ? bounds.max.x + 0.01f
                : bounds.min.x - 0.01f;

        Vector2 forward =
            Vector2.right * direction;

        Vector2 origin =
            new Vector2(
                frontX,
                bounds.center.y
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                origin,
                forward,
                _wallCheckDistance,
                _environmentMask
            );

        return hit.collider != null;
    }


    // 地面に接触しているか
    private bool IsGrounded()
    {
        Bounds bounds = _collider.bounds;

        Vector2 boxCenter =
            new Vector2(
                bounds.center.x,
                bounds.min.y - _groundCheckDistance / 2f
            );

        Vector2 boxSize =
            new Vector2(
                bounds.size.x * 0.8f,
                _groundCheckDistance
            );

        Collider2D hit =
            Physics2D.OverlapBox(
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
        Collider2D currentCollider =
            GetComponent<Collider2D>();

        if (currentCollider == null)
        {
            return;
        }

        Bounds bounds =
            currentCollider.bounds;

        float direction =
            Application.isPlaying
                ? _facingDirection
                : (int)_initialDirection;

        float frontX =
            direction > 0f
                ? bounds.max.x + 0.01f
                : bounds.min.x - 0.01f;

        Vector3 forward =
            Vector3.right * direction;

        // -------------------------
        // 上り段差判定
        // -------------------------
        Vector3 lowerOrigin =
            new Vector3(
                frontX,
                bounds.min.y + 0.15f,
                transform.position.z
            );

        Vector3 upperOrigin =
            new Vector3(
                frontX,
                bounds.min.y + _stepCheckHeight,
                transform.position.z
            );

        Gizmos.DrawLine(
            lowerOrigin,
            lowerOrigin +
            forward * _stepCheckDistance
        );

        Gizmos.DrawLine(
            upperOrigin,
            upperOrigin +
            forward * _stepCheckDistance
        );

        // -------------------------
        // 壁判定
        // -------------------------
        Vector3 wallOrigin =
            new Vector3(
                frontX,
                bounds.center.y,
                transform.position.z
            );

        Gizmos.DrawLine(
            wallOrigin,
            wallOrigin +
            forward * _wallCheckDistance
        );

        // -------------------------
        // 下り段差・崖判定
        // -------------------------
        float dropX =
            direction > 0f
                ? bounds.max.x + _dropCheckDistance
                : bounds.min.x - _dropCheckDistance;

        Vector3 dropOrigin =
            new Vector3(
                dropX,
                bounds.min.y + 0.05f,
                transform.position.z
            );

        Gizmos.DrawLine(
            dropOrigin,
            dropOrigin +
            Vector3.down * _dropCheckDepth
        );

        // -------------------------
        // 接地判定
        // -------------------------
        Vector3 groundCenter =
            new Vector3(
                bounds.center.x,
                bounds.min.y - _groundCheckDistance / 2f,
                transform.position.z
            );

        Vector3 groundSize =
            new Vector3(
                bounds.size.x * 0.8f,
                _groundCheckDistance,
                0f
            );

        Gizmos.DrawWireCube(
            groundCenter,
            groundSize
        );
    }
#endif
}