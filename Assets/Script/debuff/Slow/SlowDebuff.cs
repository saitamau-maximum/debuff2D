using UnityEngine;

public class SlowDebuff : MonoBehaviour
{
    [Header("Slow Debuff Settings")]
    [SerializeField] private float multiplierPerStack = 0.9f;
    [SerializeField] private PlayerStatus playerStatus;

    private int currentStacks = 0;
    private float moveSpeed;
    private float dashSpeed;

    // 全メソッドはFloorDifficultySystem から呼ばれる(それ以外から呼ぶの禁止)
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }
    public void ChangeStatus()
    {
        moveSpeed = playerStatus.MoveSpeed * Mathf.Pow(multiplierPerStack, currentStacks);
        dashSpeed = playerStatus.DashSpeed * Mathf.Pow(multiplierPerStack, currentStacks);
        playerStatus.ApplyDebuffValues(
            moveSpeed,
            dashSpeed,
            playerStatus.GroundAcceleration,
            playerStatus.AirAcceleration,
            playerStatus.JumpPower,
            playerStatus.MaxJumpCount,
            playerStatus.Gravity);
    }
}
