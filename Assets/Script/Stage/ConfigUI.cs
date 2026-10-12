using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ConfigUI : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    private void Awake()
    {
        if(targetPanel != null && targetPanel != gameObject && !transform.IsChildOf(targetPanel.transform))
            targetPanel.SetActive(false);
    }
    public void Activate()
    {
        if (targetPanel == null)
        {
            Debug.LogError("ActivatePanelButton: targetPanel が設定されていません。", this);
            return;
        }

        targetPanel.SetActive(true);
        targetPanel.transform.SetAsLastSibling();
    }
}
