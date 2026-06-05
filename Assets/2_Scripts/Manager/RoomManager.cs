using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    private static RoomManager instance;
    public static RoomManager Instance => instance;

    [Header("스폰 관리")]
    [SerializeField] private Spawner spawner;

    [Header("닫힌 문 (각 1개)")]
    [SerializeField] private GameObject door_cyan_closed;
    [SerializeField] private GameObject door_magenta_closed;
    [SerializeField] private GameObject door_yellow_closed;

    [Header("열린 문 - Cyan (등급별 3개)")]
    [SerializeField] private GameObject door_cyan_opened_c;
    [SerializeField] private GameObject door_cyan_opened_b;
    [SerializeField] private GameObject door_cyan_opened_a;

    [Header("열린 문 - Magenta (등급별 3개)")]
    [SerializeField] private GameObject door_magenta_opened_c;
    [SerializeField] private GameObject door_magenta_opened_b;
    [SerializeField] private GameObject door_magenta_opened_a;

    [Header("열린 문 - Yellow (등급별 3개)")]
    [SerializeField] private GameObject door_yellow_opened_c;
    [SerializeField] private GameObject door_yellow_opened_b;
    [SerializeField] private GameObject door_yellow_opened_a;

    [Header("Door 스크립트 (등급별 9개)")]
    [SerializeField] private Door door_cyan_c;
    [SerializeField] private Door door_cyan_b;
    [SerializeField] private Door door_cyan_a;
    [SerializeField] private Door door_magenta_c;
    [SerializeField] private Door door_magenta_b;
    [SerializeField] private Door door_magenta_a;
    [SerializeField] private Door door_yellow_c;
    [SerializeField] private Door door_yellow_b;
    [SerializeField] private Door door_yellow_a;

    [Header("방 상태")]
    [SerializeField] private bool is_boss_room = false;

    // ─────────────────────────────────────────
    // 라운드 관리
    // ─────────────────────────────────────────
    private static int current_round = 0; // 씬 전환해도 유지
    public static int CurrentRound => current_round;

    // ─────────────────────────────────────────
    private bool is_cleared  = false;
    private bool is_checking = false;

    private string cyan_rank         = "";
    private string magenta_rank      = "";
    private string yellow_rank       = "";
    private string last_selected_rank = "C";

    [Header("이벤트")]
    public UnityEvent on_room_clear;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 씬 시작 시 라운드 증가
        current_round++;
        Debug.Log($"[RoomManager] 현재 라운드: {current_round}");

        // 8라운드면 보스방으로 설정
        if (current_round == 3)
        {
            is_boss_room = true;
            Debug.Log("[RoomManager] 8라운드 — 보스방!");
        }
    }

    private void Update()
    {
        if (!is_cleared && is_checking)
            CheckRoomClear();
    }

    // 방 시작 (SafeZone에서 호출)
    public void StartRoom()
    {
        if (is_checking) return;

        is_checking = true;
        CloseDoors();

        // 8라운드: 보스 소환
        if (current_round == 3)
        {
            spawner.SpawnBoss();
            Debug.Log("[RoomManager] 8라운드 — 보스 소환!");
        }

        Debug.Log($"[RoomManager] 방 시작! (라운드 {current_round})");
    }

    // 적 클리어 확인
    private void CheckRoomClear()
    {
        if (spawner == null) return;

        if (spawner.AreAllEnemiesDead())
            OnRoomClear();
    }

    // 방 클리어 처리
    private void OnRoomClear()
    {
        is_cleared  = true;
        is_checking = false;

        // 펫 라운드 카운트 알림
        Pet pet = FindAnyObjectByType<Pet>();
        if (pet != null) pet.OnRoomCleared();

        OpenDoorsWithRandomRanks();
        Debug.Log($"[RoomManager] 방 클리어! (라운드 {current_round})");

        on_room_clear?.Invoke();
    }

    // 문 닫기
    private void CloseDoors()
    {
        if (door_cyan_closed    != null) door_cyan_closed.SetActive(true);
        if (door_magenta_closed != null) door_magenta_closed.SetActive(true);
        if (door_yellow_closed  != null) door_yellow_closed.SetActive(true);

        DeactivateAllOpenedDoors();
        DeactivateAllDoorScripts();
    }

    // 문 열기
    private void OpenDoorsWithRandomRanks()
    {
        if (is_boss_room)
        {
            Debug.Log("[RoomManager] 보스 클리어! 게임 종료");
            return;
        }

        if (door_cyan_closed    != null) door_cyan_closed.SetActive(false);
        if (door_magenta_closed != null) door_magenta_closed.SetActive(false);
        if (door_yellow_closed  != null) door_yellow_closed.SetActive(false);

        cyan_rank    = GetRandomRank();
        magenta_rank = GetRandomRank();
        yellow_rank  = GetRandomRank();

        ActivateDoorByRank("Cyan",    cyan_rank);
        ActivateDoorByRank("Magenta", magenta_rank);
        ActivateDoorByRank("Yellow",  yellow_rank);

        Debug.Log($"[RoomManager] 문 열림 — Cyan:{cyan_rank}, Magenta:{magenta_rank}, Yellow:{yellow_rank}");
    }

    private string GetRandomRank()
    {
        switch (Random.Range(0, 3))
        {
            case 0: return "C";
            case 1: return "B";
            case 2: return "A";
            default: return "C";
        }
    }

    private void ActivateDoorByRank(string color, string rank)
    {
        if (color == "Cyan")
        {
            if (rank == "A" && door_cyan_opened_a != null) { door_cyan_opened_a.SetActive(true); door_cyan_a?.SetRank("A"); door_cyan_a?.SetInteractable(true); }
            else if (rank == "B" && door_cyan_opened_b != null) { door_cyan_opened_b.SetActive(true); door_cyan_b?.SetRank("B"); door_cyan_b?.SetInteractable(true); }
            else if (rank == "C" && door_cyan_opened_c != null) { door_cyan_opened_c.SetActive(true); door_cyan_c?.SetRank("C"); door_cyan_c?.SetInteractable(true); }
        }
        else if (color == "Magenta")
        {
            if (rank == "A" && door_magenta_opened_a != null) { door_magenta_opened_a.SetActive(true); door_magenta_a?.SetRank("A"); door_magenta_a?.SetInteractable(true); }
            else if (rank == "B" && door_magenta_opened_b != null) { door_magenta_opened_b.SetActive(true); door_magenta_b?.SetRank("B"); door_magenta_b?.SetInteractable(true); }
            else if (rank == "C" && door_magenta_opened_c != null) { door_magenta_opened_c.SetActive(true); door_magenta_c?.SetRank("C"); door_magenta_c?.SetInteractable(true); }
        }
        else if (color == "Yellow")
        {
            if (rank == "A" && door_yellow_opened_a != null) { door_yellow_opened_a.SetActive(true); door_yellow_a?.SetRank("A"); door_yellow_a?.SetInteractable(true); }
            else if (rank == "B" && door_yellow_opened_b != null) { door_yellow_opened_b.SetActive(true); door_yellow_b?.SetRank("B"); door_yellow_b?.SetInteractable(true); }
            else if (rank == "C" && door_yellow_opened_c != null) { door_yellow_opened_c.SetActive(true); door_yellow_c?.SetRank("C"); door_yellow_c?.SetInteractable(true); }
        }
    }

    private void DeactivateAllOpenedDoors()
    {
        if (door_cyan_opened_c    != null) door_cyan_opened_c.SetActive(false);
        if (door_cyan_opened_b    != null) door_cyan_opened_b.SetActive(false);
        if (door_cyan_opened_a    != null) door_cyan_opened_a.SetActive(false);
        if (door_magenta_opened_c != null) door_magenta_opened_c.SetActive(false);
        if (door_magenta_opened_b != null) door_magenta_opened_b.SetActive(false);
        if (door_magenta_opened_a != null) door_magenta_opened_a.SetActive(false);
        if (door_yellow_opened_c  != null) door_yellow_opened_c.SetActive(false);
        if (door_yellow_opened_b  != null) door_yellow_opened_b.SetActive(false);
        if (door_yellow_opened_a  != null) door_yellow_opened_a.SetActive(false);
    }

    private void DeactivateAllDoorScripts()
    {
        door_cyan_c?.SetInteractable(false);
        door_cyan_b?.SetInteractable(false);
        door_cyan_a?.SetInteractable(false);
        door_magenta_c?.SetInteractable(false);
        door_magenta_b?.SetInteractable(false);
        door_magenta_a?.SetInteractable(false);
        door_yellow_c?.SetInteractable(false);
        door_yellow_b?.SetInteractable(false);
        door_yellow_a?.SetInteractable(false);
    }

    public void LockAllDoors()
    {
        DeactivateAllDoorScripts();
        Debug.Log("[RoomManager] 모든 문 잠금 완료");
    }

    /// <summary>보스 처치 시 외부에서 직접 클리어 호출</summary>
    public void ClearBossRoom()
    {
        if (is_cleared) return;
        Debug.Log("[RoomManager] 보스방 클리어!");
        OnRoomClear();
    }

    public bool IsCleared()              => is_cleared;
    public bool IsBossRoom()             => is_boss_room;
    public string GetCyanRank()          => cyan_rank;
    public string GetMagentaRank()       => magenta_rank;
    public string GetYellowRank()        => yellow_rank;
    public void SetLastSelectedRank(string rank) { last_selected_rank = rank; Debug.Log($"[RoomManager] 선택된 문 등급: {rank}"); }
    public string GetLastSelectedRank()  => last_selected_rank;

    /// <summary>라운드 초기화 (타이틀/재시작 시 호출)</summary>
    public static void ResetRound() { current_round = 0; }
}