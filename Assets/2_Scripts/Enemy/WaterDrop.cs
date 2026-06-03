using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 물방울 — X 고정, 위에서 아래로 수직 낙하 → 착지 데미지
/// </summary>
public class WaterDrop : MonoBehaviour
{
    [SerializeField] private float fall_speed    = 10f;  // 낙하 속도
    [SerializeField] private float splash_radius = 0.5f; // 착지 판정 반경

    private float land_y;
    private float damage;
    private bool  is_initialized = false;
    private bool  has_landed     = false;

    /// <summary>
    /// land_x : 착지 X (생성 위치의 X와 동일하게 맞춰서 호출)
    /// land_y : 착지 목표 Y
    /// dmg    : 데미지
    /// </summary>
    public void Initialize(float land_x, float land_y, float dmg)
    {
        // X를 착지 위치로 정렬 (Boss에서 이미 spawn 시 land_x로 생성하므로 일치)
        Vector3 pos = transform.position;
        transform.position = new Vector3(land_x, pos.y, 0f);

        this.land_y       = land_y;
        this.damage       = dmg;
        is_initialized    = true;

        Debug.Log($"[WaterDrop] 낙하 시작: ({land_x:F1}, {pos.y:F1}) → 착지 Y={land_y:F1}");
    }

    private void Update()
    {
        if (!is_initialized || has_landed) return;

        // 수직 낙하 (Y만 감소)
        transform.position += Vector3.down * fall_speed * Time.deltaTime;

        // 착지 판정
        if (transform.position.y <= land_y)
        {
            transform.position = new Vector3(transform.position.x, land_y, 0f);
            has_landed = true;
            StartCoroutine(SplashAndDestroy());
        }
    }

    private IEnumerator SplashAndDestroy()
    {
        Debug.Log($"[WaterDrop] 착지! pos={transform.position}");

        // 착지 순간 범위 데미지
        DealSplashDamage();

        // 잠깐 잔류 후 재판정 (느린 플레이어 대응)
        yield return new WaitForSeconds(0.2f);
        DealSplashDamage();

        Destroy(gameObject);
    }

    private void DealSplashDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, splash_radius);
        foreach (var hit in hits)
        {
            Player p = hit.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(damage);
                Debug.Log($"[WaterDrop] 피격! 데미지={damage}");
            }
        }
    }

    // 낙하 중 직접 충돌
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!is_initialized || has_landed) return;

        Player p = collision.GetComponent<Player>();
        if (p != null)
        {
            p.TakeDamage(damage);
            has_landed = true;
            Debug.Log($"[WaterDrop] 낙하 중 직접 피격! 데미지={damage}");
            Destroy(gameObject);
        }
    }
}