using UnityEngine;

/// <summary>
/// 보스 파도 패턴 - 가로 프리팹에 붙이는 컴포넌트
/// 한 방향으로 이동하다가 최대 거리 초과 시 자동 삭제
/// </summary>
public class WaveBullet : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float max_travel = 30f;  // 최대 이동 거리

    private Vector2 move_direction;
    private float move_speed;
    private float damage;
    private float traveled = 0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float speed, float dmg)
    {
        move_direction = direction.normalized;
        move_speed     = speed;
        damage         = dmg;

        // 왼쪽 방향이면 X 반전
        if (move_direction.x < 0)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(-s.x, s.y, s.z);
        }

        // 아래 방향이면 Y 반전
        if (move_direction.y < 0)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(s.x, -s.y, s.z);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = move_direction * move_speed;
        }
        else
        {
            transform.Translate(move_direction * move_speed * Time.fixedDeltaTime, Space.World);
        }

        traveled += move_speed * Time.fixedDeltaTime;

        if (traveled >= max_travel)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Wall/Door 충돌 시 소멸
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Destroy(gameObject);
            return;
        }

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"[WaveBullet] 파도 피격! 데미지: {damage}");
            // 파도는 관통 – Destroy 하지 않음
        }
    }
}