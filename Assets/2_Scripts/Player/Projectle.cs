using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D circle_collider;
    private SpriteRenderer sprite_renderer;

    private Vector2 direction;
    private float bullet_speed;
    private float attack_damage;
    private float max_range;
    private bool has_pierce;
    private bool has_bleed;

    private float traveled_distance = 0f;
    private bool is_active = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circle_collider = GetComponent<CircleCollider2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!is_active) return;

        rb.linearVelocity = direction * bullet_speed;

        traveled_distance += (direction * bullet_speed * Time.fixedDeltaTime).magnitude;

        if (traveled_distance >= max_range)
        {
            Deactivate();
        }
    }

    public void Initialize(Vector2 shoot_direction, float speed, float damage, float range, bool pierce, bool bleed, Sprite sprite = null)
    {
        direction = shoot_direction.normalized;
        bullet_speed = speed;
        attack_damage = damage;
        max_range = range;
        has_pierce = pierce;
        has_bleed = bleed;
        is_active = true;
        traveled_distance = 0f;

        // 방향에 맞는 스프라이트 적용
        if (sprite != null && sprite_renderer != null)
        {
            sprite_renderer.sprite = sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_active) return;

        // Wall, Door에 닿으면 무조건 사라짐 (pierce 무시)
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Deactivate();
            return;
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            HandleEnemyCollision(enemy);
        }
    }

    private void HandleEnemyCollision(Enemy enemy)
    {
        enemy.TakeDamage(attack_damage);

        if (has_bleed)
        {
            ApplyBleed(enemy);
        }

        if (!has_pierce)
        {
            Deactivate();
        }
    }

    private void ApplyBleed(Enemy enemy)
    {
        float bleed_damage = attack_damage * 0.15f;
        StartCoroutine(BleedCoroutine(enemy, bleed_damage, 6f, 0.5f));
    }

    private IEnumerator BleedCoroutine(Enemy enemy, float bleed_damage, float duration, float interval)
    {
        float elapsed_time = 0f;

        while (elapsed_time < duration && enemy != null && enemy.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(interval);

            if (enemy != null && enemy.gameObject.activeSelf)
            {
                enemy.TakeDamage(bleed_damage);
            }

            elapsed_time += interval;
        }
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