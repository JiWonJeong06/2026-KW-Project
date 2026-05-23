using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    [Header("스폰 관리")]
    [SerializeField] private Spawner spawner;

    [Header("닫힌 문 (각 1개)")]
    [SerializeField] private GameObject door_cyan_closed;
    [SerializeField] private GameObject door_magenta_closed;
    [SerializeField] private GameObject door_yellow_closed;

    [Header("열린 문 - Cyan (등급별 3개)")]
    [SerializeField] private GameObject door_cyan_opened_c; // 연한문 (C등급)
    [SerializeField] private GameObject door_cyan_opened_b; // 중간문 (B등급)
    [SerializeField] private GameObject door_cyan_opened_a; // 진한문 (A등급)

    [Header("열린 문 - Magenta (등급별 3개)")]
    [SerializeField] private GameObject door_magenta_opened_c; // 연한문 (C등급)
    [SerializeField] private GameObject door_magenta_opened_b; // 중간문 (B등급)
    [SerializeField] private GameObject door_magenta_opened_a; // 진한문 (A등급)

    [Header("열린 문 - Yellow (등급별 3개)")]
    [SerializeField] private GameObject door_yellow_opened_c; // 연한문 (C등급)
    [SerializeField] private GameObject door_yellow_opened_b; // 중간문 (B등급)
    [SerializeField] private GameObject door_yellow_opened_a; // 진한문 (A등급)

    [Header("방 상태")]
    [SerializeField] private bool is_boss_room = false;

    private bool is_cleared = false;
    private bool is_checking = false;

    private string cyan_rank = "";
    private string magenta_rank = "";
    private string yellow_rank = "";

    [Header("이벤트")]
    public UnityEvent on_room_clear;

    private void Update()
    {
        if (!is_cleared && is_checking)
        {
            CheckRoomClear();
        }
    }

    // 방 시작 (SafeZone에서 호출)
    public void StartRoom()
    {
        if (is_checking) return;
        is_checking = true;
        CloseDoors();
        Debug.Log("방 시작!");
    }

    // 적 클리어 확인
    private void CheckRoomClear()
    {
        if (spawner == null) return;

        if (spawner.AreAllEnemiesDead())
        {
            OnRoomClear();
        }
    }

    // 방 클리어 처리
    private void OnRoomClear()
    {
        is_cleared = true;
        is_checking = false;

        OpenDoorsWithRandomRanks();
        Debug.Log("방 클리어!");

        // 이벤트 발생
        on_room_clear?.Invoke();
    }

    // 문 닫기 (전투 시작)
    private void CloseDoors()
    {
        // 닫힌 문 활성화
        if (door_cyan_closed != null) door_cyan_closed.SetActive(true);
        if (door_magenta_closed != null) door_magenta_closed.SetActive(true);
        if (door_yellow_closed != null) door_yellow_closed.SetActive(true);

        // 모든 열린 문 비활성화
        DeactivateAllOpenedDoors();
    }

    // 문 열기 (방 클리어, 등급 랜덤)
    private void OpenDoorsWithRandomRanks()
    {
        // 보스방에서는 문을 열지 않음
        if (is_boss_room)
        {
            Debug.Log("보스 클리어! 게임 종료");
            return;
        }

        // 닫힌 문 비활성화
        if (door_cyan_closed != null) door_cyan_closed.SetActive(false);
        if (door_magenta_closed != null) door_magenta_closed.SetActive(false);
        if (door_yellow_closed != null) door_yellow_closed.SetActive(false);

        // 각 색깔별로 A/B/C 중 랜덤 선택
        cyan_rank = GetRandomRank();
        magenta_rank = GetRandomRank();
        yellow_rank = GetRandomRank();

        // 선택된 등급의 문만 활성화
        ActivateDoorByRank("Cyan", cyan_rank);
        ActivateDoorByRank("Magenta", magenta_rank);
        ActivateDoorByRank("Yellow", yellow_rank);

        Debug.Log($"문 열림 - Cyan: {cyan_rank}, Magenta: {magenta_rank}, Yellow: {yellow_rank}");
    }

    // 랜덤 등급 선택 (A, B, C)
    private string GetRandomRank()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: return "C";
            case 1: return "B";
            case 2: return "A";
            default: return "C";
        }
    }

    // 특정 색깔의 특정 등급 문 활성화
    private void ActivateDoorByRank(string color, string rank)
    {
        if (color == "Cyan")
        {
            if (rank == "A" && door_cyan_opened_a != null) door_cyan_opened_a.SetActive(true);
            else if (rank == "B" && door_cyan_opened_b != null) door_cyan_opened_b.SetActive(true);
            else if (rank == "C" && door_cyan_opened_c != null) door_cyan_opened_c.SetActive(true);
        }
        else if (color == "Magenta")
        {
            if (rank == "A" && door_magenta_opened_a != null) door_magenta_opened_a.SetActive(true);
            else if (rank == "B" && door_magenta_opened_b != null) door_magenta_opened_b.SetActive(true);
            else if (rank == "C" && door_magenta_opened_c != null) door_magenta_opened_c.SetActive(true);
        }
        else if (color == "Yellow")
        {
            if (rank == "A" && door_yellow_opened_a != null) door_yellow_opened_a.SetActive(true);
            else if (rank == "B" && door_yellow_opened_b != null) door_yellow_opened_b.SetActive(true);
            else if (rank == "C" && door_yellow_opened_c != null) door_yellow_opened_c.SetActive(true);
        }
    }

    // 모든 열린 문 비활성화
    private void DeactivateAllOpenedDoors()
    {
        // Cyan
        if (door_cyan_opened_c != null) door_cyan_opened_c.SetActive(false);
        if (door_cyan_opened_b != null) door_cyan_opened_b.SetActive(false);
        if (door_cyan_opened_a != null) door_cyan_opened_a.SetActive(false);

        // Magenta
        if (door_magenta_opened_c != null) door_magenta_opened_c.SetActive(false);
        if (door_magenta_opened_b != null) door_magenta_opened_b.SetActive(false);
        if (door_magenta_opened_a != null) door_magenta_opened_a.SetActive(false);

        // Yellow
        if (door_yellow_opened_c != null) door_yellow_opened_c.SetActive(false);
        if (door_yellow_opened_b != null) door_yellow_opened_b.SetActive(false);
        if (door_yellow_opened_a != null) door_yellow_opened_a.SetActive(false);
    }

    // 외부에서 방 클리어 상태 확인
    public bool IsCleared() => is_cleared;

    // 보스방 여부 확인
    public bool IsBossRoom() => is_boss_room;

    // 각 문의 등급 확인 (DoorSystem에서 사용)
    public string GetCyanRank() => cyan_rank;
    public string GetMagentaRank() => magenta_rank;
    public string GetYellowRank() => yellow_rank;
}