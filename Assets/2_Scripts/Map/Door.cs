using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("문 정보")]
    [SerializeField] private string door_color; // "Cyan", "Magenta", "Yellow"
    
    [Header("상호작용 설정")]
    [SerializeField] private float interaction_range = 2f; // 상호작용 범위
    
    [Header("연동")]
    [SerializeField] private RoomManager room_manager;

    private string door_rank = ""; // "A", "B", "C" (RoomManager에서 설정)
    private bool player_nearby = false;
    private bool is_interactable = false;
    private Player current_player = null; // 현재 범위 내 플레이어

    private void Update()
    {
        // 플레이어와의 거리 체크
        CheckPlayerDistance();

        // 상호작용 불가 상태면 F키 입력 무시
        if (!is_interactable) return;
        if (!player_nearby) return;

        // F키 입력 확인 (New Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            OnDoorInteract();
        }
    }

    private void CheckPlayerDistance()
    {
        // Player 찾기
        Player player = Object.FindAnyObjectByType<Player>();
        
        if (player == null) return;

        // 플레이어와의 거리 계산
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // 범위 안에 들어옴
        if (distance <= interaction_range)
        {
            if (!player_nearby)
            {
                player_nearby = true;
                current_player = player;
                ShowInteractPrompt();
            }
        }
        // 범위 밖으로 나감
        else
        {
            if (player_nearby)
            {
                // 순서 중요: HideInteractPrompt 먼저, current_player = null은 나중에!
                HideInteractPrompt();
                player_nearby = false;
                current_player = null;
            }
        }
    }

    private void OnDisable()
    {
        // Door 오브젝트가 비활성화될 때 F키 프롬프트 숨김 (안전장치)
        HideInteractPrompt();
    }

    private void OnDoorInteract()
    {
        Debug.Log($"[Door] {door_color} {door_rank}등급 문 선택!");

        // F키 프롬프트 즉시 숨김
        HideInteractPrompt();

        // 선택된 문의 등급을 RoomManager에 저장
        if (room_manager != null)
        {
            room_manager.SetLastSelectedRank(door_rank);
        }

        // 모든 문 상호작용 비활성화 (선택 잠금)
        if (room_manager != null)
        {
            room_manager.LockAllDoors();
        }

        // AbilityCardUI 호출
        if (AbilityCardUI.Instance != null)
        {
            AbilityCardUI.Instance.ShowCards(door_rank, door_color);
        }
        else
        {
            Debug.LogError("[Door] AbilityCardUI가 씬에 없습니다!");
        }
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
        
        // 상호작용 비활성화 시 F키 프롬프트 강제 숨김 (안전장치)
        if (!state)
        {
            HideInteractPrompt();
        }
        
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

    // 상호작용 범위 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interaction_range);
    }
}