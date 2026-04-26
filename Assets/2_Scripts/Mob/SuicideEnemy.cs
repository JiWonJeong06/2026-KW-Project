using UnityEngine;

public class SuicideEnemy : Enemy
{
    [Header("Suicide Setting")]
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private float explosionRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 moveDirection;
    private bool isExploded = false;

    void Update()
    {
        if (player == null || isExploded)
            return;

        float distance = GetDistanceToPlayer();

        if (distance <= explosionRange)
        {
            Explode();
            return;
        }

        if (distance <= detectRange)
        {
            moveDirection = GetDirectionToPlayer();
        }
        else
        {
            moveDirection = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (isExploded)
            return;

        Move();
    }

    void Move()
    {
        if (rb == null)
            return;

        Vector2 nextPosition = rb.position + moveDirection * movespeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    void Explode()
    {
        isExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRange, playerLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Player playerTarget = hits[i].GetComponent<Player>();

            if (playerTarget != null)
            {
                playerTarget.TakeDamage(atk);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}