using UnityEngine;
public static class StageScoreCalculator
{
    [SerializeField] private static int coinValue = 1;
    [SerializeField] private static float timeToScore = 1f;
    public static int TotalScoreCalculate(int coinCount, float remaintime)
    {
        return coinCount * coinValue + (int)(remaintime * timeToScore);
    }
    public static int CoinScoreCalculate(int coinCount)
    {
        return coinCount * coinValue;
    }
    public static int TimeScoreCalcurate(float remaintime)
    {
        return (int)(remaintime * timeToScore);
    }
}
