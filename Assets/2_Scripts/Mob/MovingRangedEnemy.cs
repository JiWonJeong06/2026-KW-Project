using UnityEngine;

public class MovingRangedEnemy : Enemy
{
    [Header("Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private float nextAttackTime;
    private Vector2 moveDirection;

    void Update()
    {
        if (player == null)
            return;

        float distance = GetDistanceToPlayer();

        if (distance > range)
        {
            moveDirection = GetDirectionToPlayer();
        }
        else
        {
            moveDirection = Vector2.zero;
            TryAttack();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (rb == null)
            return;

        Vector2 nextPosition = rb.position + moveDirection * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
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