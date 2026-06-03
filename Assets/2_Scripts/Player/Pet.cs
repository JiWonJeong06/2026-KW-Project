using UnityEngine;
using System.Collections;

public class Pet : MonoBehaviour
{
    [Header("Pet Prefabs")]
    [SerializeField] private GameObject pet_bullet_prefab;

    [Header("Follow Settings")]
    [SerializeField] private Vector2 follow_offset = new Vector2(-1f, -0.5f); // 플레이어 기준 오프셋
    [SerializeField] private float   follow_speed  = 5f;                      // 따라오는 속도

    // ─────────────────────────────────────────
    private PetData   pet_data;
    private Player    player;

    private float  cooldown_timer  = 0f;
    private bool   enemy_detected  = false;

    // hp 효과: 방 클리어 카운트
    private int   room_clear_count = 0;

    // ─────────────────────────────────────────
    private void Start()
    {
        player   = FindAnyObjectByType<Player>();
        pet_data = PetDataLoader.Instance?.GetPetData();

        if (pet_data == null)
        {
            Debug.LogError("[Pet] PetData 로드 실패!");
            return;
        }

        // 쿨타임은 즉시 준비 상태로 시작
        cooldown_timer = pet_data.cooldown;

        Debug.Log($"[Pet] 등장! {pet_data.korean_name} / 공격력={pet_data.atk} / 쿨다운={pet_data.cooldown}초");
    }

    private void Update()
    {
        if (player == null || pet_data == null) return;

        FollowPlayer();
        DetectEnemy();
        UpdateCooldown();
    }

    // ─────────────────────────────────────────
    // 플레이어 오프셋 위치로 부드럽게 이동
    // ─────────────────────────────────────────
    private void FollowPlayer()
    {
        Vector2 target_pos = (Vector2)player.transform.position + follow_offset;
        transform.position = Vector2.Lerp(transform.position,
                                          target_pos,
                                          follow_speed * Time.deltaTime);
    }

    // ─────────────────────────────────────────
    // 씬에 살아있는 적이 있는지 감지
    // ─────────────────────────────────────────
    private void DetectEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        enemy_detected = false;

        foreach (var e in enemies)
        {
            if (e.IsAlive())
            {
                enemy_detected = true;
                break;
            }
        }
    }

    // ─────────────────────────────────────────
    // 쿨타임 관리 및 발사 조건 체크
    // ─────────────────────────────────────────
    private void UpdateCooldown()
    {
        // SafeZone 안이거나 적이 없으면 타이머 정지 (쿨타임 소모 안 함)
        if (IsInSafeZone() || !enemy_detected) return;

        cooldown_timer += Time.deltaTime;

        if (cooldown_timer >= pet_data.cooldown)
        {
            cooldown_timer = 0f;
            Shoot();
        }
    }

    // ─────────────────────────────────────────
    // 발사
    // ─────────────────────────────────────────
    private void Shoot()
    {
        if (pet_bullet_prefab == null)
        {
            Debug.LogError("[Pet] pet_bullet_prefab 미할당!");
            return;
        }

        // 메인 탄 발사
        FireBullet();

        // additional_bullet: 0~100 확률로 추가 1발
        if (pet_data.additional_bullet > 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < pet_data.additional_bullet)
            {
                FireBullet();
                Debug.Log($"[Pet] 추가 탄 발사! (확률 {pet_data.additional_bullet}%)");
            }
        }
    }

    private void FireBullet()
    {
        GameObject obj = Instantiate(pet_bullet_prefab,
                                     transform.position,
                                     Quaternion.identity);
        PetBullet pb = obj.GetComponent<PetBullet>();
        if (pb != null)
            pb.Initialize(pet_data.bullet_speed, pet_data.atk);
    }

    // ─────────────────────────────────────────
    // 방 클리어 시 외부에서 호출
    // ─────────────────────────────────────────
    public void OnRoomCleared()
    {
        room_clear_count++;
        Debug.Log($"[Pet] 방 클리어 카운트: {room_clear_count}");

        // 5라운드마다 플레이어 체력 +1
        if (room_clear_count % 5 == 0)
        {
            if (player != null)
            {
                player.Heal(pet_data.hp);
                Debug.Log($"[Pet] 5라운드 달성! 플레이어 체력 +{pet_data.hp}");
            }
        }
    }

    // ─────────────────────────────────────────
    // SafeZone 체크
    // ─────────────────────────────────────────
    private bool IsInSafeZone()
    {
        return SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside();
    }
}