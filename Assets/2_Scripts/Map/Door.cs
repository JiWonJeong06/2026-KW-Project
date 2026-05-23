using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("문 정보")]
    [SerializeField] private string door_color; // "Cyan", "Magenta", "Yellow"
    
    [Header("연동")]
    [SerializeField] private RoomManager room_manager;

    private string door_rank = ""; // "A", "B", "C" (RoomManager에서 설정)
    private bool player_nearby = false;
    private bool is_interactable = false;
    private Player current_player = null; // 현재 범위 내 플레이어

    private void Update()
    {
        if (!is_interactable) return;
        if (!player_nearby) return;

        // F키 입력 확인 (New Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            OnDoorInteract();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player_nearby = true;
            current_player = player;
            ShowInteractPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player_nearby = false;
            current_player = null;
            HideInteractPrompt();
        }
    }

    private void OnDoorInteract()
    {
        Debug.Log($"[Door] {door_color} {door_rank}등급 문 선택!");

        // F키 프롬프트 즉시 숨김
        HideInteractPrompt();

        // 모든 문 상호작용 비활성화 (선택 잠금)
        if (room_manager != null)
        {
            room_manager.LockAllDoors();
        }

        // TODO: AbilityCardUI 호출 (다음 단계에서 구현)
        // AbilityCardUI.Instance.ShowCards(door_rank);

        // TODO: 증강 선택 완료 후 다음 맵 로드
        // SceneManager.LoadScene("NextStage");
        
        // 임시: 선택 완료 로그
        Debug.Log($"[Door] {door_color} {door_rank}등급 문으로 이동 확정! (더 이상 변경 불가)");
    }

    private void ShowInteractPrompt()
    {
        if (is_interactable && current_player != null)
        {
            current_player.ShowFKeyPrompt();
            Debug.Log($"[Door] F키를 눌러 {door_color} {door_rank}등급 문으로 이동");
        }
    }

    private void HideInteractPrompt()
    {
        if (current_player != null)
        {
            current_player.HideFKeyPrompt();
        }
    }

    // RoomManager에서 문이 열릴 때 호출
    public void SetInteractable(bool state)
    {
        is_interactable = state;
        Debug.Log($"[Door] {door_color} 문 상호작용 가능: {state}");
    }

    // RoomManager에서 등급 설정
    public void SetRank(string rank)
    {
        door_rank = rank;
        Debug.Log($"[Door] {door_color} 문 등급 설정: {rank}");
    }

    public string GetColor() => door_color;
    public string GetRank() => door_rank;
}