using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 총알 스크립트
/// 
/// 특징:
/// 1. 관통(Pierce): Enemy 통과, 사거리 내에서만 작동
/// 2. 출혈(Bleed): Enemy 적중 시 3초 상태이상, 플레이어 공격력의 15% × 0.5초
/// 3. Enemy당 처음 충돌할 때만 데미지 적용
/// 4. Wall 충돌 시 즉시 사라짐
/// 5. 사거리 끝 → 사라짐
/// </summary>
public class Bullet : MonoBehaviour
{
    // ===== 데이터 =====
    private Vector2 direction;
    private float bulletSpeed;
    private float bulletDamage;
    private float bulletRange;
    private bool pierce;
    private bool bleed;
    private string weaponType; // "Cyan", "Magenta", "Yellow"

    // ===== 내부 변수 =====
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPosition;
    private float distanceTraveled = 0f;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();  // 이미 맞은 적들

    // ===== 색상 정의 =====
    private static readonly Color cyanColor = new Color(0, 1, 1);      // 시안
    private static readonly Color magentaColor = new Color(1, 0, 1);   // 마젠타
    private static readonly Color yellowColor = new Color(1, 1, 0);    // 옐로우

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("[Bullet] Rigidbody2D가 없습니다");
    }

    void Start()
    {
        startPosition = rb.position;
    }

    void FixedUpdate()
    {
        // 이동
        rb.linearVelocity = direction * bulletSpeed;

        // 이동 거리 계산
        distanceTraveled = Vector2.Distance(startPosition, rb.position);

        // 사거리 초과 → 사라짐
        if (distanceTraveled > bulletRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Wall 충돌 → 즉시 사라짐
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("[Bullet] 벽과 충돌");
            Destroy(gameObject);
            return;
        }

        // Enemy 충돌
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                OnEnemyHit(enemy);
            }
        }
    }

    /// <summary>
    /// 적과 충돌했을 때
    /// </summary>
    void OnEnemyHit(Enemy enemy)
    {
        // 이미 맞은 적이면 무시
        if (hitEnemies.Contains(enemy))
        {
            return;
        }

        // 처음 맞은 적이므로 등록
        hitEnemies.Add(enemy);

        // 기본 데미지 적용
        enemy.TakeDamage(bulletDamage);

        Debug.Log($"[Bullet] {enemy.mobName}에 {bulletDamage:F1} 데미지");

        // 출혈 효과 적용
        if (bleed)
        {
            float bleedDamage = bulletDamage * 0.15f;  // 공격력의 15%
            enemy.ApplyBleed(duration: 3f, damagePerTick: bleedDamage, tickInterval: 0.5f);
            Debug.Log($"[Bullet] {enemy.mobName}에 출혈 적용 (피해: {bleedDamage:F1}/0.5초 × 3초)");
        }

        // 관통 효과가 없으면 사라짐
        if (!pierce)
        {
            Debug.Log("[Bullet] 관통 효과 없음 → 사라짐");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[Bullet] 관통 효과 → 계속 이동");
        }
    }

    /// <summary>
    /// 총알 초기화
    /// </summary>
    public void Init(Vector2 direction, float bulletSpeed, float bulletDamage, 
                     float bulletRange, bool pierce, bool bleed, string weaponType)
    {
        this.direction = direction.normalized;
        this.bulletSpeed = bulletSpeed;
        this.bulletDamage = bulletDamage;
        this.bulletRange = bulletRange;
        this.pierce = pierce;
        this.bleed = bleed;
        this.weaponType = weaponType;

        // 색상 설정
        SetBulletColor(weaponType);

        Debug.Log($"[Bullet] 초기화: {weaponType}, 데미지={bulletDamage:F1}, 사거리={bulletRange:F1}, " +
                  $"관통={pierce}, 출혈={bleed}");
    }

    /// <summary>
    /// 무기 색상에 따라 총알 색상 설정
    /// </summary>
    void SetBulletColor(string weaponType)
    {
        if (spriteRenderer == null) return;

        switch (weaponType)
        {
            case "Cyan":
                spriteRenderer.color = cyanColor;
                break;
            case "Magenta":
                spriteRenderer.color = magentaColor;
                break;
            case "Yellow":
                spriteRenderer.color = yellowColor;
                break;
            default:
                spriteRenderer.color = Color.white;
                break;
        }
    }
}