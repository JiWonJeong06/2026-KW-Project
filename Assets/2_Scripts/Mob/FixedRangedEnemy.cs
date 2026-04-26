using UnityEngine;

public class FixedRangedEnemy : Enemy
{
    [Header("Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private float nextAttackTime;

    void Update()
    {
        if (player == null)
            return;

        if (GetDistanceToPlayer() <= range)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        Fire();

        nextAttackTime = Time.time + atkspeed;
    }

    void Fire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab이 비어 있음");
            return;
        }

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        GameObject bulletObj = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();

        if (bullet != null)
        {
            Vector2 dir = GetDirectionToPlayer();
            bullet.Init(dir, bulletspeed, atk);
        }
    }
}