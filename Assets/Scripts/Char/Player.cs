using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 애니메이션 리스트
    enum Anime { Idle, Forward, Backward, Left, Right, AttackUp, AttackDown, AttackLeft, AttackRight, Hit, Die }

    public Animator animator; 
    public GameObject bulletPrefab;   // GameObject로 변경 (중요)

    public float speed = 5f;
    public float bulletSpeed = 10f;
    public int maxHealth = 5;
    public int currentHealth;
    public float attackspeed = 0.8f;
    private float lastAttackTime = 0f;
    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // 상하좌우 움직임
        Vector2 input = Vector2.zero;

        if (Keyboard.current.sKey.isPressed)
            input.y = -1;
        if (Keyboard.current.wKey.isPressed)
            input.y = 1;
        if (Keyboard.current.aKey.isPressed)
            input.x = -1;
        if (Keyboard.current.dKey.isPressed)
            input.x = 1;

        Vector3 movement = new Vector3(input.x, input.y, 0.0f).normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("Speed", input.sqrMagnitude);

        // 방향키 공격
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Shoot(Vector2.up);
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            Shoot(Vector2.down);
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            Shoot(Vector2.left);
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Shoot(Vector2.right);
        }
    }

void Shoot(Vector2 direction)
{
    // 공격 속도 제한
    if (Time.time - lastAttackTime < attackspeed)
        return;

    lastAttackTime = Time.time;

    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.linearVelocity = direction * bulletSpeed;
    }
}
}