using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject f_key_prompt;

    [Header("Weapon Visuals")]
    [SerializeField] private GameObject cyan_weapon;
    [SerializeField] private GameObject magenta_weapon;
    [SerializeField] private GameObject yellow_weapon;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibility_duration = 2f;
    [SerializeField] private float blink_interval = 0.01f;

    private PlayerData playerData;
    private Rigidbody2D rb;
    private Weapon weapon;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 move_input = Vector2.zero;
    private Vector2 current_direction = Vector2.right;

    private float current_hp;
    private bool is_alive = true;
    private bool is_invincible = false;

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

        if (f_key_prompt != null)
            f_key_prompt.SetActive(false);

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
        float vertical   = 0f;

        if (keyboard.wKey.isPressed) vertical   += 1f;
        if (keyboard.sKey.isPressed) vertical   -= 1f;
        if (keyboard.aKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed) horizontal += 1f;

        move_input = new Vector2(horizontal, vertical).normalized;

        bool is_walking = move_input.sqrMagnitude > 0f;

        // ── 걷기 사운드 ──
        if (is_walking) SoundManager.Instance?.StartWalk();
        else            SoundManager.Instance?.StopWalk();

        animator.SetFloat("MoveX", move_input.x);
        animator.SetFloat("MoveY", move_input.y);
        animator.SetBool("isWalk", is_walking);
    }

    private void HandleShootInput()
    {
        if (SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside())
        {
            animator.SetBool("isAttack", false);
            return;
        }

        var keyboard = Keyboard.current;
        Vector2 shoot_direction = Vector2.zero;
        bool is_shooting = false;

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
        if (is_invincible)
        {
            Debug.Log("[Player] 무적 상태 - 데미지 무시");
            return;
        }

        current_hp -= damage;
        Debug.Log($"플레이어 피해: {damage}, 남은 체력: {current_hp}");

        // ── 피격 사운드 ──
        SoundManager.Instance?.PlayPlayerHit();

        StartCoroutine(InvincibilityCoroutine());

        if (current_hp <= 0)
            Die();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        is_invincible = true;
        float elapsed = 0f;
        Color original_color = spriteRenderer.color;

        while (elapsed < invincibility_duration)
        {
            spriteRenderer.color = new Color(original_color.r, original_color.g, original_color.b, 0.1f);
            yield return new WaitForSeconds(blink_interval);
            spriteRenderer.color = original_color;
            yield return new WaitForSeconds(blink_interval);
            elapsed += blink_interval * 2;
        }

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

        // ── 사망 사운드 ──
        SoundManager.Instance?.StopWalk();
        SoundManager.Instance?.PlayPlayerDeath();

        StopAllCoroutines();

        if (spriteRenderer != null)
        {
            Color original_color = spriteRenderer.color;
            spriteRenderer.color = new Color(original_color.r, original_color.g, original_color.b, 1f);
        }

        Debug.Log("플레이어 사망");

        // ── 패배 결산창 (약간 딜레이 후 표시)
        ResultUI.Instance?.ShowLose();

        gameObject.SetActive(false);
    }

    public PlayerData GetPlayerData() => playerData;
    public float GetCurrentHp() => current_hp;
    public float GetMaxHp() => playerData?.hp ?? 0;
    public Vector2 GetCurrentDirection() => current_direction;
    public bool IsAlive() => is_alive;
    public bool IsInvincible() => is_invincible;

    public void ShowFKeyPrompt() { if (f_key_prompt != null) f_key_prompt.SetActive(true); }
    public void HideFKeyPrompt() { if (f_key_prompt != null) f_key_prompt.SetActive(false); }

    public void UpdateWeaponVisual()
    {
        string dominant_type = PlayerStats.Instance.GetDominantAbilityType();

        if (cyan_weapon    != null) cyan_weapon.SetActive(false);
        if (magenta_weapon != null) magenta_weapon.SetActive(false);
        if (yellow_weapon  != null) yellow_weapon.SetActive(false);

        if (dominant_type == "Cyan" && cyan_weapon != null)
        { cyan_weapon.SetActive(true); Debug.Log("[Player] Cyan 무기 활성화"); }
        else if (dominant_type == "Magenta" && magenta_weapon != null)
        { magenta_weapon.SetActive(true); Debug.Log("[Player] Magenta 무기 활성화"); }
        else if (dominant_type == "Yellow" && yellow_weapon != null)
        { yellow_weapon.SetActive(true); Debug.Log("[Player] Yellow 무기 활성화"); }
        else
        { Debug.LogWarning($"[Player] {dominant_type} 무기 GameObject가 없습니다!"); }

        Weapon active_weapon = null;
        if (cyan_weapon    != null && cyan_weapon.activeSelf)    active_weapon = cyan_weapon.GetComponent<Weapon>();
        else if (magenta_weapon != null && magenta_weapon.activeSelf) active_weapon = magenta_weapon.GetComponent<Weapon>();
        else if (yellow_weapon  != null && yellow_weapon.activeSelf)  active_weapon = yellow_weapon.GetComponent<Weapon>();

        if (active_weapon != null) active_weapon.SetWeaponType(dominant_type);
        else if (weapon != null)   weapon.SetWeaponType(dominant_type);
        else Debug.LogWarning("[Player] Weapon 컴포넌트를 찾을 수 없습니다!");
    }
}