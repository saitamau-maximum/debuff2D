using UnityEngine;

[System.Serializable]
public class DebuffStacks
{
    public int slow = 0;
    public int jump = 0;
    public int jumpCount = 2;
    public int fallAccelaration = 0;
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
        DebuffStacks stacks = useAutoGenerate
            ? GenerateStacks(currentFloor)
            : manualStacks;

        if (slowDebuff != null){
            slowDebuff.Apply(stacks.slow);
            slowDebuff.ChangeStatus();
            
        }
        if (jumpdebuff != null){
            jumpdebuff.Apply(stacks.jump);
            jumpdebuff.ChangeStatus();
        }
        if (limmitJump != null){
            limmitJump.Apply(stacks.jumpCount);
            limmitJump.ChangeStatus();
        }
        if (fallDebuff != null){
            fallDebuff.Apply(stacks.fallAccelaration);
            fallDebuff.ChangeStatus();
        }

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
