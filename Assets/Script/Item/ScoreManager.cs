using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    // シーン内でどこからでも呼び出せるようにする（シングルトン）
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coinText; // UIのテキスト
    private int score = 0;
    public int CoinCount { get; private set; }
    public bool IsFinished { get; private set; }

    public void FinishStage() => IsFinished = true;

    public void AddCoin()
    {
        if (IsFinished) return;
        CoinCount++;
        UpdateCoinText();
        AddScore();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinText();
    }

    // スコアを加算するメソッド
    Animator anim; // （animationなどは一旦置いておいてシンプルに）
    public void AddScore()
    {
        if (IsFinished) return;
        score = StageScoreCalculator.CoinScoreCalculate(CoinCount);
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coin: " + CoinCount;
        }
    }
}
