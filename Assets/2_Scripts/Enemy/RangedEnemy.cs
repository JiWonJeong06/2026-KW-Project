using UnityEngine;

public class RangedEnemy : Enemy
{
    private float last_shoot_time = 0f;
    private bool is_chasing = false;
    private Vector2 random_direction;
    private float direction_change_timer = 0f;

    [SerializeField] private float direction_change_interval = 2f;
    [SerializeField] private GameObject enemy_bullet_prefab;
    [SerializeField] private Transform shoot_point;

    private static readonly Vector2[] patrol_directions =
    {
        Vector2.right, Vector2.left, Vector2.up, Vector2.down
    };

    protected override void Start()
    {
        base.Start();

        // 4방향 중 랜덤으로 초기 순찰 방향 설정
        random_direction = patrol_directions[Random.Range(0, patrol_directions.Length)];
        current_direction = random_direction;

        // 처음부터 자연스럽게 이동하도록 타이머를 최대값으로 초기화
        direction_change_timer = direction_change_interval;
    }

    protected override void UpdateBehavior()
    {
        // Enemy.cs의 is_detected가 해제되면 추적 상태도 해제
        if (!is_detected)
            is_chasing = false;

        if (!is_chasing)
        {
            PatrolBehavior();
        }
        else
        {
            TracePlayer();
            TryShoot();
        }
    }

    protected override void Move()
    {
        // 순찰 중에만 스폰 범위 제한 적용
        if (!is_chasing && !IsWithinSpawnRange())
        {
            // 스폰 위치로 돌아가기
            Vector2 direction_to_spawn = (spawn_position - (Vector2)transform.position).normalized;
            
            // 상하좌우만 (대각선 불가)
            if (Mathf.Abs(direction_to_spawn.x) > Mathf.Abs(direction_to_spawn.y))
                current_direction = direction_to_spawn.x > 0 ? Vector2.right : Vector2.left;
            else
                current_direction = direction_to_spawn.y > 0 ? Vector2.up : Vector2.down;
            
            random_direction = current_direction;
        }

        // 분리 벡터를 합산하여 밀집 방지 (current_direction 보존)
        Vector2 separation = GetSeparationForce();
        Vector2 final_direction = (current_direction + separation).normalized;

        rb.linearVelocity = final_direction * enemy_data.move_speed;

        // Animator 파라미터 설정
        if (animator != null)
        {
            bool is_moving = rb.linearVelocity.sqrMagnitude > 0.01f;
            animator.SetBool("isMove", is_moving);
            animator.SetFloat("MoveX", current_direction.x);
            animator.SetFloat("MoveY", current_direction.y);
        }
    }

    protected override void OnPlayerDetected()
    {
        is_chasing = true;
    }

    private void PatrolBehavior()
    {
        direction_change_timer -= Time.deltaTime;

        if (direction_change_timer <= 0f)
        {
            // 현재 방향을 제외한 3방향 중 랜덤 선택
            Vector2 new_direction;
            do
            {
                new_direction = patrol_directions[Random.Range(0, patrol_directions.Length)];
            }
            while (new_direction == random_direction);

            random_direction = new_direction;
            current_direction = random_direction;
            direction_change_timer = direction_change_interval;
        }
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
            current_direction = direction_to_player.x > 0 ? Vector2.right : Vector2.left;
        else
            current_direction = direction_to_player.y > 0 ? Vector2.up : Vector2.down;
    }

    private void TryShoot()
    {
        if (player == null || !player.IsAlive()) return;

        float shoot_cooldown = 1f / enemy_data.atk_speed;

        if (Time.time - last_shoot_time < shoot_cooldown) return;

        if (enemy_bullet_prefab == null)
        {
            Debug.LogError("RangedEnemy: Enemy Bullet prefab이 할당되지 않았습니다.");
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