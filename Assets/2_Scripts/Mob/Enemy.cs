using UnityEngine;

/// <summary>
/// 기본 적 클래스
/// 
/// 출혈 특수 효과:
/// - 3초 동안 상태이상
/// - 플레이어 공격력의 15%만큼 0.5초마다 피해
/// </summary>
public class Enemy : MonoBehaviour
{
    // ===== 데이터 =====
    [Header("Data")]
    [SerializeField]
    public int mobID;

    [Header("Info")]
    [SerializeField]
    public string mobName = "Enemy";

    [SerializeField]
    public string mobType = "Normal";

    // ===== 스탯 (변수명 통일) =====
    [Header("Stat")]
    [SerializeField]
    public float attackDamage = 5f;

    [SerializeField]
    public float attackSpeed = 1f;

    [SerializeField]
    public float bulletSpeed = 10f;

    [SerializeField]
    public float moveSpeed = 2f;

    [SerializeField]
    public float attackRange = 5f;

    [SerializeField]
    public float maxHp = 20f;

    [SerializeField]
    public float currentHp;

    // ===== 감지 =====
    [Header("Vision")]
    [SerializeField]
    public float visionRange = 10f;

    [Range(0f, 360f)]
    [SerializeField]
    public float visionAngle = 90f;

    // ===== 출혈 상태이상 =====
    private float bleedEndTime = 0f;
    private float bleedDamagePerTick = 0f;
    private float bleedTickInterval = 0.5f;
    private float nextBleedTickTime = 0f;
    private bool isBleedActive = false;

    // ===== 내부 변수 =====
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected bool isPlayerDetected = false;

    // ===== Lifecycle =====

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        if (rb == null)
            Debug.LogError($"[{mobName}] Rigidbody2D가 없습니다");
    }

    protected virtual void Start()
    {
        FindPlayer();
        LoadData();
    }

    protected virtual void Update()
    {
        UpdateVision();
        UpdateBleed();
    }

    // ===== 플레이어 찾기 =====

    protected virtual void FindPlayer()
    {
        Player foundPlayer = FindFirstObjectByType<Player>();

        if (foundPlayer != null)
        {
            playerTransform = foundPlayer.transform;
        }
        else
        {
            Debug.LogWarning($"[{mobName}] Player를 찾을 수 없음");
        }
    }

    // ===== 데이터 로드 =====

    protected virtual void LoadData()
    {
        if (MobDataLoader.Instance == null)
        {
            Debug.LogWarning($"[{mobName}] MobDataLoader가 씬에 없음");
            return;
        }

        MobData data = MobDataLoader.Instance.GetMobData(mobID);

        if (data == null)
        {
            Debug.LogWarning($"[{mobName}] ID {mobID}의 데이터를 찾을 수 없음");
            return;
        }

        ApplyData(data);
    }

    public virtual void ApplyData(MobData data)
    {
        if (data == null) return;

        mobName = data.Name;
        mobType = data.Type;

        attackDamage = data.atk;
        attackSpeed = data.atkspeed;
        bulletSpeed = data.bulletspeed;
        maxHp = data.hp;
        currentHp = maxHp;
        moveSpeed = data.movespeed;
        attackRange = data.range;

        Debug.Log($"[{mobName}] 데이터 적용: ATK={attackDamage}, HP={maxHp}");
    }

    // ===== 시야 감지 =====

    protected virtual void UpdateVision()
    {
        if (playerTransform == null)
        {
            isPlayerDetected = false;
            return;
        }

        isPlayerDetected = IsInVision(playerTransform.position);
    }

    public bool IsInVision(Vector2 targetPosition)
    {
        float distance = Vector2.Distance(transform.position, targetPosition);
        if (distance > visionRange)
            return false;

        Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 facingDirection = GetFacingDirection();

        float angle = Vector2.Angle(facingDirection, directionToTarget);

        return angle <= visionAngle * 0.5f;
    }

    protected virtual Vector2 GetFacingDirection()
    {
        if (playerTransform == null)
            return Vector2.right;

        return ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
    }

    // ===== 출혈 상태이상 =====

    /// <summary>
    /// 출혈 상태이상 적용
    /// </summary>
    public void ApplyBleed(float duration, float damagePerTick, float tickInterval)
    {
        bleedEndTime = Time.time + duration;
        bleedDamagePerTick = damagePerTick;
        bleedTickInterval = tickInterval;
        nextBleedTickTime = Time.time;
        isBleedActive = true;

        Debug.Log($"[{mobName}] 출혈 적용: {duration}초, 피해={damagePerTick:F1}/{tickInterval}초");
    }

    void UpdateBleed()
    {
        if (!isBleedActive)
            return;

        // 출혈 지속 시간 끝남
        if (Time.time >= bleedEndTime)
        {
            isBleedActive = false;
            Debug.Log($"[{mobName}] 출혈 종료");
            return;
        }

        // 출혈 피해 적용
        if (Time.time >= nextBleedTickTime)
        {
            currentHp -= bleedDamagePerTick;
            if (currentHp < 0)
                currentHp = 0;

            nextBleedTickTime = Time.time + bleedTickInterval;

            Debug.Log($"[{mobName}] 출혈 피해: {bleedDamagePerTick:F1} | HP: {currentHp:F1}/{maxHp:F1}");

            if (currentHp <= 0)
                Die();
        }
    }

    // ===== 피해 & 사망 =====

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"[{mobName}] 사망");
        Destroy(gameObject);
    }

    // ===== 디버그 =====

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override string ToString()
    {
        return $"[{mobName}] Type={mobType}, ID={mobID}\n" +
               $"  HP: {currentHp}/{maxHp}\n" +
               $"  ATK: {attackDamage} (Speed: {attackSpeed}, Range: {attackRange})";
    }
}