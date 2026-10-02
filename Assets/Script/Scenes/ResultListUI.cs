using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultListUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private ResultListRow rowTemplate;
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        // The inactive template lives outside Content and never appears in the list.
        foreach (Transform child in content)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
        scrollRect.StopMovement();
        statusText.gameObject.SetActive(false);

        try
        {
            var scores = HighScoreStore.GetBestScores(HighScoreStore.CurrentPlayerId);
            foreach (var score in scores)
            {
                var row = Instantiate(rowTemplate, content, false);
                row.name = "Floor " + score.Floor;
                row.SetScore(score);
                row.gameObject.SetActive(true);
            }
            if (scores.Count == 0)
                ShowStatus("No saved scores yet. Clear a floor to record your score.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            ShowStatus("Could not load saved scores.");
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void ShowStatus(string message)
    {
        statusText.text = message;
        statusText.gameObject.SetActive(true);
    }
}
