using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public float Atk = 1f;

    private Rigidbody2D rb;
    private float maxDistance = 50f;
    private Vector3 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // ===== 펫 데이터 적용 (PetDataLoader에서 호출) =====
    public void ApplyBulletData(PetData data)
    {
        Atk = data.atk;
        speed = data.bulletSpeed;
    }


    void FixedUpdate()
    {
        if (target == null)
        {
            // 타겟이 없으면 앞으로 이동
            rb.linearVelocity = transform.right * speed;
        }
        else
        {
            // 타겟을 향해 회전
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 타겟을 향해 이동
            rb.linearVelocity = transform.right * speed;

            // 최대 거리를 벗어나면 소멸
            if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 적에게 맞음
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Atk);
            }

            Destroy(gameObject);
        }
    }
}