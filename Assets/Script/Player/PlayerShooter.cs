using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // 弾のプレハブ
    [SerializeField] private Transform firePoint;        // 発射位置

    private PlayerController playerController; // PlayerControllerへの参照

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // 弾を生成
            GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // PlayerControllerから現在の向きを取得
            float facingDir = (playerController != null) ? playerController.FacingDirection : 1f;

            // 弾に向きを伝える
            Projectile projectile = bulletObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetDirection(facingDir);
            }
        }
        else
        {
            Debug.LogWarning("弾のプレハブまたは発射位置が設定されていません！");
        }
    }
}