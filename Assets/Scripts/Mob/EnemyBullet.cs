using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;
    private float damage;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float dmg)
    {
        damage = dmg;
        rb.linearVelocity = dir * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Player에 데미지 함수 있다면 호출
            collision.GetComponent<Player>().TakeDamage(damage);

            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
         if (collision.CompareTag("Door"))
        {
            Destroy(gameObject);
        }
    }
}