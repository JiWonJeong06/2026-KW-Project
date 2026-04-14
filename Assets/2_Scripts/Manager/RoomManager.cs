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
    [SerializeField] private TMP_Text enemyText;

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

        enemyText.text = "Enemy: " + enemyCount.ToString("D1");

        if (enemyCount == 0)
        {
            isCleared = true;
            enemyText.text = "Clear!";
            OpenAllDoors();
        }
    }

    void SetupDoors()
    {
        redDoor.Init(this, DoorColor.Red, GetRandomTier());
        blueDoor.Init(this, DoorColor.Blue, GetRandomTier());
        yellowDoor.Init(this, DoorColor.Yellow, GetRandomTier());

        Debug.Log($"Red Door Tier: {redDoor.doorTier}");
        Debug.Log($"Blue Door Tier: {blueDoor.doorTier}");
        Debug.Log($"Yellow Door Tier: {yellowDoor.doorTier}");
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
    }

    void OpenAllDoors()
    {
        redDoor.SetOpen(true);
        blueDoor.SetOpen(true);
        yellowDoor.SetOpen(true);
    }

    public void SelectDoor(DoorController selectedDoor)
    {
        Debug.Log($"선택한 문 색: {selectedDoor.doorColor}");
        Debug.Log($"선택한 문 단계: {selectedDoor.doorTier}");

        // 여기서 나중에 업그레이드 UI 열기
        // UpgradeManager.Instance.ShowUpgradeChoices(selectedDoor.doorColor, selectedDoor.doorTier);

        // 지금은 테스트용
        Debug.Log("업그레이드 선택창 열기");
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