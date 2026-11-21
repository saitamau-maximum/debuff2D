using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("プレイヤー参照")]
    [Tooltip("未設定の場合はPlayerタグから自動取得します")]
    [SerializeField] private Health playerHealth;

    [Header("UIオブジェクト参照")]
    [SerializeField] private Transform heartContainer;       // Horizontal Layout Group を付けた親
    [SerializeField] private GameObject heartPrefab;          // ハート1個分のUI Prefab
    [SerializeField] private TextMeshProUGUI stackCountText;  // ハートの右下に表示する個数テキスト

    [Header("スタック表示設定")]
    [Tooltip("この個数以上になったらハート1個＋右下数字のスタック表示に切り替える")]
    [Min(2)]
    [SerializeField] private int stackThreshold = 5;

    private readonly List<GameObject> heartInstances = new List<GameObject>();
    private int lastDisplayedHealth = -1;

    private void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
        }

        if (stackCountText != null)
        {
            stackCountText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        int currentHP = Mathf.Max(0, playerHealth.CurrentHealth);

        // HPが変化したときだけUIを更新
        if (currentHP != lastDisplayedHealth)
        {
            RefreshUI(currentHP);
            lastDisplayedHealth = currentHP;
        }
    }

    private void RefreshUI(int currentHP)
    {
        // 0以下の場合はすべて非表示
        if (currentHP <= 0)
        {
            SetActiveHeartCount(0);
            if (stackCountText != null) stackCountText.gameObject.SetActive(false);
            return;
        }

        // 一定個数未満なら、HPの数だけハートを並べる
        if (currentHP < stackThreshold)
        {
            SetActiveHeartCount(currentHP);
            if (stackCountText != null) stackCountText.gameObject.SetActive(false);
        }
        // 一定個数以上なら、ハート1個＋右下に個数をスタック表示
        else
        {
            SetActiveHeartCount(1);
            if (stackCountText != null)
            {
                stackCountText.gameObject.SetActive(true);
                stackCountText.text = $"×{currentHP}";
            }
        }
    }

    private void SetActiveHeartCount(int count)
    {
        // 足りない分だけ動的に生成（後々HP上限が増えても自動対応）
        while (heartInstances.Count < count)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heartInstances.Add(heart);
        }

        for (int i = 0; i < heartInstances.Count; i++)
        {
            heartInstances[i].SetActive(i < count);
        }
    }
}