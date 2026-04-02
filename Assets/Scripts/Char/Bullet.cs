using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float speed)
    {
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 데미지 가능한 대상 확인
        var damageTarget = collision.GetComponent<Enemy>();
        if (damageTarget != null)
        {
            damageTarget.TakeDamage(damage);
        }

        // 어떤 물체든 부딪히면 총알 제거
        Destroy(gameObject);
    }
}