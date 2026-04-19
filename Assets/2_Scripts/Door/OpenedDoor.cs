using UnityEngine;

public class OpenedDoor : MonoBehaviour
{
    [Header("Door Info")]
    public DoorColor doorColor;
    public DoorTier doorTier;

    private RoomManager roomManager;
    public CardSpawner cardSpawner;

    public void Start()
    {
               cardSpawner = FindFirstObjectByType<CardSpawner>();
    }
    public void Init(RoomManager manager, DoorColor color, DoorTier tier)
    {
        roomManager = manager;
        doorColor = color;
        doorTier = tier;

        
    }

    public void Interact()
    {
        Debug.Log($"{doorColor} 열린 문과 상호작용");

        if (cardSpawner == null)
        {
            Debug.LogWarning("CardSpawner를 찾지 못함");
            return;
        }

        // 문 색상에 맞는 카드 3장 소환
        cardSpawner.SpawnDummyCards(doorColor, 3);

        // 카드 테스트 중에는 잠시 막아두는 것이 좋음
        // StageManager.Instance.LoadNextRoom();
    }
    public void LoadNextRoom()
    {
        StageManager.Instance.LoadNextRoom();
    }
}