using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 메인 스크립트
/// 
/// 특징:
/// 1. MyckaData 직접 적용
/// 2. WASD로 이동 (MoveX, MoveY)
/// 3. 방향키로 공격 (AttackX, AttackY) - 아이작의 번제 스타일
/// 4. 이동과 공격 방향 독립적
/// 5. 무기 교체: 증강 카운트 기반 (Cyan > Magenta > Yellow)
/// </summary>
public class Player : MonoBehaviour
{
    // ===== 데이터 =====
    [Header("Data")]
    [SerializeField]
    private MyckaData myckaData;

    public float currentHp;

    // ===== 컴포넌트 =====
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField]
    private Animator animatorComponent;

    // ===== 입력 =====
    [Header("Input")]
    private Vector2 moveInput = Vector2.zero;
    private Vector2 attackInput = Vector2.zero;
    private Vector2 lastAttackDirection = Vector2.right;

    // ===== 상호작용 =====
    [Header("Interaction")]
    [SerializeField]
    private float interactRadius = 2f;

    [SerializeField]
    private LayerMask interactLayer;

    [SerializeField]
    private GameObject interactPrompt;

    // ===== 총알 =====
    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;

    private float nextFireTime = 0f;

    // ===== 무기 시스템 =====
    [Header("Weapon")]
    private string currentWeapon = "Cyan";  // 현재 무기 색상
    public int cyanCount = 0;               // Cyan 증강 횟수
    public int magentaCount = 0;            // Magenta 증강 횟수
    public int yellowCount = 0;             // Yellow 증강 횟수

    // ===== 능력 카운트 =====
    [Header("Ability Levels")]
    public int pierceLevel = 0;
    public int bleedLevel = 0;

    // ===== 캐시 =====
    private Collider2D[] overlapResults = new Collider2D[10];

    // ===== 이벤트 =====
    public delegate void PlayerDamageHandler(float damage);
    public event PlayerDamageHandler OnDamaged;

    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnDeath;

    public delegate void AbilityAddedHandler(string color);
    public event AbilityAddedHandler OnAbilityAdded;

    // ===== Getters =====
    public MyckaData Data => myckaData;
    public float CurrentHp => currentHp;
    public float MaxHp => myckaData.maxHp;
    public Vector2 Position => rb.position;
    public Vector2 LastAttackDirection => lastAttackDirection;
    public bool Pierce => myckaData.pierce;
    public bool Bleed => myckaData.bleed;
    public bool IsAlive => currentHp > 0;

    // ===== Lifecycle =====

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = animatorComponent != null ? animatorComponent : GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("[Player] Rigidbody2D가 없습니다");
    }

    void OnEnable()
    {
        if (myckaData != null)
        {
            currentHp = myckaData.maxHp;
            Debug.Log($"[Player] {myckaData.name} 로드됨 (HP: {currentHp})");
        }
    }

    void Start()
    {
        if (myckaData == null)
        {
            Debug.LogError("[Player] MyckaData가 할당되지 않았습니다");
            return;
        }

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        Debug.Log($"[Player] 초기화 완료: {myckaData.koreanName}");
        DebugPrintStats();
    }

    void Update()
    {
        if (!IsAlive) return;

        HandleMoveInput();
        HandleAttackInput();
        CheckInteractPrompt();

        // F 키로 상호작용
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        if (!IsAlive) return;

        Move();
    }

    // ===== 이동 입력 =====

    void HandleMoveInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x = 1;   // 오른쪽
        if (Keyboard.current.aKey.isPressed)
            moveInput.x = -1;  // 왼쪽
        if (Keyboard.current.wKey.isPressed)
            moveInput.y = 1;   // 위
        if (Keyboard.current.sKey.isPressed)
            moveInput.y = -1;  // 아래

        moveInput = moveInput.normalized;

        // 애니메이터 업데이트
        if (animator != null)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            animator.SetBool("isWalk", moveInput != Vector2.zero);
        }
    }

    void Move()
    {
        if (rb == null) return;

        rb.linearVelocity = moveInput * myckaData.moveSpeed;
    }

    // ===== 공격 입력 =====

    void HandleAttackInput()
    {
        attackInput = Vector2.zero;

        // 방향키로 공격 입력
        if (Keyboard.current.upArrowKey.isPressed)
            attackInput.y = 1;     // 위
        if (Keyboard.current.downArrowKey.isPressed)
            attackInput.y = -1;    // 아래
        if (Keyboard.current.leftArrowKey.isPressed)
            attackInput.x = -1;    // 왼쪽
        if (Keyboard.current.rightArrowKey.isPressed)
            attackInput.x = 1;     // 오른쪽

        attackInput = attackInput.normalized;

        // 공격 입력이 있으면 마지막 방향 업데이트
        if (attackInput != Vector2.zero)
        {
            lastAttackDirection = attackInput;
        }

        // 공격 실행
        if (Time.time >= nextFireTime && attackInput != Vector2.zero)
        {
            Fire();
            nextFireTime = Time.time + (1f / myckaData.attackSpeed);

            // 애니메이터 업데이트
            if (animator != null)
            {
                animator.SetFloat("AttackX", lastAttackDirection.x);
                animator.SetFloat("AttackY", lastAttackDirection.y);
                animator.SetBool("isAttack", true);
            }
        }
        else if (attackInput == Vector2.zero && animator != null)
        {
            animator.SetBool("isAttack", false);
        }
    }

    void Fire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("[Player] Bullet Prefab이 할당되지 않았습니다");
            return;
        }

        // 총알 생성
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            rb.position,
            Quaternion.identity
        );

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            // 총알 초기화
            bullet.Init(
                direction: lastAttackDirection,
                bulletSpeed: myckaData.bulletSpeed,
                bulletDamage: myckaData.attackDamage,
                bulletRange: myckaData.attackRange,
                pierce: myckaData.pierce,
                bleed: myckaData.bleed,
                weaponType: GetCurrentWeaponType()
            );
        }
    }

    /// <summary>
    /// 현재 무기 결정 (증강 카운트 기반)
    /// 우선순위: Cyan > Magenta > Yellow
    /// </summary>
    string GetCurrentWeaponType()
    {
        if (cyanCount >= magentaCount && cyanCount >= yellowCount)
            return "Cyan";
        else if (magentaCount >= cyanCount && magentaCount >= yellowCount)
            return "Magenta";
        else
            return "Yellow";
    }

    // ===== 상호작용 =====

    void CheckInteractPrompt()
    {
        if (rb == null) return;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            rb.position,
            interactRadius,
            overlapResults,
            interactLayer
        );

        bool hasTarget = hitCount > 0;
        if (interactPrompt != null)
            interactPrompt.SetActive(hasTarget);
    }

    void TryInteract()
    {
        if (rb == null) return;

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            rb.position,
            interactRadius,
            overlapResults,
            interactLayer
        );

        if (hitCount == 0) return;

        // 가장 가까운 객체 찾기
        Collider2D nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            float distance = Vector2.Distance(rb.position, overlapResults[i].ClosestPoint(rb.position));
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = overlapResults[i];
            }
        }

        if (nearest != null)
        {
            OpenedDoor door = nearest.GetComponentInParent<OpenedDoor>();
            door?.Interact();
        }
    }

    // ===== 데이터 적용 =====

    public void ApplyMyckaData(MyckaData data)
    {
        if (data == null)
        {
            Debug.LogError("[Player] MyckaData가 null입니다");
            return;
        }

        myckaData = data;
        currentHp = myckaData.maxHp;

        Debug.Log($"[Player] MyckaData 적용: {myckaData.koreanName}");
        DebugPrintStats();
    }

    // ===== 능력 관리 =====

    public void AddAbility(string colorName)
    {
        if (!IsAlive) return;

        switch (colorName)
        {
            case "Cyan":
                cyanCount++;
                break;
            case "Magenta":
                magentaCount++;
                break;
            case "Yellow":
                yellowCount++;
                break;
            default:
                Debug.LogWarning($"[Player] 알 수 없는 색상: {colorName}");
                return;
        }

        currentWeapon = GetCurrentWeaponType();
        OnAbilityAdded?.Invoke(colorName);

        Debug.Log($"[Player] 능력 추가: {colorName} → 현재 무기: {currentWeapon}");
    }

    public void ApplyAbilityEffect(string abilityName, float value)
    {
        if (!IsAlive) return;

        switch (abilityName)
        {
            case "attackDamage":
                myckaData.attackDamage += value;
                break;
            case "attackSpeed":
                myckaData.attackSpeed += value;
                break;
            case "bulletSpeed":
                myckaData.bulletSpeed += value;
                break;
            case "attackRange":
                myckaData.attackRange += value;
                break;
            case "additionalBullets":
                myckaData.additionalBullets += value;
                break;
            case "maxHp":
                myckaData.maxHp += value;
                currentHp += value;
                break;
            case "moveSpeed":
                myckaData.moveSpeed += value;
                break;
            default:
                Debug.LogWarning($"[Player] 알 수 없는 능력: {abilityName}");
                return;
        }

        Debug.Log($"[Player] 능력 효과 적용: {abilityName} +{value}");
    }

    // ===== 피해 & 사망 =====

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHp -= damage;
        if (currentHp < 0)
            currentHp = 0;

        OnDamaged?.Invoke(damage);
        Debug.Log($"[Player] 피해: {damage:F1} | HP: {currentHp:F1}/{myckaData.maxHp:F1}");

        if (currentHp <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("[Player] 플레이어 사망!");
        OnDeath?.Invoke();

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager?.GameOver();

        gameObject.SetActive(false);
    }

    // ===== 디버그 =====

    void DebugPrintStats()
    {
        if (myckaData == null) return;

        Debug.Log($"[Player] 스탯:\n" +
                  $"  이름: {myckaData.koreanName} ({myckaData.name})\n" +
                  $"  HP: {myckaData.maxHp:F1}\n" +
                  $"  공격력: {myckaData.attackDamage:F1}\n" +
                  $"  공격속도: {myckaData.attackSpeed:F2}\n" +
                  $"  총알속도: {myckaData.bulletSpeed:F1}\n" +
                  $"  사거리: {myckaData.attackRange:F1}\n" +
                  $"  이동속도: {myckaData.moveSpeed:F1}\n" +
                  $"  관통: {myckaData.pierce}\n" +
                  $"  출혈: {myckaData.bleed}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}