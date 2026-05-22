using UnityEngine;

public class RangedEnemy : Enemy
{
    private float last_shoot_time = 0f;
    private bool is_chasing = false;
    private Vector2 random_direction;
    private float direction_change_timer = 0f;
    private float direction_change_interval = 2f;

    [SerializeField] private GameObject enemy_bullet_prefab;
    [SerializeField] private Transform shoot_point;

    protected override void Start()
    {
        base.Start();
        random_direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
        current_direction = random_direction;
    }

    protected override void UpdateBehavior()
    {
        if (!is_chasing)
        {
            // 플레이어를 감지하지 못했을 때 - 랜덤 방향 이동
            direction_change_timer -= Time.deltaTime;
            
            if (direction_change_timer <= 0)
            {
                random_direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
                direction_change_timer = direction_change_interval;
            }

            current_direction = random_direction;
        }
        else
        {
            // 플레이어 추적 중
            TracePlayer();
            TryShoot();
        }
    }

    protected override void Move()
    {
        // 범위 내에서만 이동
        if (!IsWithinSpawnRange())
        {
            current_direction = -current_direction;
        }

        // 주변 적과의 거리 체크
        HandleEnemyDistance();

        // 이동
        rb.linearVelocity = current_direction * enemy_data.move_speed;

        // 스프라이트 뒤집기
        FlipSprite(current_direction);
    }

    protected override void OnPlayerDetected()
    {
        is_chasing = true;
    }

    private void TracePlayer()
    {
        if (player == null || !player.IsAlive())
        {
            is_chasing = false;
            return;
        }

        Vector2 direction_to_player = (player.transform.position - transform.position).normalized;
        
        // 상하좌우만 (대각선 불가)
        if (Mathf.Abs(direction_to_player.x) > Mathf.Abs(direction_to_player.y))
        {
            current_direction = direction_to_player.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            current_direction = direction_to_player.y > 0 ? Vector2.up : Vector2.down;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            HandleWallCollision();
        }
    }
}