using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // 弾のプレハブ
    [SerializeField] private Transform firePoint;        // 発射位置

    private void Update()
    {
        // Jキーが押されたら弾を生成
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("弾のプレハブまたは発射位置が設定されていません！");
        }
    }
}