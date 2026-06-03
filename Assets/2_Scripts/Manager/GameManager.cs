using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("맵 프리팹")]
    [SerializeField] private GameObject map_prefab;
    [SerializeField] private GameObject boss_map_prefab;

    [Header("플레이어")]
    [SerializeField] private GameObject player_prefab;

    [Header("페이드 효과")]
    [SerializeField] private FadeController fade_controller;

    private int    current_stage      = 1;
    private string current_difficulty = "Easy";
    private GameObject current_map      = null;
    private GameObject player_instance  = null;

    private bool is_game_started = false; // 인게임 씬 진입 여부

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 전환 감지 — 2_InGame 씬 진입 시 자동 초기화
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // LoadingSceneUI에서 InitGame()을 직접 호출하므로 여기선 처리 안 함
        // (Additive 로드 방식이라 sceneLoaded 이벤트로 처리하지 않음)
    }

    private void Start()
    {
        // 씬 전환은 OnSceneLoaded에서 처리
        // Title 씬에서는 아무것도 하지 않음
    }

    /// <summary>
    /// InGame 씬 진입 시 초기화 — GameManager.Start() 또는 씬 전환 후 호출
    /// </summary>
    public void InitGame()
    {
        VolumeSettingsUI.LoadSavedVolume();

        current_stage      = 1;
        current_difficulty = "Easy";

        CreatePlayer();
        LoadStage(1, "Easy");

        // 스테이지 BGM
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM_Stage();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.f1Key.wasPressedThisFrame)
            DebugKillAllEnemies();
#endif
    }

#if UNITY_EDITOR
    private void DebugKillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) { Debug.Log("[DEBUG] Enemy 없음"); return; }
        foreach (var e in enemies) Destroy(e);
        Debug.Log($"[DEBUG] F1 — {enemies.Length}개 제거");
    }
#endif

    // ─────────────────────────────────────────
    // 플레이어 생성
    // ─────────────────────────────────────────
    private void CreatePlayer()
    {
        if (player_prefab == null)
        {
            Debug.LogError("[GameManager] Player Prefab 미할당!");
            return;
        }

        // 이미 있으면 위치만 초기화
        if (player_instance != null)
        {
            player_instance.transform.position = new Vector3(0f, -7f, 0f);
            return;
        }

        player_instance = Instantiate(player_prefab, new Vector3(0f, -7f, 0f), Quaternion.identity);
        DontDestroyOnLoad(player_instance);
        Debug.Log("[GameManager] 플레이어 생성 완료");
    }

    // ─────────────────────────────────────────
    // 스테이지 로드
    // ─────────────────────────────────────────
    public void LoadNextStage(string selected_difficulty)
    {
        current_stage++;

        if (current_stage > 8)
        {
            Debug.Log("[GameManager] 게임 클리어!");
            return;
        }

        current_difficulty = selected_difficulty;
        LoadStage(current_stage, current_difficulty);
    }

    private void LoadStage(int stage, string difficulty)
    {
        StartCoroutine(LoadStageCoroutine(stage, difficulty));
    }

    private IEnumerator LoadStageCoroutine(int stage, string difficulty)
    {
        Debug.Log($"[GameManager] Stage {stage} 로드 (난이도: {difficulty})");

        if (fade_controller != null)
            yield return fade_controller.FadeOut();

        if (current_map != null)
        {
            Destroy(current_map);
            current_map = null;
        }

        if (stage <= 7)
            CreateNormalMap(difficulty);
        else if (stage == 8)
            CreateBossMap();

        MovePlayerToStartPosition();

        if (fade_controller != null)
            yield return fade_controller.FadeIn();

        Debug.Log($"[GameManager] Stage {stage} 로드 완료");
    }

    private void CreateNormalMap(string difficulty)
    {
        if (map_prefab == null) { Debug.LogError("[GameManager] map_prefab 미할당!"); return; }

        current_map = Instantiate(map_prefab, Vector3.zero, Quaternion.identity);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM_Stage();

        MapManager map_manager = current_map.GetComponentInChildren<MapManager>();
        if (map_manager != null)
            map_manager.SetDifficulty(difficulty);
        else
            Debug.LogError("[GameManager] MapManager를 찾을 수 없습니다!");
    }

    private void CreateBossMap()
    {
        if (boss_map_prefab == null) { Debug.LogError("[GameManager] boss_map_prefab 미할당!"); return; }

        current_map = Instantiate(boss_map_prefab, Vector3.zero, Quaternion.identity);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM_Boss();

        Debug.Log("[GameManager] 보스 맵 생성 완료");
    }

    private void MovePlayerToStartPosition()
    {
        if (player_instance == null) return;
        player_instance.transform.position = new Vector3(0f, -7f, 0f);
    }

    public int    GetCurrentStage()      => current_stage;
    public string GetCurrentDifficulty() => current_difficulty;
}