using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Animator animator;
    public float speed;
    public float maxHealth;
    public float currentHealth;

    Rigidbody2D rb;
    Vector2 input;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        InputMove();
        Move();
    }

    void InputMove()
    {
        input = Vector2.zero;

        if (Keyboard.current.dKey.isPressed)
            input.x = 1;

        if (Keyboard.current.aKey.isPressed)
            input.x = -1;

        if (Keyboard.current.wKey.isPressed)
            input.y = 1;

        if (Keyboard.current.sKey.isPressed)
            input.y = -1;

        input = input.normalized;
    }

    void Move()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("MoveX", input.x);
        animator.SetBool("isWalk", input != Vector2.zero);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망");
        gameObject.SetActive(false);
    }
}