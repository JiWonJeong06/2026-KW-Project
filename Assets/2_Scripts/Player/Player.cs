using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject f_key_prompt; // F키 상호작용 스프라이트

    [Header("Weapon Visuals")]
    [SerializeField] private GameObject cyan_weapon;    // Cyan 무기 외형
    [SerializeField] private GameObject magenta_weapon; // Magenta 무기 외형
    [SerializeField] private GameObject yellow_weapon;  // Yellow 무기 외형

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibility_duration = 2f; // 무적 시간
    [SerializeField] private float blink_interval = 0.5f;       // 깜빡임 간격

    private PlayerData playerData;
    private Rigidbody2D rb;
    private Weapon weapon;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 move_input = Vector2.zero;
    private Vector2 current_direction = Vector2.right;

    private float current_hp;
    private bool is_alive = true;
    private bool is_invincible = false; // 무적 상태

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weapon = GetComponent<Weapon>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerData = PlayerDataLoader.Instance.GetPlayerData();

        if (playerData == null)
        {
            Debug.LogError("Player 데이터 로드 실패");
            return;
        }

        current_hp = playerData.hp;

        // F키 프롬프트 초기화
        if (f_key_prompt != null)
        {
            f_key_prompt.SetActive(false);
        }

        // 초기 무기 설정 (Cyan 우선)
        UpdateWeaponVisual();
    }

    private void Update()
    {
        if (!is_alive) return;

        HandleMovementInput();
        HandleShootInput();
    }

    private void FixedUpdate()
    {
        if (!is_alive) return;

        MovePlayer();
    }

    private void HandleMovementInput()
    {
        var keyboard = Keyboard.current;

        float horizontal = 0f;
        float vertical = 0f;

        // WASD 입력 처리 (대각선 가능)
        if (keyboard.wKey.isPressed) vertical += 1f;
        if (keyboard.sKey.isPressed) vertical -= 1f;
        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;

        move_input = new Vector2(horizontal, vertical).normalized;

        bool is_walking = move_input.sqrMagnitude > 0f;

        // Animator 이동 파라미터 업데이트
        animator.SetFloat("MoveX", move_input.x);
        animator.SetFloat("MoveY", move_input.y);
        animator.SetBool("isWalk", is_walking);
    }

    private void HandleShootInput()
    {
        // 안전지대에서는 공격 불가
        if (SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside())
        {
            animator.SetBool("isAttack", false);
            return;
        }

        var keyboard = Keyboard.current;
        Vector2 shoot_direction = Vector2.zero;
        bool is_shooting = false;

        // 화살표 입력 처리 (대각선 불가, 꾹 누르는 방식)
        if (keyboard.upArrowKey.isPressed)
        {
            shoot_direction = Vector2.up;
            is_shooting = true;
        }
        else if (keyboard.downArrowKey.isPressed)
        {
            shoot_direction = Vector2.down;
            is_shooting = true;
        }
        else if (keyboard.leftArrowKey.isPressed)
        {
            shoot_direction = Vector2.left;
            is_shooting = true;
        }
        else if (keyboard.rightArrowKey.isPressed)
        {
            shoot_direction = Vector2.right;
            is_shooting = true;
        }

        if (is_shooting)
        {
            current_direction = shoot_direction;
            weapon.Shoot(current_direction);

            animator.SetFloat("AttackX", shoot_direction.x);
            animator.SetFloat("AttackY", shoot_direction.y);
        }

        // 키를 누르고 있는 동안 isAttack = true, 떼면 false
        animator.SetBool("isAttack", is_shooting);
    }

    private void MovePlayer()
    {
        if (playerData == null) return;

        rb.linearVelocity = move_input * playerData.speed;
    }

    public void TakeDamage(float damage)
    {
        if (!is_alive) return;

        // 무적 상태면 데미지 무시
        if (is_invincible)
        {
            Debug.Log("[Player] 무적 상태 - 데미지 무시");
            return;
        }

        current_hp -= damage;
        Debug.Log($"플레이어 피해: {damage}, 남은 체력: {current_hp}");

        // 무적 시작
        StartCoroutine(InvincibilityCoroutine());

        if (current_hp <= 0)
            Die();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        is_invincible = true;
        float elapsed = 0f;

        // 원래 색상 저장
        Color original_color = spriteRenderer.color;

        // 무적 시간 동안 깜빡임
        while (elapsed < invincibility_duration)
        {
            // 투명하게 (Alpha 0.3)
            spriteRenderer.color = new Color(original_color.r, original_color.g, original_color.b, 0.1f);
            yield return new WaitForSeconds(blink_interval);

            // 불투명하게 (Alpha 1.0)
            spriteRenderer.color = original_color;
            yield return new WaitForSeconds(blink_interval);

            elapsed += blink_interval * 2;
        }

        // 무적 종료 - 원래 색상으로 복구
        spriteRenderer.color = original_color;
        is_invincible = false;

        Debug.Log("[Player] 무적 종료");
    }

    public void Heal(float amount)
    {
        if (!is_alive) return;

        current_hp = Mathf.Min(current_hp + amount, playerData.hp);
        Debug.Log($"플레이어 회복: {amount}, 현재 체력: {current_hp}");
    }

    private void Die()
    {
        is_alive = false;
        animator.SetBool("isWalk", false);
        animator.SetBool("isAttack", false);
        
        // 무적 Coroutine 중지
        StopAllCoroutines();
        
        // 색상 복구
        if (spriteRenderer != null)
        {
            Color original_color = spriteRenderer.color;
            spriteRenderer.color = new Color(original_color.r, original_color.g, original_color.b, 1f);
        }

        Debug.Log("플레이어 사망");
        gameObject.SetActive(false);
    }

    public PlayerData GetPlayerData() => playerData;
    public float GetCurrentHp() => current_hp;
    public float GetMaxHp() => playerData?.hp ?? 0;
    public Vector2 GetCurrentDirection() => current_direction;
    public bool IsAlive() => is_alive;
    public bool IsInvincible() => is_invincible; // 무적 상태 확인

    // F키 프롬프트 표시/숨김
    public void ShowFKeyPrompt()
    {
        if (f_key_prompt != null)
        {
            f_key_prompt.SetActive(true);
        }
    }

    public void HideFKeyPrompt()
    {
        if (f_key_prompt != null)
        {
            f_key_prompt.SetActive(false);
        }
    }

    // 무기 외형 업데이트 (증강 개수에 따라)
    public void UpdateWeaponVisual()
    {
        // PlayerStats에서 가장 많은 타입 가져오기 (우선순위: Cyan > Magenta > Yellow)
        string dominant_type = PlayerStats.Instance.GetDominantAbilityType();

        // 모든 무기 비활성화
        if (cyan_weapon != null) cyan_weapon.SetActive(false);
        if (magenta_weapon != null) magenta_weapon.SetActive(false);
        if (yellow_weapon != null) yellow_weapon.SetActive(false);

        // 우세한 타입의 무기만 활성화
        if (dominant_type == "Cyan" && cyan_weapon != null)
        {
            cyan_weapon.SetActive(true);
            Debug.Log("[Player] Cyan 무기 활성화");
        }
        else if (dominant_type == "Magenta" && magenta_weapon != null)
        {
            magenta_weapon.SetActive(true);
            Debug.Log("[Player] Magenta 무기 활성화");
        }
        else if (dominant_type == "Yellow" && yellow_weapon != null)
        {
            yellow_weapon.SetActive(true);
            Debug.Log("[Player] Yellow 무기 활성화");
        }
        else
        {
            Debug.LogWarning($"[Player] {dominant_type} 무기 GameObject가 없습니다!");
        }

        // Weapon 컴포넌트에 현재 타입 전달 (총알 스프라이트 변경)
        // 활성화된 무기에서 Weapon 컴포넌트 찾기
        Weapon active_weapon = null;
        if (cyan_weapon != null && cyan_weapon.activeSelf)
        {
            active_weapon = cyan_weapon.GetComponent<Weapon>();
        }
        else if (magenta_weapon != null && magenta_weapon.activeSelf)
        {
            active_weapon = magenta_weapon.GetComponent<Weapon>();
        }
        else if (yellow_weapon != null && yellow_weapon.activeSelf)
        {
            active_weapon = yellow_weapon.GetComponent<Weapon>();
        }

        if (active_weapon != null)
        {
            active_weapon.SetWeaponType(dominant_type);
        }
        else if (weapon != null)
        {
            // Player에 직접 붙어있는 Weapon 컴포넌트 사용
            weapon.SetWeaponType(dominant_type);
        }
        else
        {
            Debug.LogWarning("[Player] Weapon 컴포넌트를 찾을 수 없습니다!");
        }
    }
}