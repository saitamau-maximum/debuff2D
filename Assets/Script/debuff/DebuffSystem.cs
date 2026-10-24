using UnityEngine;

public enum DebuffType
{
    slow,
    jump,
    jumpCount,
    fallAccelaration,

    all,//すべてのデバフを対象とする
    DebuffTypeCount//デバフ種類数を保持するため必ず最後に置くこと.
}

[System.Serializable]
public class DebuffStacks
{
    public int slow = 0;
    public int jump = 0;
    public int jumpCount = 2;
    public int fallAccelaration = 0;
}
public class DebuffSystem : MonoBehaviour
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
    
    
    // 上に他のデバフスクリプトも追加可能
    
    
    private int currentFloor;
    private DebuffStacks additionalStack;
    private void Start()
    {
        currentFloor = ScreenManager.Instance.CurrentFloor;
        ResetAdditonalStack(DebuffType.all);
    }

    public void ApplyDebuffs()
    {
        status.ResetToBaseValues();
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
    public void ResetAdditonalStack(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.slow:
                additionalStack.slow = 0;
                break;
            case DebuffType.jump:
                additionalStack.jump = 0;
                break;
            case DebuffType.jumpCount:
                additionalStack.jumpCount = 0;
                break;
            case DebuffType.fallAccelaration:
                additionalStack.fallAccelaration = 0;
                break;
            case DebuffType.all:
                additionalStack.slow = 0;
                additionalStack.jump = 0;
                additionalStack.jumpCount = 0;
                additionalStack.fallAccelaration = 0;
                break;
            default:
                Debug.Log($"DebuffSystem.ResetAdditionalStask:invalid DebuffType");
                break;
        }
        ApplyDebuffs();
    }

    public void AddDebuffByOthers(int add, DebuffType type)
    {
        switch (type)
        {
            case DebuffType.slow:
                additionalStack.slow += add;
                break;
            case DebuffType.jump:
                additionalStack.jump += add;
                break;
            case DebuffType.jumpCount:
                additionalStack.jumpCount += add;
                break;
            case DebuffType.fallAccelaration:
                additionalStack.fallAccelaration += add;
                break;
            case DebuffType.all:
                additionalStack.slow += add;
                additionalStack.jump += add;
                additionalStack.jumpCount += add;
                additionalStack.fallAccelaration += add;
                break;
            default:
                Debug.Log($"DebuffSystem.AddDebuffByOthers:invalid DebuffType");
                break;
        }
        ApplyDebuffs();
    }

    // 全デバフのスタック数を階層から生成する
    private DebuffStacks GenerateStacks(int floor)
    {
        DebuffStacks stacks = new DebuffStacks();

        stacks.slow = floor / 2 + additionalStack.slow;
        stacks.jump = (floor + 1) / 4 + additionalStack.jump;
        stacks.fallAccelaration = (floor + 1) / 4 + additionalStack.fallAccelaration;
        stacks.jumpCount = (2 - (floor / 64)) > 0 ? 2 - (floor / 64) : 0;

        return stacks;
    }
}
