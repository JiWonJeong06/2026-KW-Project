using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;
    public float range = 1f;

    Rigidbody2D rb;
    Vector3 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 dir, float speed)
    {
        startPos = transform.position;
        rb.linearVelocity = dir * speed;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(startPos, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var damageTarget = collision.GetComponent<Enemy>();
        if (damageTarget != null)
        {
            damageTarget.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}