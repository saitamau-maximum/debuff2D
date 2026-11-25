using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class ContactScorePenalty : MonoBehaviour
{
    [InspectorName("接触時の減点")]
    [Min(1)]
    [SerializeField] private int scorePenalty = 1;

    [InspectorName("減点間隔（秒）")]
    [Min(0.02f)]
    [SerializeField] private float penaltyInterval = 1f;

    private float nextPenaltyTime;

    private void OnCollisionEnter2D(Collision2D collision) => TryApplyPenalty(collision.collider);
    private void OnCollisionStay2D(Collision2D collision) => TryApplyPenalty(collision.collider);
    private void OnTriggerEnter2D(Collider2D other) => TryApplyPenalty(other);
    private void OnTriggerStay2D(Collider2D other) => TryApplyPenalty(other);

    private void TryApplyPenalty(Collider2D other)
    {
        Health player = other.GetComponentInParent<Health>();
        ScoreManager manager = ScoreManager.Instance;
        if (player == null || !player.CompareTag("Player") || player.IsDead || player.IsInvincible ||
            manager == null || manager.IsFinished || Time.time < nextPenaltyTime)
        {
            return;
        }

        manager.SubtractScore(scorePenalty);
        nextPenaltyTime = Time.time + penaltyInterval;
    }

    private void OnValidate()
    {
        scorePenalty = Mathf.Max(1, scorePenalty);
        penaltyInterval = Mathf.Max(0.02f, penaltyInterval);
    }
}
