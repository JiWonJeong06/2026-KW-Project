using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class AbilityCardUI : MonoBehaviour
{
    private static AbilityCardUI instance;
    public static AbilityCardUI Instance => instance;

    [Header("카드 프리팹 - Cyan")]
    [SerializeField] private GameObject card_prefab_cyan_c; // 연한색
    [SerializeField] private GameObject card_prefab_cyan_b; // 중간색
    [SerializeField] private GameObject card_prefab_cyan_a; // 진한색

    [Header("카드 프리팹 - Magenta")]
    [SerializeField] private GameObject card_prefab_magenta_c; // 연한색
    [SerializeField] private GameObject card_prefab_magenta_b; // 중간색
    [SerializeField] private GameObject card_prefab_magenta_a; // 진한색

    [Header("카드 프리팹 - Yellow")]
    [SerializeField] private GameObject card_prefab_yellow_c; // 연한색
    [SerializeField] private GameObject card_prefab_yellow_b; // 중간색
    [SerializeField] private GameObject card_prefab_yellow_a; // 진한색

    [Header("아이콘 스프라이트 (쪼갠 스프라이트 배열)")]
    [SerializeField] private Sprite[] ability_icons; // ⭐ 인스펙터에서 할당

    [Header("카드 부모 오브젝트 (Horizontal Layout Group)")]
    [SerializeField] private Transform card_container; // 카드 3개가 들어갈 부모

    [Header("선택 표시")]
    [SerializeField] private GameObject selection_indicator; // 선택 표시 오브젝트

    private List<AbilityItem> current_abilities = new List<AbilityItem>();
    private List<GameObject> card_instances = new List<GameObject>();
    private int selected_index = 1; // 기본 선택: 중앙 (0=좌, 1=중앙, 2=우)
    private bool is_selecting = false;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false); // 시작 시 비활성화
    }

    // Door에서 호출: 증강 선택 UI 표시
    public void ShowCards(string rank, string door_color)
    {
        if (is_selecting) return; // 이미 선택 중이면 무시

        gameObject.SetActive(true);
        is_selecting = true;
        selected_index = 1; // 중앙 선택

        // 해당 등급 + 하위 등급 + 해당 색깔 증강 3개 가져오기
        current_abilities = AbilityDataLoader.Instance.GetAbilitiesByRankOrLower(rank, door_color, 3);

        if (current_abilities.Count < 3)
        {
            Debug.LogError($"[AbilityCardUI] {door_color} {rank}등급(하위 포함) 증강이 부족합니다!");
            return;
        }

        // 카드 생성
        CreateCards(door_color);

        // Canvas 레이아웃 강제 업데이트 (Horizontal Layout Group이 위치 계산하도록)
        Canvas.ForceUpdateCanvases();

        // 선택 표시 위치 업데이트 (이제 카드 위치가 정확함)
        UpdateSelectionIndicator();

        Debug.Log($"[AbilityCardUI] {door_color} {rank}등급 카드 표시 완료");
    }

    private void CreateCards(string door_color)
    {
        // 기존 카드 삭제
        ClearCards();

        // 3개 카드 생성 (각 증강의 실제 등급에 맞는 프리팹 사용)
        for (int i = 0; i < 3; i++)
        {
            AbilityItem ability = current_abilities[i];
            
            // 증강의 실제 type과 rank에 맞는 프리팹 선택
            GameObject prefab = GetCardPrefab(ability.type, ability.rank);

            if (prefab == null)
            {
                Debug.LogError($"[AbilityCardUI] {ability.type} {ability.rank}등급 카드 프리팹이 없습니다!");
                continue;
            }

            // Horizontal Layout Group이 자동 정렬
            GameObject card = Instantiate(prefab, card_container);
            card_instances.Add(card);

            // 카드 데이터 설정
            SetupCard(card, ability);
        }
    }

    private GameObject GetCardPrefab(string color, string rank)
    {
        // 색깔 + 등급 조합으로 프리팹 선택
        if (color == "Cyan")
        {
            if (rank == "C") return card_prefab_cyan_c;
            if (rank == "B") return card_prefab_cyan_b;
            if (rank == "A") return card_prefab_cyan_a;
        }
        else if (color == "Magenta")
        {
            if (rank == "C") return card_prefab_magenta_c;
            if (rank == "B") return card_prefab_magenta_b;
            if (rank == "A") return card_prefab_magenta_a;
        }
        else if (color == "Yellow")
        {
            if (rank == "C") return card_prefab_yellow_c;
            if (rank == "B") return card_prefab_yellow_b;
            if (rank == "A") return card_prefab_yellow_a;
        }

        Debug.LogError($"[AbilityCardUI] 프리팹을 찾을 수 없음: {color} {rank}");
        return null;
    }

    private void SetupCard(GameObject card, AbilityItem ability)
    {
        // 카드 프리팹 안의 UI 요소 찾기
        TextMeshProUGUI name_text = card.transform.Find("AbilityName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI explanation_text = card.transform.Find("Explan")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI flavor_text = card.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        Image icon_image = card.transform.Find("Icon")?.GetComponent<Image>();

        // 증강 이름
        if (name_text != null)
        {
            name_text.text = ability.name;
        }

        // 증강 설명
        if (explanation_text != null)
        {
            explanation_text.text = ability.explanation;
        }

        // 게임 스토리 텍스트
        if (flavor_text != null)
        {
            flavor_text.text = ability.text;
        }

        // 아이콘 이미지 (배열에서 인덱스로 가져오기)
        if (icon_image != null && ability_icons != null && ability_icons.Length > 0)
        {
            // 인덱스 범위 확인
            if (ability.icon_index >= 0 && ability.icon_index < ability_icons.Length)
            {
                icon_image.sprite = ability_icons[ability.icon_index];
                Debug.Log($"[AbilityCardUI] 아이콘 설정 성공: {ability.name} (index: {ability.icon_index})");
            }
            else
            {
                Debug.LogWarning($"[AbilityCardUI] 아이콘 인덱스 범위 초과: {ability.icon_index} (배열 크기: {ability_icons.Length})");
            }
        }
    }

    private void ClearCards()
    {
        foreach (GameObject card in card_instances)
        {
            Destroy(card);
        }
        card_instances.Clear();
    }

    private void Update()
    {
        if (!is_selecting) return;

        // 좌우 키 입력
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }
        else if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
        {
            ConfirmSelection();
        }
    }

    private void MoveSelection(int direction)
    {
        selected_index += direction;

        // 순환 (0 ↔ 1 ↔ 2)
        if (selected_index < 0) selected_index = 2;
        if (selected_index > 2) selected_index = 0;

        UpdateSelectionIndicator();
        Debug.Log($"[AbilityCardUI] 선택 이동: {selected_index}");
    }

    private void UpdateSelectionIndicator()
    {
        if (selection_indicator == null)
        {
            Debug.LogWarning("[AbilityCardUI] SelectionIndicator가 없습니다!");
            return;
        }

        if (card_instances.Count < 3)
        {
            Debug.LogWarning("[AbilityCardUI] 카드가 3개 미만입니다!");
            return;
        }

        // 선택 표시를 현재 선택된 카드 위치로 이동
        RectTransform selected_card_rect = card_instances[selected_index].GetComponent<RectTransform>();
        if (selected_card_rect != null)
        {
            // RectTransform의 위치를 정확히 복사
            RectTransform indicator_rect = selection_indicator.GetComponent<RectTransform>();
            if (indicator_rect != null)
            {
                indicator_rect.position = selected_card_rect.position;
            }
            else
            {
                selection_indicator.transform.position = selected_card_rect.position;
            }

            selection_indicator.SetActive(true);
            Debug.Log($"[AbilityCardUI] SelectionIndicator 위치 업데이트: 카드 {selected_index}");
        }
        else
        {
            Debug.LogError("[AbilityCardUI] 선택된 카드에 RectTransform이 없습니다!");
        }
    }

    private void ConfirmSelection()
    {
        AbilityItem selected_ability = current_abilities[selected_index];

        Debug.Log($"[AbilityCardUI] 증강 선택 확정: {selected_ability.name}");

        // 1. PlayerStats에 증강 적용
        PlayerStats.Instance.ApplyAbility(selected_ability);

        // 2. AbilityDataLoader에 선택 기록 (중복 방지)
        AbilityDataLoader.Instance.SelectAbility(selected_ability.number);

        // 3. 플레이어 무기 외형 업데이트
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            player.UpdateWeaponVisual();
        }
        else
        {
            Debug.LogWarning("[AbilityCardUI] Player를 찾을 수 없습니다!");
        }

        // 4. UI 닫기
        CloseUI();

        // 5. 다음 맵 로드 (TODO)
        LoadNextMap();
    }

    private void CloseUI()
    {
        is_selecting = false;
        ClearCards();
        gameObject.SetActive(false);
        
        if (selection_indicator != null)
        {
            selection_indicator.SetActive(false);
        }
    }

    private void LoadNextMap()
    {
        // 선택한 문의 등급을 난이도로 변환
        string difficulty = ConvertRankToDifficulty(RoomManager.Instance.GetLastSelectedRank());

        Debug.Log($"[AbilityCardUI] 다음 맵 로드: {difficulty} 난이도");

        // GameManager에 다음 스테이지 로드 요청
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextStage(difficulty);
        }
        else
        {
            Debug.LogError("[AbilityCardUI] GameManager를 찾을 수 없습니다!");
        }
    }

    // 등급 → 난이도 변환
    private string ConvertRankToDifficulty(string rank)
    {
        switch (rank)
        {
            case "A": return "Hard";
            case "B": return "Normal";
            case "C": return "Easy";
            default:
                Debug.LogWarning($"[AbilityCardUI] 알 수 없는 등급: {rank}. Easy로 설정.");
                return "Easy";
        }
    }
}