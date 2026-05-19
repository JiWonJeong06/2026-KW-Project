using UnityEngine;

/// <summary>
/// 스테이지를 관리하는 클래스
/// 
/// 역할:
/// 1. 현재 스테이지 번호 관리
/// 2. 스테이지 클리어 감지
/// 3. DoorSystem과 연동
/// 4. 다음 스테이지로 진행
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 1;
    
    private DoorSystem doorSystem;
    private AbilityDataLoader abilityDataLoader;
    private Player player;
    
    private bool isStageCleared = false;

    // ===== 싱글톤 패턴 =====
    private static StageManager instance;
    
    public static StageManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // 싱글톤 설정
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        doorSystem = FindFirstObjectByType<DoorSystem>();
        abilityDataLoader = FindFirstObjectByType<AbilityDataLoader>();
        player = FindFirstObjectByType<Player>();

        if (doorSystem == null)
            Debug.LogError("[StageManager] DoorSystem을 찾을 수 없음");
        if (abilityDataLoader == null)
            Debug.LogError("[StageManager] AbilityDataLoader를 찾을 수 없음");
        if (player == null)
            Debug.LogError("[StageManager] Player를 찾을 수 없음");

        Debug.Log($"[StageManager] 스테이지 {currentStage} 시작");
    }

    /// <summary>
    /// 스테이지 클리어 감지
    /// </summary>
    public void OnStageClear()
    {
        if (isStageCleared)
            return;

        isStageCleared = true;

        Debug.Log($"[StageManager] 스테이지 {currentStage} 클리어!");

        Invoke(nameof(ShowDoors), 1.0f);
    }

    /// <summary>
    /// 문 표시
    /// </summary>
    private void ShowDoors()
    {
        if (doorSystem == null)
            return;

        doorSystem.SpawnDoors();

        Debug.Log("[StageManager] 3개 문 생성");
    }

    /// <summary>
    /// 카드 선택 완료 후 호출
    /// </summary>
    public void OnAbilitySelected()
    {
        Debug.Log($"[StageManager] 증강 선택 완료 → 다음 스테이지로 진행");

        Invoke(nameof(NextStage), 1.0f);
    }

    /// <summary>
    /// 다음 스테이지로 진행
    /// </summary>
    private void NextStage()
    {
        currentStage++;
        isStageCleared = false;

        Debug.Log($"[StageManager] 스테이지 {currentStage} 시작");

        ResetStage();
    }

    /// <summary>
    /// 다음 방으로 로드 (기존 코드 호환)
    /// </summary>
    public void LoadNextRoom()
    {
        NextStage();
    }

    /// <summary>
    /// 스테이지 초기화
    /// </summary>
    private void ResetStage()
    {
        if (doorSystem != null)
            doorSystem.CloseDoors();

        Debug.Log($"[StageManager] 스테이지 {currentStage} 준비 완료");
    }

    /// <summary>
    /// 현재 스테이지 반환
    /// </summary>
    public int GetCurrentStage()
    {
        return currentStage;
    }

    /// <summary>
    /// 스테이지 클리어 여부
    /// </summary>
    public bool IsStageCleared()
    {
        return isStageCleared;
    }

    /// <summary>
    /// 스테이지 강제 클리어 (디버그)
    /// </summary>
    public void DebugClearStage()
    {
        OnStageClear();
        Debug.Log($"[StageManager] (디버그) 스테이지 {currentStage} 강제 클리어");
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    public void GameOver()
    {
        Debug.Log("[StageManager] 게임 오버!");
        
        Time.timeScale = 0;
    }
}