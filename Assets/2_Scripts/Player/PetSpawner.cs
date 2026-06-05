using UnityEngine;

/// <summary>
/// 씬 시작 시 펫을 소환하는 스포너
/// </summary>
public class PetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pet_prefab;  // 펫 프리팹

    private void Awake()
    {
        SpawnPet();
    }

    private void SpawnPet()
    {
        if (pet_prefab == null)
        {
            Debug.LogError("[PetSpawner] pet_prefab 미할당!");
            return;
        }

        // 플레이어 위치 찾기
        Player player = FindAnyObjectByType<Player>();
        Vector3 spawn_pos = player != null
            ? player.transform.position
            : Vector3.zero;

        // 펫 소환
        GameObject pet_obj = Instantiate(pet_prefab, spawn_pos, Quaternion.identity);

        Debug.Log($"[PetSpawner] 펫 소환 완료: {pet_obj.name}");
    }
}