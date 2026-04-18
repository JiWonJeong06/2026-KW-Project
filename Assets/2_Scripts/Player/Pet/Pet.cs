using UnityEngine;

public class Pet : MonoBehaviour
{
    [Header("Follow")]
    public Transform player;
    public float followSpeed;
    public float followDistance;

    [Header("Attack")]
    public GameObject homingBulletPrefab;
    public float petcd;
    public float pethp;
    public float petaddbullet;

    private float timer;

    void FixedUpdate()
    {
        FollowPlayer();
        AttackTimer();
    }

    public void ApplyPetData(MyckaData data)
    {
        petcd = data.petcd;
        pethp = data.pethp;
        petaddbullet = data.petaddbullet;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
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
                followSpeed * Time.fixedDeltaTime
            );
        }
    }

    void AttackTimer()
    {
        if (petcd <= 0f) return;

        timer += Time.fixedDeltaTime;

        if (timer >= petcd)
        {
            timer = 0f;
            FireToNearestEnemy();
        }
    }

    void FireToNearestEnemy()
    {
        if (homingBulletPrefab == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
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

            HomingBullet homingBullet = bullet.GetComponent<HomingBullet>();
            if (homingBullet != null)
            {
                homingBullet.target = nearest.transform;
            }
        }
    }
}