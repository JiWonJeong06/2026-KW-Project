using UnityEngine;

public class PetSpawner : MonoBehaviour
{
    public GameObject petPrefab;
    public Transform spawnPoint;

    void Start()
    {
        SpawnPet();
    }

    void SpawnPet()
    {
        if (petPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Pet prefab or spawn point is not assigned.");
            return;
        }

        Player player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없음");
            return;
        }

        GameObject petObj = Instantiate(
            petPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );

        Pet pet = petObj.GetComponent<Pet>();

        if (pet != null)
        {
            pet.SetPlayer(player.transform);
        }
        else
        {
            Debug.LogError("생성한 펫 프리팹에 Pet 스크립트가 없음");
        }
    }
}