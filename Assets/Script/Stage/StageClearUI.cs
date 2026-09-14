using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class StageClearUI : MonoBehaviour
{
    [SerializeField] private GameObject stageClearText;
    [SerializeField, Min(0f)] private float stageClearDuration = 2f;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text statusText;
    private bool isShowing;
    public bool IsConfigured => resultPanel != null && scoreText != null && highScoreText != null
        && resultPanel != gameObject && !transform.IsChildOf(resultPanel.transform);

    private void Awake()
    {
        if (stageClearText != null) stageClearText.SetActive(false);
        // Attach this component to an active Canvas, outside the hidden panel.
        if (resultPanel != null && resultPanel != gameObject && !transform.IsChildOf(resultPanel.transform))
            resultPanel.SetActive(false);
    }

    public void Show(int score, int? previousBestScore, bool isNewBest, bool saveFailed)
    {
        if (isShowing) return;
        if (!IsConfigured)
        {
            Debug.LogError("Assign Result Panel, Score Text and High Score Text.", this);
            return;
        }
        resultPanel.SetActive(false);
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + (previousBestScore.HasValue ? previousBestScore.Value.ToString() : "--");
        if (statusText != null)
            statusText.text = saveFailed ? "Save failed" : isNewBest ? "New High Score!" : "";
        isShowing = true;
        Time.timeScale = 0f;
        StartCoroutine(ShowSequence());
    }

    private IEnumerator ShowSequence()
    {
        if (stageClearText != null)
        {
            stageClearText.SetActive(true);
            stageClearText.transform.SetAsLastSibling();
            // ゲーム停止中も実時間で待ってからリザルトに切り替える。
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, stageClearDuration));
            stageClearText.SetActive(false);
        }
        resultPanel.SetActive(true);
        resultPanel.transform.SetAsLastSibling();
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.name);
    }

    public void ReturnToFloorSelect()
    {
        if (ScreenManager.Instance != null)
            ScreenManager.Instance.ChangeScene(ScreenManager.SceneType.FloorSelect);
    }

    private void OnDestroy()
    {
        if (isShowing) Time.timeScale = 1f;
    }
}
