using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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
    private GameObject bulletPrefab;
    public float Atkspeed;
    public float Bulletspeed;
    public float addbullet;

    private float nextFireTime;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lookDirection = Vector2.down;

    //무기 총알
    [Header("BulletType")]
    public GameObject[] magentabullet;
    public GameObject[] cyanbullet;
    public GameObject[] yellowbullet;

    [Header("WeaponType")]
    public bool magentaweapon;
    public bool cyanweapon;
    public bool yellowweapon;

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

        if (Keyboard.current.dKey.isPressed){
            input.x = 1;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            input.x = -1;
    
        }

        if (Keyboard.current.wKey.isPressed)
        {
            input.y = 1;

        }
        else if (Keyboard.current.sKey.isPressed)
        {
            input.y = -1;

        }

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
        dir = Vector2.right;
    else if (Keyboard.current.leftArrowKey.isPressed)
        dir = Vector2.left;
    else if (Keyboard.current.upArrowKey.isPressed)
        dir = Vector2.up;
    else if (Keyboard.current.downArrowKey.isPressed)
        dir = Vector2.down;

    if (dir != Vector2.zero && Time.time >= nextFireTime)
    {
        Fire(dir);
        nextFireTime = Time.time + Atkspeed;
    }
}
void Fire(Vector2 dir)
{
    int bulletIndex = 0;

    if (dir == Vector2.right)
        bulletIndex = 0;
    else if (dir == Vector2.left)
        bulletIndex = 1;
    else if (dir == Vector2.up)
        bulletIndex = 2;
    else if (dir == Vector2.down)
        bulletIndex = 3;

    GameObject prefab = magentabullet[bulletIndex];
    GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);

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