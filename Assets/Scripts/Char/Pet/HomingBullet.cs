using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float damage = 1f;

    public Vector2 explosionSize = new Vector2(2f, 2f); // 2x2 범위
    public GameObject explosionEffectPrefab;
    public float effectDuration = 0.3f;
    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        Vector2 center = transform.position;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, explosionSize, 0f);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        //  범위 시각화 생성
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(effect, effectDuration);
        }

        Destroy(gameObject);
    }
}
}