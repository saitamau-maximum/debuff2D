using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float dashSpeed = 12f;
    private bool isDashing = false;
    [SerializeField] private float groundAcceleration = 40f;
    [SerializeField] private float airAcceleration = 20f;

    [Header("ジャンプ")]
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("接地判定")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    // 左右の入力値
    private float moveInput;

    // 現在何回ジャンプしたか
    private int jumpCount;

    // 地面に接しているか
    private bool isGrounded;

    // ジャンプボタンが押されたか
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 地面にいるかを毎フレーム確認
        CheckGround();

        // 地面に着いたらジャンプ回数を戻す
        if (isGrounded)
        {
            jumpCount = 0;
        }
    }

    private void FixedUpdate()
    {
        Move();

        Jump();
    }

    // Player InputのMoveアクションから呼ばれる
    public void OnMove(InputAction.CallbackContext context)
    {
    Vector2 input = context.ReadValue<Vector2>();
    moveInput = input.x;
    }

    // Player InputのJumpアクションから呼ばれる
    public void OnJump(InputAction.CallbackContext context)
    {
      if (context.performed)
      {
          jumpRequested = true;
      }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
    bool pressed = context.ReadValueAsButton();

    Debug.Log("押されている？ " + pressed);

    isDashing = pressed;
    }

    private void Move()
    {
        // ダッシュ中なら dashSpeed、それ以外なら moveSpeed
        float currentSpeed = isDashing ? dashSpeed : moveSpeed;

        // 目標の横方向速度
        float targetSpeed = moveInput * currentSpeed;

        // 地上と空中で加速度を変える
        float acceleration;

        if (isGrounded)
        {
            acceleration = groundAcceleration;
        }
        else
        {
            acceleration = airAcceleration;
        }

        // 現在の横方向速度を目標速度へ近づける
        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newVelocityX,
            rb.linearVelocity.y
        );
    }

    private void Jump()
    {
        if (!jumpRequested)
        {
            return;
        }

        // 最大ジャンプ回数を超えていたら何もしない
        if (jumpCount >= maxJumpCount)
        {
            jumpRequested = false;
            return;
        }

        // 上方向の速度を設定
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpPower
        );

        jumpCount++;

        // ジャンプ入力を消費
        jumpRequested = false;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    // UnityのScene画面で接地判定を見やすくする
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}