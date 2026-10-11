using UnityEngine;

public class FallAccelarate : MonoBehaviour
{
    [Header("Fall Debuff Settings")]
    [SerializeField] private float AddPerStack = 0.1f;
    [SerializeField] private PlayerStatus playerStatus;

    private int currentStacks = 0;
    private float gravity;

    // 全メソッドはFloorDifficultySystem から呼ばれる(それ以外から呼ぶの禁止)
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }
    public void ChangeStatus()
    {
        gravity = (currentStacks * AddPerStack) + playerStatus.Gravity;
        playerStatus.ApplyDebuffValues(
            playerStatus.MoveSpeed,
            playerStatus.DashSpeed,
            playerStatus.GroundAcceleration,
            playerStatus.AirAcceleration,
            playerStatus.JumpPower,
            playerStatus.MaxJumpCount,
            gravity
        );
    }
}
