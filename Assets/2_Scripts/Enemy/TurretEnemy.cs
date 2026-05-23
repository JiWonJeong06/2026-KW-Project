using UnityEngine;

public class TurretEnemy : Enemy
{
    private float last_shoot_time = 0f;
    private bool is_shooting = false;

    [SerializeField] private GameObject enemy_bullet_prefab;
    [SerializeField] private Transform shoot_point;

    protected override void UpdateBehavior()
    {
        // is_detected가 해제되면 발사 중단
        if (!is_detected)
        {
            is_shooting = false;
        }
        else
        {
            // 감지 중이면 발사
            if (!is_shooting)
                is_shooting = true;

            TryShoot();
        }

        // Animator 파라미터 설정
        if (animator != null)
        {
            animator.SetBool("isAttack", is_shooting);
        }
    }

    protected override void Move()
    {
        // 고정형이므로 움직이지 않음 (Rigidbody2D를 Kinematic으로 설정 권장)
    }

    protected override void OnPlayerDetected()
    {
        is_shooting = true;
    }

    private void TryShoot()
    {
        if (player == null || !player.IsAlive()) return;

        float shoot_cooldown = 1f / enemy_data.atk_speed;

        if (Time.time - last_shoot_time < shoot_cooldown) return;

        if (enemy_bullet_prefab == null)
        {
            Debug.LogError("TurretEnemy: Enemy Bullet prefab이 할당되지 않았습니다.");
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