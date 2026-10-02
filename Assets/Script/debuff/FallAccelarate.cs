using UnityEngine;

public class FallAccelarate : MonoBehaviour
{
    [Header("Fall Debuff Settings")]
    [SerializeField] private float AddPerStack = 0.1f;
    [SerializeField] private float baseGravityScale = 3f;
    [SerializeField] private Rigidbody2D playerGravity;

    private int currentStacks = 0;

    // FloorDifficultySystem から呼ばれる
    public void Apply(int stacks)
    {
        currentStacks = stacks;
    }
    public void GetAdd()
    {
        playerGravity.gravityScale = (currentStacks * AddPerStack) + baseGravityScale;
    }
}
