using UnityEngine;

public class OpenedDoor : MonoBehaviour
{
    [Header("Door Info")]
    public DoorColor doorColor;
    public DoorTier doorTier;

    private RoomManager roomManager;
    private CardSpawner cardSpawner;

    public void Init(RoomManager manager, DoorColor color, DoorTier tier)
    {
        roomManager = manager;
        doorColor = color;
        doorTier = tier;

        cardSpawner = FindFirstObjectByType<CardSpawner>();
    }

    public void Interact()
    {
        Debug.Log($"{doorColor} 열린 문과 상호작용");

        if (cardSpawner != null)
        {
            cardSpawner.SpawnRandomCards(3);
        }
        else
        {
            Debug.LogWarning("CardSpawner를 찾지 못함");
        }

 
}
}