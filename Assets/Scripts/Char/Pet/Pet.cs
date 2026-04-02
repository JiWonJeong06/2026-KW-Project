using UnityEngine;

public class Pet : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 3f;
    public float followDistance = 1.5f;
    public GameObject homingBulletPrefab;
    public float attackInterval = 30f;

    float timer;

    void Update()
    {
        FollowPlayer();
        AttackTimer();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        Vector3 targetPos = player.position;
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > followDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }
    }

    void AttackTimer()
    {
        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            FireToNearestEnemy();
        }
    }

    void FireToNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0) return;

        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        if (nearest != null)
        {
            GameObject bullet = Instantiate(
                homingBulletPrefab,
                transform.position,
                Quaternion.identity
            );

            bullet.GetComponent<HomingBullet>().target = nearest.transform;
        }
    }
}