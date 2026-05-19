using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Vector2 center = Vector2.zero;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float tileSize;

    [Header("UI")]
    //[SerializeField] private TMP_Text enemyText;

    [Header("Doors")]
    [SerializeField] private DoorController redDoor;
    [SerializeField] private DoorController blueDoor;
    [SerializeField] private DoorController yellowDoor;

    private bool isCleared = false;

    void Start()
    {
        SetupDoors();
        CloseAllDoors();
    }

    void Update()
    {
        if (isCleared) return;

        int enemyCount = CountEnemiesInRoom();

        // enemyText.text = "Enemy: " + enemyCount.ToString("D1");

        if (enemyCount == 0)
        {
            isCleared = true;
            Debug.Log("방 클리어! 모든 적 처치됨.");
            OpenAllDoors();
            
            // ===== NEW: StageManager에 알림 =====
            StageManager stageManager = FindFirstObjectByType<StageManager>();
            if (stageManager != null)
            {
                stageManager.OnStageClear();
            }
        }
    }

    void SetupDoors()
    {
        // ===== 수정: Init() 호출 - RoomManager 참조 제거, enum을 string/int로 변환 =====
        
        // DoorColor enum을 string으로 변환
        redDoor.Init("Magenta", (int)GetRandomTier());
        blueDoor.Init("Cyan", (int)GetRandomTier());
        yellowDoor.Init("Yellow", (int)GetRandomTier());
        
        Debug.Log("[RoomManager] 3개 문 설정 완료");
    }

    DoorTier GetRandomTier()
    {
        int randomValue = Random.Range(1, 4);
        return (DoorTier)randomValue;
    }

    void CloseAllDoors()
    {
        redDoor.SetOpen(false);
        blueDoor.SetOpen(false);
        yellowDoor.SetOpen(false);
        
        Debug.Log("[RoomManager] 모든 문 닫음");
    }

    void OpenAllDoors()
    {
        redDoor.SetOpen(true);
        blueDoor.SetOpen(true);
        yellowDoor.SetOpen(true);
        
        Debug.Log("[RoomManager] 모든 문 열음");
    }

    public void SelectDoor(DoorController selectedDoor)
    {
        // 선택한 문 정보 출력 (선택사항)
        Debug.Log($"[RoomManager] 선택한 문 열기");

        // 나중에 카드 UI 표시
        // CardUISystem.DisplayCards(...);
    }

    int CountEnemiesInRoom()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int count = 0;

        float halfWidth = (width * tileSize) / 2f;
        float halfHeight = (height * tileSize) / 2f;

        foreach (GameObject enemy in enemies)
        {
            Vector2 pos = enemy.transform.position;

            bool isInside =
                pos.x >= center.x - halfWidth &&
                pos.x <= center.x + halfWidth &&
                pos.y >= center.y - halfHeight &&
                pos.y <= center.y + halfHeight;

            if (isInside)
                count++;
        }

        return count;
    }
}

// ===== 기존 코드용 Enum 정의 (필요시) =====
