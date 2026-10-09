using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BounceGimmick : MonoBehaviour
{
    [Header("反発倍率 (n)")]
    [SerializeField] private float bounceMultiplier = 1.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            // PlayerControllerも取得する
            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();

            if (playerRb != null && playerCtrl != null)
            {
                Vector2 surfaceNormal = transform.up;
                ContactPoint2D contact = collision.contacts[0];

                if (Mathf.Abs(Vector2.Dot(contact.normal, surfaceNormal)) > 0.5f)
                {
                    Vector2 incomingVelocity = collision.relativeVelocity;
                    Vector2 normalVelocity = Vector2.Dot(incomingVelocity, surfaceNormal) * surfaceNormal;
                    Vector2 tangentVelocity = incomingVelocity - normalVelocity;

                    // フラグを見て倍率を決定し、権利を消費する
                    float currentMultiplier = 1.0f;
                    if (playerCtrl.canBounceBoost)
                    {
                        currentMultiplier = bounceMultiplier;
                        playerCtrl.canBounceBoost = false; // 1度使ったらオフにする
                    }

                    float bounceSpeed = normalVelocity.magnitude * currentMultiplier;
                    Vector2 newNormalVelocity = surfaceNormal * bounceSpeed;

                    playerRb.linearVelocity = tangentVelocity + newNormalVelocity;
                }
            }
        }
    }
}