using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private static SafeZone instance;
    
    [SerializeField] private Spawner spawner;
    [SerializeField] private RoomManager room_manager;
    [SerializeField] private GameObject barrier_wall; // 안전지대 벗어날 때 활성화할 벽
    [SerializeField] private GameObject safezone_visual; // 안전지대 시각적 표시 (옵션)

    private bool player_inside = true;
    private bool has_left = false;

    public static SafeZone Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 벽 비활성화
        if (barrier_wall != null)
        {
            barrier_wall.SetActive(false);
        }

        // 적 미리 스폰
        if (spawner != null)
        {
            spawner.SpawnEnemies();
            Debug.Log("안전지대 안에서 적 스폰 완료");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        
        if (player != null && !has_left)
        {
            OnPlayerLeaveSafeZone();
        }
    }

    private void OnPlayerLeaveSafeZone()
    {
        has_left = true;
        player_inside = false;

        Debug.Log("플레이어가 안전지대를 벗어났습니다. 전투 시작!");

        // 벽 활성화 (복귀 불가)
        if (barrier_wall != null)
        {
            barrier_wall.SetActive(true);
        }

        // 안전지대 시각적 표시 비활성화 (옵션)
        if (safezone_visual != null)
        {
            safezone_visual.SetActive(false);
        }

        // 방 시작 (문 닫기)
        if (room_manager != null)
        {
            room_manager.StartRoom();
        }

        // SafeZone 스크립트만 비활성화 (GameObject는 활성화 유지)
        enabled = false;
    }

    public bool IsPlayerInside()
    {
        return player_inside;
    }
}