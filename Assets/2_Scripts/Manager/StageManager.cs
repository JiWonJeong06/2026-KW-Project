using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Room Settings")]
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private Transform roomParent;

    private GameObject currentRoom;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FirstRoomSetup();
    }

    void FirstRoomSetup()
    {
        if (roomPrefabs == null || roomPrefabs.Length == 0)
        {
            Debug.LogWarning("Room Prefabs가 비어 있음");
            return;
        }

        int randomIndex = Random.Range(0, roomPrefabs.Length);
        GameObject firstRoom = Instantiate(
            roomPrefabs[randomIndex],
            Vector3.zero,
            Quaternion.identity,
            roomParent
        );

        currentRoom = firstRoom;


    }

    public void SetCurrentRoom(GameObject room)
    {
        currentRoom = room;
    }

    public void LoadNextRoom()
    {
        if (roomPrefabs == null || roomPrefabs.Length == 0)
        {
            Debug.LogWarning("Room Prefabs가 비어 있음");
            return;
        }

        Vector3 spawnPosition = Vector3.zero;

        if (currentRoom != null)
        {
            spawnPosition = currentRoom.transform.position;
            Destroy(currentRoom);
        }

        int randomIndex = Random.Range(0, roomPrefabs.Length);
        GameObject nextRoom = Instantiate(
            roomPrefabs[randomIndex],
            spawnPosition,
            Quaternion.identity,
            roomParent
        );

        currentRoom = nextRoom;

        Debug.Log("다음 방 생성 완료");
    }
}