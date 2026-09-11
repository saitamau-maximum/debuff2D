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

    [Header("Current Floor")]
    [SerializeField] private int currentFloor = 1;

    [Header("Debuff References")]
    [SerializeField] private SlowDebuff slowDebuff;
    // 他のデバフも追加可能
    

    private void Start()
    {
        ApplyDebuffs();
    }

    public void SetFloor(int floor)
    {
        currentFloor = floor;
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

        stacks.slow = floor;//いったんは階層が進むごとにslowデバフ

        return stacks;
    }
}
