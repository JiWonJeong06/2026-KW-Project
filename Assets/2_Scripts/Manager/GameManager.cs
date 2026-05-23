using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("맵 프리팹")]
    [SerializeField] private GameObject map_prefab;       // Stage 1~7용
    [SerializeField] private GameObject boss_map_prefab;  // Stage 8(보스전)용

    [Header("플레이어")]
    [SerializeField] private GameObject player_prefab;    // 플레이어 프리팹

    [Header("페이드 효과")]
    [SerializeField] private FadeController fade_controller;

    private int current_stage = 1;           // 현재 스테이지 (1~8)
    private string current_difficulty = "Easy"; // 현재 난이도
    private GameObject current_map = null;   // 현재 생성된 맵
    private GameObject player_instance = null; // 생성된 플레이어 인스턴스

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 플레이어 생성
        CreatePlayer();
        
        // Stage 1 시작 (Easy 고정)
        LoadStage(1, "Easy");
    }

    private void CreatePlayer()
    {
        if (player_prefab == null)
        {
            Debug.LogError("[GameManager] Player Prefab이 할당되지 않았습니다!");
            return;
        }

        // 플레이어 생성 (0, -7, 0)
        player_instance = Instantiate(player_prefab, new Vector3(0f, -7f, 0f), Quaternion.identity);
        DontDestroyOnLoad(player_instance); // 씬 전환 시에도 유지
        Debug.Log("[GameManager] 플레이어 생성 완료");
    }

    // 다음 스테이지 로드 (증강 선택 후 호출)
    public void LoadNextStage(string selected_difficulty)
    {
        current_stage++;

        if (current_stage > 8)
        {
            Debug.Log("[GameManager] 게임 클리어!");
            // TODO: 승리 화면
            return;
        }

        current_difficulty = selected_difficulty;
        LoadStage(current_stage, current_difficulty);
    }

    // 스테이지 로드
    private void LoadStage(int stage, string difficulty)
    {
        StartCoroutine(LoadStageCoroutine(stage, difficulty));
    }

    private IEnumerator LoadStageCoroutine(int stage, string difficulty)
    {
        Debug.Log($"[GameManager] Stage {stage} 로드 시작 (난이도: {difficulty})");

        // 1. 페이드 아웃
        if (fade_controller != null)
        {
            yield return fade_controller.FadeOut();
        }

        // 2. 기존 맵 파괴
        if (current_map != null)
        {
            Destroy(current_map);
            current_map = null;
        }

        // 3. 새 맵 생성
        if (stage <= 7)
        {
            // Stage 1~7: Map_prefab 사용
            CreateNormalMap(difficulty);
        }
        else if (stage == 8)
        {
            // Stage 8: Boss_Map_prefab 사용
            CreateBossMap();
        }

        // 4. 플레이어를 (0, -7, 0)으로 이동
        MovePlayerToStartPosition();

        // 5. 페이드 인
        if (fade_controller != null)
        {
            yield return fade_controller.FadeIn();
        }

        Debug.Log($"[GameManager] Stage {stage} 로드 완료");
    }

    private void CreateNormalMap(string difficulty)
    {
        if (map_prefab == null)
        {
            Debug.LogError("[GameManager] Map_prefab이 할당되지 않았습니다!");
            return;
        }

        // 맵 생성
        current_map = Instantiate(map_prefab, Vector3.zero, Quaternion.identity);

        // MapManager 찾기 (자식 오브젝트 포함)
        MapManager map_manager = current_map.GetComponentInChildren<MapManager>();
        if (map_manager != null)
        {
            map_manager.SetDifficulty(difficulty);
            Debug.Log("[GameManager] MapManager 찾음 - 난이도 설정 완료");
        }
        else
        {
            Debug.LogError("[GameManager] MapManager를 찾을 수 없습니다! Map_prefab에 MapManager 스크립트가 있는지 확인하세요.");
        }
    }

    private void CreateBossMap()
    {
        if (boss_map_prefab == null)
        {
            Debug.LogError("[GameManager] Boss_Map_prefab이 할당되지 않았습니다!");
            return;
        }

        // 보스 맵 생성
        current_map = Instantiate(boss_map_prefab, Vector3.zero, Quaternion.identity);
        Debug.Log("[GameManager] 보스 맵 생성 완료");
    }

    private void MovePlayerToStartPosition()
    {
        if (player_instance == null)
        {
            Debug.LogWarning("[GameManager] Player가 생성되지 않았습니다!");
            return;
        }

        // 플레이어를 (0, -7, 0)으로 이동
        player_instance.transform.position = new Vector3(0f, -7f, 0f);
        Debug.Log("[GameManager] 플레이어를 (0, -7, 0)으로 이동");
    }

    // Getter
    public int GetCurrentStage() => current_stage;
    public string GetCurrentDifficulty() => current_difficulty;
}