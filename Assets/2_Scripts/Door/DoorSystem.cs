using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 스테이지 클리어 후 3개의 문을 생성하는 시스템
/// 
/// 역할:
/// 1. 3개 문 생성 (Magenta, Cyan, Yellow)
/// 2. 각 문의 티어 랜덤 (1~3별)
/// 3. 각 문의 컨트롤러에 정보 전달
/// </summary>
public class DoorSystem : MonoBehaviour
{
    [SerializeField] private GameObject doorPrefab;  // 문 프리팹
    [SerializeField] private Transform[] doorSpawnPositions;  // 3개의 문 위치
    
    private DoorController[] doors = new DoorController[3];
    private string[] doorColors = { "Magenta", "Cyan", "Yellow" };  // 고정
    
    /// <summary>
    /// 스테이지 클리어 후 호출
    /// 3개 문을 생성하고 랜덤 티어 할당
    /// </summary>
    public void SpawnDoors()
    {
        // 1. 기존 문 제거
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] != null)
            {
                Destroy(doors[i].gameObject);
            }
        }

        // 2. 3개 문 생성
        for (int i = 0; i < 3; i++)
        {
            // 위치에 문 생성
            GameObject doorObj = Instantiate(doorPrefab, doorSpawnPositions[i]);
            DoorController doorController = doorObj.GetComponent<DoorController>();
            
            if (doorController == null)
            {
                Debug.LogError("[DoorSystem] 문에 DoorController가 없습니다!");
                continue;
            }

            // 색깔 할당 (고정)
            string color = doorColors[i];
            
            // 티어 할당 (랜덤: 1~3)
            int tier = Random.Range(1, 4);  // 1, 2, 3
            
            // 문 초기화
            doorController.Initialize(color, tier);
            
            doors[i] = doorController;
            
            Debug.Log($"[DoorSystem] 문 {i} 생성: {color} {tier}별");
        }

        Debug.Log("[DoorSystem] 3개 문 생성 완료");
    }

    /// <summary>
    /// 모든 문 닫기 (다음 스테이지로 넘어갈 때)
    /// </summary>
    public void CloseDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] != null)
            {
                Destroy(doors[i].gameObject);
            }
        }

        Debug.Log("[DoorSystem] 모든 문 닫음");
    }

    /// <summary>
    /// 특정 색깔의 문 정보 반환
    /// </summary>
    public DoorController GetDoor(string color)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] != null && doors[i].Color == color)
            {
                return doors[i];
            }
        }

        return null;
    }
}