using UnityEngine;

/// <summary>
/// 펫 유도탄 — 가장 가까운 적을 향해 추적
/// </summary>
public class PetBullet : MonoBehaviour
{
    [SerializeField] private float turn_speed = 200f;  // 유도 회전 속도 (도/초)

    private float  move_speed;
    private float  damage;
    private bool   is_active = false;

    private Transform target;

    public void Initialize(float speed, float dmg)
    {
        move_speed = speed;
        damage     = dmg;
        is_active  = true;

        // 발사 시점에 가장 가까운 적 탐색
        target = FindClosestEnemy();
    }

    private void Update()
    {
        if (!is_active) return;

        // 타겟이 없거나 죽었으면 다시 탐색
        if (target == null || !IsTargetAlive())
            target = FindClosestEnemy();

        if (target != null)
        {
            // 타겟 방향으로 회전
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float   new_angle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.z, angle, turn_speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, new_angle);
        }

        // 앞 방향으로 이동
        transform.Translate(Vector2.right * move_speed * Time.deltaTime);
    }

    private Transform FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform closest = null;
        float min_dist = float.MaxValue;

        foreach (var e in enemies)
        {
            if (!e.IsAlive()) continue;
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < min_dist)
            {
                min_dist = dist;
                closest  = e.transform;
            }
        }

        return closest;
    }

    private bool IsTargetAlive()
    {
        Enemy e = target.GetComponent<Enemy>();
        return e != null && e.IsAlive();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_active) return;

        // 벽 / 문 충돌 시 소멸
        if (collision.CompareTag("Wall") || collision.CompareTag("Door"))
        {
            Deactivate();
            return;
        }

        // 적 피격
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"[PetBullet] 적 피격! 데미지: {damage}");
            Deactivate();
        }
    }

    private void Deactivate()
    {
        is_active = false;
        Destroy(gameObject);
    }
}