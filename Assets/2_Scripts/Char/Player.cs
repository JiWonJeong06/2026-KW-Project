using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Move")]
    public Animator animator;
    public float speed = 5f;

    [Header("Health")]
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Interaction")]
    public float interactRadius = 1.5f;
    public LayerMask interactLayer;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lookDirection = Vector2.down;

    private NPC currentTargetNpc;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        animator.SetFloat("MoveX", lookDirection.x);
        animator.SetFloat("MoveY", lookDirection.y);
        animator.SetBool("isWalk", false);
    }

    void Update()
    {
        InputMove();
        UpdateInteractionTarget();

        if (Keyboard.current.eKey != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
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
            animator.SetFloat("MoveX", lookDirection.x);
            animator.SetFloat("MoveY", lookDirection.y);
        }
    }

    void Move()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
        animator.SetBool("isWalk", input != Vector2.zero);
    }

    void UpdateInteractionTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(rb.position, interactRadius, interactLayer);

        NPC nearestNpc = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            NPC npc = hits[i].GetComponent<NPC>();
            if (npc == null)
                continue;

            float distance = Vector2.Distance(rb.position, hits[i].transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNpc = npc;
            }
        }

        if (currentTargetNpc != nearestNpc)
        {
            if (currentTargetNpc != null)
                currentTargetNpc.SetHighlight(false);

            currentTargetNpc = nearestNpc;

            if (currentTargetNpc != null)
                currentTargetNpc.SetHighlight(true);
        }
    }

    void Interact()
    {
        if (currentTargetNpc != null)
        {
            currentTargetNpc.Interact();
        }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}