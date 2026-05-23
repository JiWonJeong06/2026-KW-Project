using UnityEngine;

public class BombEnemy : Enemy
{
    private bool is_chasing = false;
    private Vector2 patrol_direction;
    private float direction_change_timer = 0f;

    [SerializeField] private float direction_change_interval = 2f;
    [SerializeField] private float explosion_radius = 1.5f;

    private static readonly Vector2[] directions =
    {
        Vector2.right, Vector2.left, Vector2.up, Vector2.down
    };

    protected override void Start()
    {
        base.Start();

        // 랜덤 초기 방향 설정
        patrol_direction = directions[Random.Range(0, directions.Length)];
        current_direction = patrol_direction;
        direction_change_timer = direction_change_interval;
    }

    protected override void UpdateBehavior()
    {
        // is_detected가 해제되면 추적 중단
        if (!is_detected)
            is_chasing = false;

        if (!is_chasing)
        {
            PatrolBehavior();
        }
        else
        {
            TracePlayer();
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
            
            patrol_direction = current_direction;
        }

        // 분리 벡터 합산 (밀집 방지)
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
            // 랜덤 방향으로 변경
            patrol_direction = directions[Random.Range(0, directions.Length)];
            current_direction = patrol_direction;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 충돌 시 폭발
        Player player_script = collision.gameObject.GetComponent<Player>();
        if (player_script != null && is_alive)
        {
            Explode();
            return;
        }

        // 벽이나 문과 충돌 시 방향 전환
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Door"))
        {
            HandleWallCollision();
        }
    }

    private void Explode()
    {
        if (!is_alive) return;

        // 폭발 범위 내의 모든 플레이어에게 피해
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_radius);

        foreach (Collider2D collider in colliders)
        {
            Player player_script = collider.GetComponent<Player>();
            if (player_script != null)
            {
                player_script.TakeDamage(enemy_data.atk);
                Debug.Log($"자폭 폭발: 플레이어에게 {enemy_data.atk} 피해");
            }
        }

        Die();
    }

    protected override void Die()
    {
        is_alive = false;
        Debug.Log($"{enemy_data.name} 폭발");
        Destroy(gameObject);
    }

    protected override void OnDrawGizmosSelected()
    {
        // 기본 Enemy 기즈모도 표시
        base.OnDrawGizmosSelected();

        // 폭발 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosion_radius);
    }
}