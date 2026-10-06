using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    [Header("移動能力初期値")]
    [SerializeField] private float baseMoveSpeed = 6f;
    [SerializeField] private float baseDashSpeed = 12f;
    [SerializeField] private float baseGroundAcceleration = 40f;
    [SerializeField] private float baseAirAcceleration = 20f;

    [Header("ジャンプ能力初期値")]
    [SerializeField] private float baseJumpPower = 12f;
    [SerializeField] private int baseMaxJumpCount = 2;

    [Header("重力関係初期値")]
    [SerializeField] private float baseGravity = 3f;

    // 外部から読み取れるが書き換えられない参照用データ(デバフ適用済み)
    public float MoveSpeed { get; private set; }
    public float DashSpeed { get; private set; }
    public float GroundAcceleration { get; private set; }
    public float AirAcceleration { get; private set; }

    public float JumpPower { get; private set; }
    public int MaxJumpCount { get; private set; }

    public float Gravity { get; private set; }

    private void Awake()
    {
        Instance = this;

        // 初期値を最終値にコピー
        ResetToBaseValues();
    }

    private void ResetToBaseValues()
    {
        MoveSpeed = baseMoveSpeed;
        DashSpeed = baseDashSpeed;
        GroundAcceleration = baseGroundAcceleration;
        AirAcceleration = baseAirAcceleration;

        JumpPower = baseJumpPower;
        MaxJumpCount = baseMaxJumpCount;

        Gravity = baseGravity;
    }

    // デバフ適用後に呼ばれる唯一の窓口
    public void ApplyDebuffValues(
        float moveSpeed,
        float dashSpeed,
        float groundAccel,
        float airAccel,
        float jumpPower,
        int maxJumpCount,
        float gravity
    )
    {
        MoveSpeed = moveSpeed;
        DashSpeed = dashSpeed;
        GroundAcceleration = groundAccel;
        AirAcceleration = airAccel;

        JumpPower = jumpPower;
        MaxJumpCount = maxJumpCount;

        Gravity = gravity;
    }
}
