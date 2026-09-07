using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1; // 獲得できるスコア

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ぶつかった相手がプレイヤーの場合
        if (collision.CompareTag("Player"))
        {
            //スコア加算
            ScoreManager.Instance.AddScore(scoreValue);

            // アイテムオブジェクトを消去
            Destroy(gameObject);
        }
    }
}