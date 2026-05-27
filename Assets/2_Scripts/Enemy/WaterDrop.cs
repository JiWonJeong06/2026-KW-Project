using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 물방울 패턴 - 위에서 목표 지점으로 낙하 후 데미지
/// </summary>
public class WaterDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float fall_speed = 8f;         // 낙하 속도
    [SerializeField] private float splash_duration = 0.3f;  // 착지 후 잔류 시간
    [SerializeField] private float splash_radius = 0.4f;    // 착지 판정 반경

    private Vector3 target_position;
    private float damage;
    private bool is_falling = true;
    private bool has_splashed = false;

    public void Initialize(Vector3 land_pos, float dmg)
    {
        target_position = land_pos;
        damage = dmg;
    }

    private void Update()
    {
        if (!is_falling) return;

        // 목표 Y 좌표까지 낙하
        transform.position = Vector3.MoveTowards(
            transform.position,
            target_position,
            fall_speed * Time.deltaTime
        );

        // 목표 도달
        if (Vector3.Distance(transform.position, target_position) < 0.05f)
        {
            is_falling = false;
            StartCoroutine(Splash());
        }
    }

    private IEnumerator Splash()
    {
        if (has_splashed) yield break;
        has_splashed = true;

        // 착지 판정: 착지 순간 + 잔류 시간 동안 데미지 가능
        Collider2D hit = Physics2D.OverlapCircle(transform.position, splash_radius);
        if (hit != null)
        {
            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"[WaterDrop] 착지 피격! 데미지: {damage}");
            }
        }

        yield return new WaitForSeconds(splash_duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_falling) return;

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"[WaterDrop] 낙하 중 피격! 데미지: {damage}");
            is_falling = false;
            Destroy(gameObject);
        }
    }
}