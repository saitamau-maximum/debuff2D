using UnityEngine;

public class JumpDebuff : MonoBehaviour
{
    [Header("Jump Debuff Settings")]
    [SerializeField] private float multiplierPerStack = 0.9f;

    private int currentStacks = 0;
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }
    public float GetMultiplier()
    {
        return Mathf.Pow(multiplierPerStack, currentStacks);
    }
}
