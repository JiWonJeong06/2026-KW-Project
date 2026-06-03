using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyItem enemy_data;
    protected Rigidbody2D rb;
    protected SpriteRenderer sprite_renderer;
    protected Animator animator;

    protected float current_hp;
    protected bool is_alive = true;
    protected bool is_detected = false;

    protected Vector2 spawn_position;
    protected float spawn_range = 3f;
    protected Vector2 current_direction = Vector2.right;

    protected Player player;
    protected float detection_range;
    protected float min_distance_to_enemy = 1f;

    [SerializeField] private int enemy_id;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        spawn_position = transform.position;

        // 인스펙터에서 설정한 ID로 자동 초기화
        // Spawner 구현 후에는 Spawner에서 Initialize()를 직접 호출
        if (enemy_id > 0)
        {
            Initialize(enemy_id);
        }
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

    public virtual void Initialize(int id)
    {
        enemy_data = EnemyDataLoader.Instance.GetEnemyData(id);

        if (enemy_data == null)
        {
            Debug.LogError($"Enemy: ID {id} 데이터 로드 실패");
            return;
        }

        current_hp = enemy_data.hp;
        detection_range = enemy_data.range + 1f;
    }

    protected virtual void CheckPlayerDetection()
    {
        if (player == null || !player.IsAlive()) return;

        // 안전지대 안의 플레이어는 감지하지 못함
        if (SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside())
        {
            is_detected = false; // 감지 상태 즉시 해제
            return;
        }

        float distance_to_player = Vector2.Distance(transform.position, player.transform.position);
        bool in_range = distance_to_player <= detection_range;

        // 최초 감지 시에만 OnPlayerDetected() 호출
        if (in_range && !is_detected)
        {
            is_detected = true;
            OnPlayerDetected();
        }
        else if (!in_range && is_detected)
        {
            // 감지 범위를 벗어나면 감지 상태 해제
            is_detected = false;
        }
    }

    protected abstract void UpdateBehavior();
    protected abstract void Move();
    protected abstract void OnPlayerDetected();

    protected virtual void HandleWallCollision()
    {
        current_direction = -current_direction;
    }

    // current_direction을 덮어쓰지 않고 분리 벡터만 반환
    // 호출부에서 current_direction과 합산하여 사용
    protected Vector2 GetSeparationForce()
    {
        Collider2D[] nearby_enemies = Physics2D.OverlapCircleAll(transform.position, min_distance_to_enemy);

        foreach (Collider2D collider in nearby_enemies)
        {
            Enemy other_enemy = collider.GetComponent<Enemy>();

            if (other_enemy != null && other_enemy != this)
            {
                return (transform.position - other_enemy.transform.position).normalized;
            }
        }

        return Vector2.zero;
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
        return Vector2.Distance(transform.position, spawn_position) <= spawn_range;
    }

    public EnemyItem GetEnemyData() => enemy_data;
    public float GetCurrentHp() => current_hp;
    public float GetMaxHp() => enemy_data?.hp ?? 0;
    public bool IsAlive() => is_alive;

    protected virtual void OnDrawGizmosSelected()
    {
        // 감지 범위 (녹색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detection_range);

        // 스폰 범위 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawn_position, spawn_range);

        // 밀집 방지 거리 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, min_distance_to_enemy);
    }
}