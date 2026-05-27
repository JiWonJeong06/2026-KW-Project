using UnityEngine;

public class OrbitBullet : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbit_speed = 90f;   // 회전 속도 (도/초)

    private Transform boss_transform;
    private float current_angle = 0f;
    private float damage = 1f;

    // 반경 변화
    private float radius_min = 1.5f;
    private float radius_max = 3.0f;
    private float radius_speed = 0.5f;
    private float radius_time = 0f;    // 사인파 시간 누적

    /// <summary>
    /// 공전 탄막 초기화
    /// </summary>
    public void Initialize(Transform boss, float start_angle, float dmg,
                           float r_min, float r_max, float r_speed)
    {
        boss_transform = boss;
        current_angle  = start_angle;
        damage         = dmg;
        radius_min     = r_min;
        radius_max     = r_max;
        radius_speed   = r_speed;

        // 각 탄막마다 사인파 위상을 다르게 → 자연스러운 물결
        radius_time = start_angle / 360f * Mathf.PI * 2f;

        Debug.Log($"[OrbitBullet] 생성: 시작각도 {start_angle}°, 반경 {r_min}~{r_max}, 데미지 {dmg}");
    }

    private void Update()
    {
        if (boss_transform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 반경 사인파 계산 (0~1 → min~max)
        radius_time += Time.deltaTime * radius_speed;
        float t = (Mathf.Sin(radius_time) + 1f) * 0.5f; // 0 ~ 1
        float current_radius = Mathf.Lerp(radius_min, radius_max, t);

        // 각도 회전
        current_angle += orbit_speed * Time.deltaTime;
        if (current_angle >= 360f) current_angle -= 360f;

        // 위치 계산
        float rad = current_angle * Mathf.Deg2Rad;
        float x = boss_transform.position.x + Mathf.Cos(rad) * current_radius;
        float y = boss_transform.position.y + Mathf.Sin(rad) * current_radius;

        transform.position = new Vector3(x, y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"[OrbitBullet] 플레이어 피격! 데미지: {damage}");
        }
    }
}