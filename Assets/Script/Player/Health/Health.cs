using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [Header("体力")]
    [InspectorName("最大HP")]
    [Min(1)]
    [SerializeField] private int maxHealth = 3;

    [InspectorName("無敵時間（秒）")]
    [Min(0f)]
    [SerializeField] private float invincibleDuration = 1f;

    [Header("リスポーン")]
    [InspectorName("リスポーン地点")]
    [Tooltip("死亡後にプレイヤーを戻す位置。未設定の場合はゲーム開始位置を使用します。")]
    [SerializeField] private Transform respawnPoint;

    [InspectorName("リスポーンまでの時間（秒）")]
    [Min(0f)]
    [SerializeField] private float respawnDelay = 1f;

    [Header("イベント")]
    [InspectorName("HP変更時")]
    [SerializeField] private UnityEvent<int, int> onHealthChanged;
    [InspectorName("ダメージを受けた時")]
    [SerializeField] private UnityEvent onDamaged;
    [InspectorName("死亡時")]
    [SerializeField] private UnityEvent onDeath;
    [InspectorName("リスポーン時")]
    [SerializeField] private UnityEvent onRespawn;

    private Rigidbody2D playerRigidbody;
    private Vector3 initialPosition;
    private int currentHealth;
    private float invincibleUntil;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsInvincible => !isDead && Time.time < invincibleUntil;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    /// <summary>
    /// プレイヤーにダメージを与えます。
    /// 敵や罠のスクリプトから呼び出してください。
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        if (damage <= 0 || isDead || IsInvincible)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        NotifyHealthChanged();
        onDamaged?.Invoke();

        if (currentHealth == 0)
        {
            StartCoroutine(RespawnRoutine());
            return;
        }

        invincibleUntil = Time.time + invincibleDuration;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || isDead || currentHealth >= maxHealth)
        {
            return;
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        NotifyHealthChanged();
    }

    /// <summary>
    /// HPの残量に関係なく、直ちに死亡処理を開始します。
    /// </summary>
    public void Kill()
    {
        if (isDead)
        {
            return;
        }

        currentHealth = 0;
        NotifyHealthChanged();
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isDead = true;
        onDeath?.Invoke();

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.simulated = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        transform.position = respawnPoint != null
            ? respawnPoint.position
            : initialPosition;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.simulated = true;
        }

        currentHealth = maxHealth;
        isDead = false;
        invincibleUntil = Time.time + invincibleDuration;

        NotifyHealthChanged();
        onRespawn?.Invoke();
    }

    private void NotifyHealthChanged()
    {
        Debug.Log($"{gameObject.name} HP: {currentHealth} / {maxHealth}", this);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        invincibleDuration = Mathf.Max(0f, invincibleDuration);
        respawnDelay = Mathf.Max(0f, respawnDelay);
    }

    private void OnDrawGizmosSelected()
    {
        if (respawnPoint == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(respawnPoint.position, 0.3f);
        Gizmos.DrawLine(
            respawnPoint.position + Vector3.left * 0.5f,
            respawnPoint.position + Vector3.right * 0.5f
        );
    }
}
