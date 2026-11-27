using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BounceGimmick : MonoBehaviour
{
    [Header("反発設定")]
    [SerializeField] private float bounceMultiplier = 1.5f;
    [SerializeField] private float maxBounceSpeed = 25f; // 反発速度の上限値

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();

            if (playerRb != null && playerCtrl != null)
            {
                Vector2 surfaceNormal = transform.up;
                ContactPoint2D contact = collision.contacts[0];

                if (Vector2.Dot(contact.normal, surfaceNormal) < -0.5f)
                {
                    Vector2 incomingVelocity = collision.relativeVelocity;
                    Vector2 normalVelocity = Vector2.Dot(incomingVelocity, surfaceNormal) * surfaceNormal;
                    Vector2 tangentVelocity = incomingVelocity - normalVelocity;

                    // フラグを見て倍率を決定（フラグオフ処理は削除）
                    float currentMultiplier = playerCtrl.canBounceBoost ? bounceMultiplier : 1.0f;

                    // 反発速度を計算し、上限値（maxBounceSpeed）でキャップする
                    float rawBounceSpeed = normalVelocity.magnitude * currentMultiplier;
                    float bounceSpeed = Mathf.Min(rawBounceSpeed, maxBounceSpeed);

                    Vector2 newNormalVelocity = surfaceNormal * bounceSpeed;

                    playerRb.linearVelocity = tangentVelocity + newNormalVelocity;
                }
            }
        }
    }
}