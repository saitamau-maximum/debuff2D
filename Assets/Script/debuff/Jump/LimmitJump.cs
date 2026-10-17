using UnityEngine;

public class LimmitJump : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private int jumpCount = 2;
    // 全メソッドはFloorDifficultySystem から呼ばれる(それ以外から呼ぶの禁止)
    public void Apply(int count)
    {
        jumpCount = count;
    }
    public void ChangeStatus()
    {
        playerStatus.ApplyDebuffValues(
            playerStatus.MoveSpeed,
            playerStatus.DashSpeed,
            playerStatus.GroundAcceleration,
            playerStatus.AirAcceleration,
            playerStatus.JumpPower,
            jumpCount,
            playerStatus.Gravity
        );
    }
}
