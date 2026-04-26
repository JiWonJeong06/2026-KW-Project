using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 이름
    [Header("Name")]
    public string Name;

    // 애니메이션
    [Header("Animation")]
    public Animator animator;

    // 이동
    [Header("Move")]
    public float speed;

    // 체력
    [Header("Health")]
    public float hp;
    public float currenthp;

    // 상호작용
    [Header("Interaction")]
    public float interactRadius;
    public LayerMask interactLayer;
    public GameObject interactPrompt;

    [Header("Special Effects")]
    public bool bleed = false;
    public bool pierce = false;

    // 무기
    [Header("Weapon")]
    public GameObject bulletPrefab;
    public float Atkspeed;
    public float Bulletspeed;
    public float addbullet;

    private float nextFireTime;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lookDirection = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator != null)
        {
            animator.SetFloat("MoveX", lookDirection.x);
            animator.SetFloat("MoveY", lookDirection.y);
            animator.SetBool("isWalk", false);
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Start()
    {
        currenthp = hp;
    }

    void Update()
    {
        InputMove();
        CheckInteractPrompt();

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Interact();
        }

        Shoot();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputMove()
    {
        input = Vector2.zero;

        if (Keyboard.current.dKey.isPressed)
            input.x = 1;
        else if (Keyboard.current.aKey.isPressed)
            input.x = -1;

        if (Keyboard.current.wKey.isPressed)
            input.y = 1;
        else if (Keyboard.current.sKey.isPressed)
            input.y = -1;

        input = input.normalized;

        if (input != Vector2.zero)
        {
            lookDirection = input;

            if (animator != null)
            {
                animator.SetFloat("MoveX", lookDirection.x);
                animator.SetFloat("MoveY", lookDirection.y);
            }
        }
    }

    void Move()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalk", input != Vector2.zero);
        }
    }

    void Shoot()
    {
        Vector2 dir = Vector2.zero;

        if (Keyboard.current.rightArrowKey.isPressed)
            dir.x += 1;
        else if (Keyboard.current.leftArrowKey.isPressed)
            dir.x -= 1;

        if (Keyboard.current.upArrowKey.isPressed)
            dir.y += 1;
        else if (Keyboard.current.downArrowKey.isPressed)
            dir.y -= 1;

        // 대각선 발사 방지
        if (dir.x != 0)
            dir.y = 0;

        if (dir != Vector2.zero && Time.time >= nextFireTime)
        {
            Fire(dir.normalized);
            nextFireTime = Time.time + Atkspeed;
        }
    }

    void Fire(Vector2 dir)
    {
        if (bulletPrefab == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Init(dir, Bulletspeed);
        }
    }

    void CheckInteractPrompt()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            rb.position,
            interactRadius,
            interactLayer
        );

        bool hasInteractTarget = hits.Length > 0;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(hasInteractTarget);
        }
    }

    void Interact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            rb.position,
            interactRadius,
            interactLayer
        );

        if (hits.Length == 0)
            return;

        Collider2D nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            float distance = Vector2.Distance(
                rb.position,
                hits[i].ClosestPoint(rb.position)
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = hits[i];
            }
        }

        if (nearest == null)
            return;

        OpenedDoor openedDoor = nearest.GetComponentInParent<OpenedDoor>();

        if (openedDoor != null)
        {
            openedDoor.Interact();
        }
    }

    // 캐릭터 데이터 적용
    public void ApplyData(MyckaData data)
    {
        Name = data.Name;
        hp = data.hp;
        speed = data.speed;
        currenthp = hp;

        bleed = data.bleed;
        pierce = data.pierce;

        Atkspeed = data.Atkspeed;
        Bulletspeed = data.Bulletspeed;
        addbullet = data.addbullet;
    }

    public void TakeDamage(float damage)
    {
        currenthp -= damage;

        if (currenthp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}