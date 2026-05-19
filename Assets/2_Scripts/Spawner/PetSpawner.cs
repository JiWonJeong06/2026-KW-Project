using UnityEngine;

/// <summary>
/// 펫을 생성하고 Player와 연결하는 클래스
/// 역할:
/// - Pet 프리팹 인스턴시화
/// - Player 찾기
/// - pet.SetPlayer(player) 호출 ← 핵심!
/// </summary>
public class PetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject petPrefab;
    [SerializeField] private Transform spawnPoint;

    void Start()
    {
        SpawnPet();
    }

    void SpawnPet()
    {
        // 1. 검증
        if (petPrefab == null)
        {
            Debug.LogError("Pet prefab이 할당되지 않음");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point가 할당되지 않음");
            return;
        }

        // 2. Player 찾기
        Player player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없음. Player가 생성되었는지 확인하세요.");
            return;
        }

        // 3. Pet 생성
        GameObject petObj = Instantiate(
            petPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );

        if (petObj == null)
        {
            Debug.LogError("Pet 인스턴시화 실패");
            return;
        }

        // 4. Pet 컴포넌트 가져오기
        Pet pet = petObj.GetComponent<Pet>();
        if (pet == null)
        {
            Debug.LogError("Pet 스크립트를 찾을 수 없음");
            Destroy(petObj);
            return;
        }

        // 5. *** 핵심: Pet에게 Player 전달 ***
        pet.SetPlayer(player.transform);
        Debug.Log("Pet이 Player를 따라다니도록 설정됨");
    }
}