using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody2D rb;
    private float damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, float damage)
    {
        this.damage = damage;

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}