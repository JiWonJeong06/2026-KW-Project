using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject f_key_prompt; // F키 상호작용 스프라이트

    private PlayerData playerData;
    private Rigidbody2D rb;
    private Weapon weapon;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 move_input = Vector2.zero;
    private Vector2 current_direction = Vector2.right;

    private float current_hp;
    private bool is_alive = true;

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

        current_hp -= damage;
        Debug.Log($"플레이어 피해: {damage}, 남은 체력: {current_hp}");

        if (current_hp <= 0)
            Die();
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
        Debug.Log("플레이어 사망");
        gameObject.SetActive(false);
    }

    public PlayerData GetPlayerData() => playerData;
    public float GetCurrentHp() => current_hp;
    public float GetMaxHp() => playerData?.hp ?? 0;
    public Vector2 GetCurrentDirection() => current_direction;
    public bool IsAlive() => is_alive;

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
}