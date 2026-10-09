using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool collected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ぶつかった相手がプレイヤーの場合
        if (!collected && collision.CompareTag("Player") && ScoreManager.Instance != null && !ScoreManager.Instance.IsFinished)
        {
            //スコア加算
            collected = true;
            ScoreManager.Instance.AddCoin();

            // アイテムオブジェクトを消去
            Destroy(gameObject);
        }
    }
}
