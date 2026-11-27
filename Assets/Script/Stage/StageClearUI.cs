using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class StageClearUI : MonoBehaviour
{
    [SerializeField] private GameObject stageClearText;
    [SerializeField, Min(0f)] private float stageClearDuration = 2f;
    [SerializeField] private GameObject resultPanel;
    [SerializeField, Min(0f)] private float resultDuration = 3f;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text coinScoreText;
    [SerializeField] private TMP_Text TimeScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text statusText;
    private bool isShowing;
    private bool isLeaving;
    public bool IsConfigured => resultPanel != null && totalScoreText != null && highScoreText != null
        && resultPanel != gameObject && !transform.IsChildOf(resultPanel.transform);

    private void Awake()
    {
        if (stageClearText != null) stageClearText.SetActive(false);
        // Attach this component to an active Canvas, outside the hidden panel.
        if (resultPanel != null && resultPanel != gameObject && !transform.IsChildOf(resultPanel.transform))
            resultPanel.SetActive(false);
    }

    public void Show(int coinScore, int timeScore, int totalScore, int? previousBestScore, bool isNewBest, bool saveFailed)
    {
        if (isShowing) return;
        if (!IsConfigured)
        {
            Debug.LogError("Assign Result Panel, Score Text and High Score Text.", this);
            return;
        }
        resultPanel.SetActive(false);
        coinScoreText.text = "COIN SCORE: " + coinScore;
        TimeScoreText.text = "TIME SCORE: " + timeScore;
        totalScoreText.text = "TOTAL SCORE: " + totalScore;
        highScoreText.text = "HIGH SCORE: " + (previousBestScore.HasValue ? previousBestScore.Value.ToString() : "--");
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
        yield return new WaitForSecondsRealtime(resultDuration);
        ReturnToFloorSelect();
    }

    public void Retry()
    {
        if (isLeaving) return;
        isLeaving = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameObject.scene.name);
    }

    public void ReturnToFloorSelect()
    {
        if (isLeaving || ScreenManager.Instance == null) return;
        isLeaving = true;
        ScreenManager.Instance.ChangeScene(ScreenManager.SceneType.FloorSelect);
    }

    private void OnDestroy()
    {
        if (isShowing) Time.timeScale = 1f;
    }
}
