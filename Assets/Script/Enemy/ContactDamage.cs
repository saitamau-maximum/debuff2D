using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    [InspectorName("接触ダメージ")]
    [Min(1)]
    [SerializeField] private int damage = 1;

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D targetCollider)
    {
        Health health = targetCollider.GetComponentInParent<Health>();

        if (health == null)
        {
            return;
        }

        health.TakeDamage(damage);
    }

    private void OnValidate()
    {
        damage = Mathf.Max(1, damage);
    }
}
