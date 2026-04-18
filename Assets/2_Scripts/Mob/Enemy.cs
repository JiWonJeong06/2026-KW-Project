using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp;
    public float damage;
    public float attackRange;
    public float fireRate;
    float fireTimer;
    Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                fireTimer = 0f;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        bulletScript.Init(dir, damage);
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0)
            Destroy(gameObject);
    }
}