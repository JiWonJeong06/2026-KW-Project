using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private static SafeZone instance;

    [SerializeField] private Spawner spawner;
    [SerializeField] private RoomManager room_manager;
    [SerializeField] private GameObject barrier_wall;
    [SerializeField] private GameObject safezone_visual;

    private bool player_inside = false; // 실제로 안에 있는지 콜라이더로 판단
    private bool has_left = false;

    public static SafeZone Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (barrier_wall != null)
            barrier_wall.SetActive(false);

        if (spawner != null)
        {
            // 8라운드(보스방)는 SafeZone에서 적 소환 안 함
            // 보스는 RoomManager.StartRoom()에서 SpawnBoss()로 소환
            if (RoomManager.CurrentRound != 8)
            {
                spawner.SpawnEnemies();
                Debug.Log("안전지대 안에서 적 스폰 완료");
            }
            else
            {
                Debug.Log("[SafeZone] 8라운드 — 잡몹 소환 생략 (보스방)");
            }
        }

        // Start에서 콜라이더 안에 플레이어가 있는지 즉시 체크
        CheckPlayerOverlap();
    }

    /// <summary>
    /// Start 시점에 이미 콜라이더 안에 있는 플레이어를 감지
    /// </summary>
    private void CheckPlayerOverlap()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        // SafeZone 콜라이더 범위 안의 모든 콜라이더 검사
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.bounds.size,
            0f
        );

        foreach (var hit in hits)
        {
            if (hit.GetComponent<Player>() != null)
            {
                player_inside = true;
                Debug.Log("[SafeZone] Start 시점 플레이어 감지 — 안전지대 안에 있음");
                return;
            }
        }

        // 플레이어가 콜라이더 밖에서 시작 (보스 씬 등)
        player_inside = false;
        Debug.Log("[SafeZone] Start 시점 플레이어 감지 — 안전지대 밖에서 시작");
    }

    // 플레이어가 콜라이더 안에 있는 동안 계속 호출
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            player_inside = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            player_inside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            player_inside = false;

            if (!has_left)
            {
                OnPlayerLeaveSafeZone();
            }
        }
    }

    private void OnPlayerLeaveSafeZone()
    {
        has_left = true;

        Debug.Log("플레이어가 안전지대를 벗어났습니다. 전투 시작!");

        if (barrier_wall != null)
            barrier_wall.SetActive(true);

        if (safezone_visual != null)
            safezone_visual.SetActive(false);

        if (room_manager != null)
            room_manager.StartRoom();

        enabled = false;
    }

    public bool IsPlayerInside() => player_inside;
}