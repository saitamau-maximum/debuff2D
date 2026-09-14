using UnityEngine;

public class SlowDebuff : MonoBehaviour
{
    [Header("Slow Debuff Settings")]
    [SerializeField] private float multiplierPerStack = 0.8f;

    private int currentStacks = 0;

    // FloorDifficultySystem から呼ばれる
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }

    // PlayerController が毎フレーム参照する
    public float GetMultiplier()
    {
        // スタック1 → 0.8倍
        // スタック2 → 0.64倍
        // スタック3 → 0.512倍
        return Mathf.Pow(multiplierPerStack, currentStacks);
    }
}
