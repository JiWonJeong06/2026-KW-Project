using UnityEngine;

public class OrbitBullet : MonoBehaviour
{
    private Transform boss_transform;
    private float current_angle  = 0f;
    private float orbit_speed    = 90f;
    private float damage         = 1f;
    private float current_radius = 2f;

    public void Initialize(Transform boss, float start_angle, float dmg,
                           float radius, float speed)
    {
        boss_transform = boss;
        current_angle  = start_angle;
        damage         = dmg;
        orbit_speed    = speed;
        current_radius = radius;

        UpdatePosition();
    }

    /// <summary>Boss에서 매 프레임 반경을 동기화해서 넣어줌</summary>
    public void SetRadius(float radius)
    {
        current_radius = radius;
    }

    private void Update()
    {
        if (boss_transform == null)
        {
            Destroy(gameObject);
            return;
        }

        current_angle += orbit_speed * Time.deltaTime;
        if (current_angle >= 360f) current_angle -= 360f;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        float rad = current_angle * Mathf.Deg2Rad;
        float x   = boss_transform.position.x + Mathf.Cos(rad) * current_radius;
        float y   = boss_transform.position.y + Mathf.Sin(rad) * current_radius;
        transform.position = new Vector3(x, y, 0f);
    }

    public void SetAngle(float angle)
    {
        current_angle = angle;
        UpdatePosition();
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