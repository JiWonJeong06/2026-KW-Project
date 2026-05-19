using UnityEngine;

public class Pet : MonoBehaviour
{
    [Header("Follow")]
    public Transform player;
    public float followSpeed;
    public float followDistance;

    [Header("Attack")]
    public GameObject homingBulletPrefab;
    
    // ===== 펫 데이터 (PetData 필드) =====
    public float atk;                // 펫 공격력
    public float cooldown;           // 펫 공격 쿨타임
    public float hp;                 // 펫 체력
    public float additionalBullet;   // 펫 추가 총알
    public float bulletSpeed;        // 펫 총알 속도

    private float timer;

    void FixedUpdate()
    {
        FollowPlayer();
        AttackTimer();
    }

    // ===== PetData를 받는 메서드 =====
    public void ApplyData(PetData data)
    {
        atk = data.atk;
        cooldown = data.cooldown;
        hp = data.hp;
        additionalBullet = data.additionalBullet;
        bulletSpeed = data.bulletSpeed;
    }



    // ===== Player 설정 (PetSpawner에서 호출됨) =====
    public void SetPlayer(Transform target)
    {
        player = target;
    }

    // ===== Player를 따라다님 =====
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

    // ===== 공격 타이머 =====
    void AttackTimer()
    {
        if (cooldown <= 0f) return;

        timer += Time.fixedDeltaTime;

        if (timer >= cooldown)
        {
            timer = 0f;
            FireToNearestEnemy();
        }
    }

    // ===== 가장 가까운 적에게 발사 =====
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