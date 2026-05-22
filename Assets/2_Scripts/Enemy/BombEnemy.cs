using UnityEngine;

public class BombEnemy : Enemy
{
    private bool is_chasing = false;
    private CircleCollider2D explosion_collider;

    protected override void Awake()
    {
        base.Awake();
        explosion_collider = GetComponent<CircleCollider2D>();
    }

    protected override void UpdateBehavior()
    {
        if (!is_chasing)
        {
            // 범위 내에서 상하좌우 움직임
            if (!IsWithinSpawnRange())
            {
                current_direction = -current_direction;
            }
        }
        else
        {
            // 플레이어 추적
            TracePlayer();
        }
    }

    protected override void Move()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌 시 폭발
        Player player_script = collision.GetComponent<Player>();
        if (player_script != null && is_alive)
        {
            Explode();
            return;
        }

        // 벽이나 문과 충돌 시 방향 전환
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            HandleWallCollision();
        }
    }

    private void Explode()
    {
        if (!is_alive) return;

        // 폭발 범위 내의 모든 플레이어와 적에게 피해
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_collider.radius);

        foreach (Collider2D collider in colliders)
        {
            // 플레이어에게 피해
            Player player_script = collider.GetComponent<Player>();
            if (player_script != null)
            {
                player_script.TakeDamage(enemy_data.atk);
                Debug.Log($"자폭 폭발: 플레이어에게 {enemy_data.atk} 피해");
            }

            // 다른 적에게는 피해 주지 않음 (자폭한 자신 제외)
        }

        Die();
    }

    protected override void Die()
    {
        is_alive = false;
        Debug.Log($"{enemy_data.name} 폭발");
        Destroy(gameObject);
    }
}