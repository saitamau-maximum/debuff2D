using UnityEngine;

[System.Serializable]
public class DebuffStacks
{
    public int slow = 0;
    public int jump = 0;
    public int jumpCount = 2;
    public int fallAccelaration = 0;
}
public readonly struct ActiveDebuffStacks
{
    public int Slow { get; }
    public int JumpPowerReduction { get; }
    public int JumpCountReduction { get; }
    public int FallAcceleration { get; }

    public ActiveDebuffStacks(int slow, int jumpPowerReduction, int jumpCountReduction, int fallAcceleration)
    {
        Slow = slow;
        JumpPowerReduction = jumpPowerReduction;
        JumpCountReduction = jumpCountReduction;
        FallAcceleration = fallAcceleration;
    }
}

public class FloorDifficultySystem : MonoBehaviour
{
    [Header("Floor Debuff Calculate Mode Settings")]
    [SerializeField] private bool useAutoGenerate = true;

    [Header("Manual Debuff Settings")]
    [SerializeField] private DebuffStacks manualStacks;

    [Header("Debuff References")]
    [SerializeField] private SlowDebuff slowDebuff;
    [SerializeField] private JumpDebuff jumpdebuff;
    [SerializeField] private LimmitJump limmitJump;
    [SerializeField] private FallAccelarate fallDebuff;
    [SerializeField] private PlayerStatus status;
    
    
    // 他のデバフも追加可能
    
    private int currentFloor;
    private void Start()
    {
        currentFloor = ScreenManager.Instance.CurrentFloor;
        ApplyDebuffs();
    }

    public void ApplyDebuffs()
    {
        status.ResetToBaseValues();
        int baseJumpCount = status.MaxJumpCount;
        DebuffStacks stacks = useAutoGenerate
            ? GenerateStacks(currentFloor)
            : manualStacks;

        int slowStacks = 0;
        int jumpPowerStacks = 0;
        int fallStacks = 0;

        if (slowDebuff != null)
        {
            slowDebuff.Apply(stacks.slow);
            slowDebuff.ChangeStatus();
            slowStacks = stacks.slow;
        }
        if (jumpdebuff != null)
        {
            jumpdebuff.Apply(stacks.jump);
            jumpdebuff.ChangeStatus();
            jumpPowerStacks = stacks.jump;
        }
        if (limmitJump != null)
        {
            limmitJump.Apply(stacks.jumpCount);
            limmitJump.ChangeStatus();
        }
        if (fallDebuff != null)
        {
            fallDebuff.Apply(stacks.fallAccelaration);
            fallDebuff.ChangeStatus();
            fallStacks = stacks.fallAccelaration;
        }

        status.ActiveDebuffs = new ActiveDebuffStacks(
            slowStacks, jumpPowerStacks, Mathf.Max(0, baseJumpCount - status.MaxJumpCount), fallStacks);
    }

    // 全デバフのスタック数を階層から生成する
    private DebuffStacks GenerateStacks(int floor)
    {
        DebuffStacks stacks = new DebuffStacks();

        stacks.slow = floor / 2;
        stacks.jump = (floor + 1) / 4;
        stacks.fallAccelaration = (floor + 1) / 4;
        stacks.jumpCount = (2 - (floor / 64)) > 0 ? 2 - (floor / 64) : 0;

        return stacks;
    }
}
