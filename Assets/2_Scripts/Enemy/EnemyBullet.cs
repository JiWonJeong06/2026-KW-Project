using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    
    private Vector2 direction;
    private float bullet_speed;
    private float attack_damage;
    
    private float traveled_distance = 0f;
    private float max_range = 10f;
    private bool is_active = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!is_active) return;

        // 이동
        rb.linearVelocity = direction * bullet_speed;
        
        // 이동 거리 누적
        traveled_distance += (direction * bullet_speed * Time.fixedDeltaTime).magnitude;

        // 범위 초과 시 사라짐
        if (traveled_distance >= max_range)
        {
            Deactivate();
        }
    }

    public void Initialize(Vector2 shoot_direction, float speed, float damage)
    {
        direction = shoot_direction.normalized;
        bullet_speed = speed;
        attack_damage = damage;
        is_active = true;
        traveled_distance = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_active) return;

        // Wall, Door 레이어에 닿으면 사라짐
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Deactivate();
            return;
        }

        // 플레이어와 충돌
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(attack_damage);
            Debug.Log($"적 총알: 플레이어에게 {attack_damage} 피해");
            Deactivate();
            return;
        }

        // 다른 적과의 충돌은 무시
    }

    private void Deactivate()
    {
        is_active = false;
        Destroy(gameObject);
    }

    public bool IsActive() => is_active;
    public float GetTraveledDistance() => traveled_distance;
    public float GetRemainingRange() => max_range - traveled_distance;
}