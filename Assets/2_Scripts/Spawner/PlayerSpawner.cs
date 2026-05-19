using UnityEngine;

/// <summary>
/// 플레이어를 생성하고 카메라를 설정하는 클래스
/// 역할:
/// - Player 프리팹 인스턴시화
/// - Camera 설정 (CameraFollow)
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
    [SerializeField] private Transform parentTransform;

    void Awake()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // 1. 검증
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab이 할당되지 않음");
            return;
        }

        // 2. Player 생성
        GameObject playerObj = Instantiate(
            playerPrefab,
            spawnPosition,
            Quaternion.identity,
            parentTransform
        );

        if (playerObj == null)
        {
            Debug.LogError("Player 인스턴시화 실패");
            return;
        }

        // 3. Player 컴포넌트 가져오기
        Player player = playerObj.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player 스크립트를 찾을 수 없음");
            Destroy(playerObj);
            return;
        }

        // 4. 카메라 설정
        SetupCamera(playerObj.transform);

        Debug.Log($"Player 생성 완료: {playerObj.name}");
    }

    void SetupCamera(Transform playerTransform)
    {
        // 메인 카메라 찾기
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera를 찾을 수 없음");
            return;
        }

        // CameraFollow 컴포넌트 찾기
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            // Player를 카메라의 타겟으로 설정
            cameraFollow.SetTarget(playerTransform);
            Debug.Log("Camera가 Player를 따라다니도록 설정됨");
        }
        else
        {
            Debug.LogWarning("CameraFollow 스크립트를 찾을 수 없음");
        }
    }
}