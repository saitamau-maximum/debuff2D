using UnityEngine;

public class JumpDebuff : MonoBehaviour
{
    [Header("Jump Debuff Settings")]
    [SerializeField] private float multiplierPerStack = 0.9f;
    [SerializeField] private PlayerStatus playerStatus;
    private float jumpPower;

    private int currentStacks = 0;
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }
    public void ChangeStatus()
    {
        jumpPower = playerStatus.JumpPower * Mathf.Pow(multiplierPerStack, currentStacks);
        playerStatus.ApplyDebuffValues(
            playerStatus.MoveSpeed,
            playerStatus.DashSpeed,
            playerStatus.GroundAcceleration,
            playerStatus.AirAcceleration,
            jumpPower,
            playerStatus.MaxJumpCount,
            playerStatus.Gravity
        );
    }
}
