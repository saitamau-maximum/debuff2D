using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    // シーン内でどこからでも呼び出せるようにする（シングルトン）
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText; // UIのテキスト
    private int score = 0;
    public int CoinCount { get; private set; }
    public bool IsFinished { get; private set; }

    public void FinishStage() => IsFinished = true;

    public void AddCoin(int scoreValue)
    {
        if (IsFinished) return;
        CoinCount++;
        AddScore(scoreValue);
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
        UpdateScoreText();
    }

    // スコアを加算するメソッド
    Animator anim; // （animationなどは一旦置いておいてシンプルに）
    public void AddScore(int amount)
    {
        if (IsFinished) return;
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
