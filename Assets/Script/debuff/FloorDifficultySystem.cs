using UnityEngine;

[System.Serializable]
public class DebuffStacks
{
    public int slow = 0;
}
public class FloorDifficultySystem : MonoBehaviour
{
    [Header("Mode Settings")]
    [SerializeField] private bool useAutoGenerate = true;

    [Header("Manual Debuff Settings")]
    [SerializeField] private DebuffStacks manualStacks;

    [Header("Debuff References")]
    [SerializeField] private SlowDebuff slowDebuff;
    
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

        // Slow デバフ適用
        slowDebuff.Apply(stacks.slow);

    }

    // 全デバフのスタック数を階層から生成する
    private DebuffStacks GenerateStacks(int floor)
    {
        DebuffStacks stacks = new DebuffStacks();

        stacks.slow = floor - 1;//いったんは階層が進むごとにslowデバフ

        return stacks;
    }
}
