using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public enum DebuffType
{
    slow,
    jump,
    jumpCount,
    fallAccelaration,
    DebuffTypeCount,//デバフ種類数を保持するため必ずデバフ種類の最後に置くこと.
    all//すべてのデバフを対象とする
    
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
    [SerializeField] private GameObject player;

    private SlowDebuff slowDebuff;
    private JumpDebuff jumpDebuff;
    private LimmitJump limmitJump;
    private FallAccelarate fallDebuff;
    private PlayerStatus status;
    
    
    // 上に他のデバフスクリプトも追加可能
    
    
    private int currentFloor;
    private DebuffStacks additionalStack;
    private void Start()
    {
        additionalStack = new DebuffStacks();
        slowDebuff = player.GetComponent<SlowDebuff>();
        jumpDebuff = player.GetComponent<JumpDebuff>();
        limmitJump = player.GetComponent<LimmitJump>();
        fallDebuff = player.GetComponent<FallAccelarate>();
        status = player.GetComponent<PlayerStatus>();
        currentFloor = ScreenManager.Instance.CurrentFloor;
        InitializeAdditonalStack(DebuffType.all);
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
        if (jumpDebuff != null){
            jumpDebuff.Apply(stacks.jump);
            jumpDebuff.ChangeStatus();
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
    private void InitializeAdditonalStack(DebuffType type)
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
    public void ResetTemporaryDebuffs()
    {
        StopAllCoroutines();

        InitializeAdditonalStack(DebuffType.all);
    }


    private void AddStack(int add, DebuffType type)
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
    public void AddTemporaryDebuff(
    int add,
    DebuffType type,
    float duration)
    {
        StartCoroutine(
            TemporaryDebuffCoroutine(
                add,
                type,
                duration));
    }
    private IEnumerator TemporaryDebuffCoroutine(
    int add,
    DebuffType type,
    float duration)
    {
        AddStack(add, type);

        yield return new WaitForSeconds(duration);

        AddStack(-add, type);
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
