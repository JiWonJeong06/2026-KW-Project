using UnityEngine;

/// <summary>
/// 게임 전체를 관리하는 클래스
/// 
/// 역할:
/// 1. 게임 상태 관리 (준비, 진행, 일시정지, 게임오버)
/// 2. 각 시스템 초기화
/// 3. 게임 흐름 제어
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,      // 게임 준비 중
        Playing,    // 게임 진행 중
        Paused,     // 일시정지
        GameOver    // 게임 오버
    }

    [SerializeField] private GameState currentState = GameState.Ready;
    
    private static GameManager instance;
    
    private Player player;
    private StageManager stageManager;
    private AbilityDataLoader abilityDataLoader;
    private AbilityManager abilityManager;
    private WeaponUpgradeSystem weaponUpgradeSystem;

    void Awake()
    {
        // 싱글톤 패턴
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // DontDestroyOnLoad(gameObject);  // 장면 전환 시에도 유지하려면 활성화
    }

    void Start()
    {
        // 모든 시스템 초기화
        InitializeSystems();

        // 게임 시작
        StartGame();
    }

    void Update()
    {
        // ESC 키로 일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
                PauseGame();
            else if (currentState == GameState.Paused)
                ResumeGame();
        }

        // 디버그: 스테이지 강제 클리어 (F1)
        if (Input.GetKeyDown(KeyCode.F1) && stageManager != null)
        {
            stageManager.DebugClearStage();
        }

        // 디버그: 게임 오버 (F2)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameOver();
        }
    }

    /// <summary>
    /// 모든 시스템 초기화
    /// </summary>
    private void InitializeSystems()
    {
        Debug.Log("[GameManager] 시스템 초기화 중...");

        // 핵심 시스템 찾기
        player = FindFirstObjectByType<Player>();
        stageManager = FindFirstObjectByType<StageManager>();
        abilityDataLoader = FindFirstObjectByType<AbilityDataLoader>();
        abilityManager = FindFirstObjectByType<AbilityManager>();
        weaponUpgradeSystem = FindFirstObjectByType<WeaponUpgradeSystem>();

        // 에러 체크
        if (player == null)
            Debug.LogError("[GameManager] Player를 찾을 수 없음");
        if (stageManager == null)
            Debug.LogError("[GameManager] StageManager를 찾을 수 없음");
        if (abilityDataLoader == null)
            Debug.LogError("[GameManager] AbilityDataLoader를 찾을 수 없음");
        if (abilityManager == null)
            Debug.LogError("[GameManager] AbilityManager를 찾을 수 없음");
        if (weaponUpgradeSystem == null)
            Debug.LogError("[GameManager] WeaponUpgradeSystem을 찾을 수 없음");

        Debug.Log("[GameManager] 시스템 초기화 완료");
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1.0f;  // 게임 실행

        Debug.Log("[GameManager] 게임 시작!");
    }

    /// <summary>
    /// 게임 일시정지
    /// </summary>
    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0.0f;  // 게임 일시정지

        Debug.Log("[GameManager] 게임 일시정지");

        // UI 표시
        // UIManager.ShowPauseUI();
    }

    /// <summary>
    /// 게임 재개
    /// </summary>
    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1.0f;  // 게임 재개

        Debug.Log("[GameManager] 게임 재개");

        // UI 숨기기
        // UIManager.HidePauseUI();
    }

    /// <summary>
    /// 게임 오버
    /// </summary>
    public void GameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0.0f;

        Debug.Log("[GameManager] 게임 오버!");

        // UI 표시
        // UIManager.ShowGameOverUI();
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[GameManager] 게임 재시작");

        // 씬 재로드
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// 현재 게임 상태 반환
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 게임이 진행 중인지 확인
    /// </summary>
    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }

    /// <summary>
    /// 싱글톤 인스턴스 반환
    /// </summary>
    public static GameManager Instance
    {
        get { return instance; }
    }
}