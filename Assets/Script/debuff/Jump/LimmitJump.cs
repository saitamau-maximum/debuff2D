using UnityEngine;

public class LimmitJump : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private int jumpCount = 2;
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
