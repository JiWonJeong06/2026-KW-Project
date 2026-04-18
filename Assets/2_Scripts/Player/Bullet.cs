using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Atk;
    public float range;

    Rigidbody2D rb;
    Vector3 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    public void ApplyData(MyckaData data)
    {
        Atk = data.Atk;
        range = data.range;
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
            damageTarget.TakeDamage(Atk);
        }

        Destroy(gameObject);
    }
}