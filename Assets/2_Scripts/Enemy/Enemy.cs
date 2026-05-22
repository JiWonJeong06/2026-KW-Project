using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyItem enemy_data;
    protected Rigidbody2D rb;
    protected SpriteRenderer sprite_renderer;
    
    protected float current_hp;
    protected bool is_alive = true;
    
    protected Vector2 spawn_position;
    protected float spawn_range = 3f;
    protected Vector2 current_direction = Vector2.right;
    
    protected Player player;
    protected float detection_range;
    protected float min_distance_to_enemy = 1f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        spawn_position = transform.position;
    }

    protected virtual void Update()
    {
        if (!is_alive) return;

        CheckPlayerDetection();
        UpdateBehavior();
    }

    protected virtual void FixedUpdate()
    {
        if (!is_alive) return;

        Move();
    }

    public virtual void Initialize(int enemy_id)
    {
        enemy_data = EnemyDataLoader.Instance.GetEnemyData(enemy_id);
        
        if (enemy_data == null)
        {
            Debug.LogError($"Enemy ID {enemy_id} 데이터 로드 실패");
            return;
        }

        current_hp = enemy_data.hp;
        detection_range = enemy_data.range + 1f;
    }

    protected virtual void CheckPlayerDetection()
    {
        if (player == null || !player.IsAlive()) return;

        float distance_to_player = Vector2.Distance(transform.position, player.transform.position);
        
        if (distance_to_player <= detection_range)
        {
            OnPlayerDetected();
        }
    }

    protected abstract void UpdateBehavior();
    protected abstract void Move();
    protected abstract void OnPlayerDetected();

    protected virtual void HandleWallCollision()
    {
        // 벽에 닿으면 즉시 반대 방향
        current_direction = -current_direction;
    }

    protected virtual void HandleEnemyDistance()
    {
        // 주변 적들과의 거리 체크 (밀집 방지)
        Collider2D[] nearby_enemies = Physics2D.OverlapCircleAll(transform.position, min_distance_to_enemy);
        
        foreach (Collider2D collider in nearby_enemies)
        {
            Enemy other_enemy = collider.GetComponent<Enemy>();
            
            if (other_enemy != null && other_enemy != this)
            {
                // 다른 적으로부터 멀어지는 방향
                Vector2 away_direction = (transform.position - other_enemy.transform.position).normalized;
                current_direction = away_direction;
                break;
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (!is_alive) return;

        current_hp -= damage;
        Debug.Log($"{enemy_data.name} 피해: {damage}, 남은 체력: {current_hp}");

        if (current_hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        is_alive = false;
        Debug.Log($"{enemy_data.name} 사망");
        Destroy(gameObject);
    }

    protected bool IsWithinSpawnRange()
    {
        float distance_to_spawn = Vector2.Distance(transform.position, spawn_position);
        return distance_to_spawn <= spawn_range;
    }

    protected void FlipSprite(Vector2 direction)
    {
        if (direction.x < 0)
            sprite_renderer.flipX = true;
        else if (direction.x > 0)
            sprite_renderer.flipX = false;
    }

    public EnemyItem GetEnemyData() => enemy_data;
    public float GetCurrentHp() => current_hp;
    public float GetMaxHp() => enemy_data?.hp ?? 0;
    public bool IsAlive() => is_alive;
}