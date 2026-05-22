using UnityEngine;

public class TurretEnemy : Enemy
{
    private float last_shoot_time = 0f;

    [SerializeField] private GameObject enemy_bullet_prefab;
    [SerializeField] private Transform shoot_point;

    protected override void UpdateBehavior()
    {
        // 고정형이므로 플레이어 감지하면 발사
        if (player != null && player.IsAlive())
        {
            float distance_to_player = Vector2.Distance(transform.position, player.transform.position);
            
            if (distance_to_player <= detection_range)
            {
                TryShoot();
            }
        }
    }

    protected override void Move()
    {
        // 고정형이므로 움직이지 않음
        rb.linearVelocity = Vector2.zero;
    }

    protected override void OnPlayerDetected()
    {
        // 플레이어 감지 시 특별한 처리 없음
        // UpdateBehavior에서 발사 처리
    }

    private void TryShoot()
    {
        if (player == null || !player.IsAlive()) return;

        float shoot_cooldown = 1f / enemy_data.atk_speed;

        if (Time.time - last_shoot_time < shoot_cooldown)
            return;

        if (enemy_bullet_prefab == null)
        {
            Debug.LogError("Enemy Bullet prefab이 할당되지 않았습니다.");
            return;
        }

        Vector2 direction_to_player = (player.transform.position - transform.position).normalized;
        
        Vector3 spawn_pos = shoot_point != null ? shoot_point.position : transform.position;
        GameObject bullet_obj = Instantiate(enemy_bullet_prefab, spawn_pos, Quaternion.identity);
        
        EnemyBullet bullet = bullet_obj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction_to_player, enemy_data.bullet_speed, enemy_data.atk);
        }

        last_shoot_time = Time.time;
    }
}