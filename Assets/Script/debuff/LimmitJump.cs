using UnityEngine;

public class LimmitJump : MonoBehaviour
{
    private int jumpCount = 2;
    public void Apply(int count)
    {
        jumpCount = count;
    }
    public int LimmitMaxJumpCount()
    {
        return jumpCount;
    }
}
