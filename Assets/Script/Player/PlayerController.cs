using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private bool isDashing = false;

    [Header("接地判定")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("ステータス参照")]
    [SerializeField] private PlayerStatus status;


    [Header("見た目の設定")]
    [SerializeField] private SpriteRenderer spriteRenderer; // プレイヤーの見た目（スプライト）

    [Header("攻撃")]
    [SerializeField] private PlayerShooter playerShooter;

    // 外部から取得できる「向いている方向」（右なら 1f、左なら -1f）
    public float FacingDirection { get; private set; } = 1f;
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

        if (playerShooter == null)
        {
            playerShooter = GetComponent<PlayerShooter>();
        }
    }
    private void Start()
    {
        rb.gravityScale = status.Gravity;
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
        

        //追加
        UpdateFacing();

        // Jキーが押されたら PlayerShooter に発射を命令する
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (playerShooter != null)
            {
                playerShooter.Shoot();
            }
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
        moveInput = context.ReadValue<Vector2>().x;
    }

    // Send Messagesを使用するシーン用
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    // Player InputのJumpアクションから呼ばれる
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpRequested = true;
        }
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpRequested = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isDashing = context.ReadValueAsButton();
    }

    public void OnSprint(InputValue value)
    {
        isDashing = value.isPressed;
    }

    private void Move()
    {
        // ダッシュ中なら DashSpeed、それ以外なら MoveSpeed
        float currentSpeed = isDashing ? status.DashSpeed : status.MoveSpeed;

        // 目標の横方向速度
        float targetSpeed = moveInput * currentSpeed;

        // 地上と空中で加速度を変える
        float acceleration;

        if (isGrounded)
        {
            acceleration = status.GroundAcceleration;
        }
        else
        {
            acceleration = status.AirAcceleration;
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
        if (jumpCount >= status.MaxJumpCount)
        {
            jumpRequested = false;
            return;
        }

        // 上方向の速度を設定
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            status.JumpPower
        );

        jumpCount++;

        // ジャンプ入力を消費
        jumpRequested = false;
    }
    
    //追加
    private void UpdateFacing()
    {
        // 入力があった場合のみ向きを更新
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            FacingDirection = Mathf.Sign(moveInput); // 右なら 1、左なら -1

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = (FacingDirection < 0f);
            }
        }
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
