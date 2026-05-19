using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// CardUISystem 수정 버전
/// AbilityUIController와 연동
/// </summary>
public class CardUISystem : MonoBehaviour
{
    [SerializeField] private Transform[] cardUIPositions;
    [SerializeField] private GameObject cardUIPrefab;
    
    private List<AbilityData> currentCards;
    private int selectedIndex = 0;
    private GameObject[] cardUIObjects = new GameObject[3];
    
    private AbilityManager abilityManager;
    private AbilityUIController abilityUIController;
    private DoorController currentDoor;
    
    private bool isSelecting = false;

    void Start()
    {
        abilityManager = FindFirstObjectByType<AbilityManager>();
        abilityUIController = FindFirstObjectByType<AbilityUIController>();
        
        if (abilityManager == null)
            Debug.LogError("[CardUISystem] AbilityManager를 찾을 수 없음");
        if (abilityUIController == null)
            Debug.LogError("[CardUISystem] AbilityUIController를 찾을 수 없음");
    }

    void Update()
    {
        if (!isSelecting)
            return;

        // ← 키: 왼쪽 카드로 이동
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0)
                selectedIndex = currentCards.Count - 1;
            
            UpdateCardDisplay();
        }

        // → 키: 오른쪽 카드로 이동
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
            if (selectedIndex >= currentCards.Count)
                selectedIndex = 0;
            
            UpdateCardDisplay();
        }

        // Enter 키: 선택 확정
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnCardConfirmed();
        }
    }

    /// <summary>
    /// 카드 3개 표시
    /// </summary>
    public void DisplayCards(List<AbilityData> cards)
    {
        currentCards = cards;
        selectedIndex = 0;
        isSelecting = true;

        Debug.Log($"[CardUISystem] 카드 3개 표시");

        // 1. 기존 카드 UI 제거
        for (int i = 0; i < cardUIObjects.Length; i++)
        {
            if (cardUIObjects[i] != null)
                Destroy(cardUIObjects[i]);
        }

        // 2. 카드 UI 생성
        for (int i = 0; i < 3; i++)
        {
            if (i < cards.Count)
            {
                GameObject cardUI = Instantiate(cardUIPrefab, cardUIPositions[i]);
                cardUIObjects[i] = cardUI;
                UpdateCardUI(i, cards[i]);
            }
        }

        // 3. 첫 번째 카드 선택 표시
        UpdateCardDisplay();
    }

    /// <summary>
    /// 카드 UI 업데이트
    /// </summary>
    private void UpdateCardUI(int index, AbilityData ability)
    {
        GameObject cardUI = cardUIObjects[index];
        
        if (cardUI == null)
            return;

        // 이름
        TextMeshProUGUI nameText = cardUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = ability.name;

        // 등급
        TextMeshProUGUI rankText = cardUI.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
        if (rankText != null)
            rankText.text = $"{ability.rank}급";

        // 설명
        TextMeshProUGUI descriptionText = cardUI.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        if (descriptionText != null)
            descriptionText.text = ability.explanation;

        // 능력
        TextMeshProUGUI abilityText = cardUI.transform.Find("AbilityText")?.GetComponent<TextMeshProUGUI>();
        if (abilityText != null)
        {
            string abilityInfo = $"{ability.mainAbility} +{ability.increase}";
            if (ability.HasSubAbility())
                abilityInfo += $"\n{ability.subAbility} +{ability.subIncrease}";
            
            abilityText.text = abilityInfo;
        }

        Debug.Log($"[CardUISystem] 카드 {index} 업데이트: {ability.name}");
    }

    /// <summary>
    /// 카드 선택 표시 업데이트
    /// </summary>
    private void UpdateCardDisplay()
    {
        for (int i = 0; i < cardUIObjects.Length; i++)
        {
            if (cardUIObjects[i] == null)
                continue;

            Image cardImage = cardUIObjects[i].GetComponent<Image>();
            if (cardImage != null)
            {
                if (i == selectedIndex)
                {
                    cardImage.color = Color.white;
                    
                    Outline outline = cardUIObjects[i].GetComponent<Outline>();
                    if (outline == null)
                        outline = cardUIObjects[i].AddComponent<Outline>();
                    outline.enabled = true;
                }
                else
                {
                    cardImage.color = Color.gray;
                    
                    Outline outline = cardUIObjects[i].GetComponent<Outline>();
                    if (outline != null)
                        outline.enabled = false;
                }
            }
        }

        Debug.Log($"[CardUISystem] 카드 선택: {selectedIndex}");
    }

    /// <summary>
    /// 카드 선택 확정
    /// </summary>
    private void OnCardConfirmed()
    {
        isSelecting = false;

        AbilityData selectedAbility = currentCards[selectedIndex];

        Debug.Log($"[CardUISystem] 카드 확정: {selectedAbility.name}");

        // 1. 능력 적용
        if (abilityManager != null)
            abilityManager.ApplyAbility(selectedAbility);

        // 2. UI 컨트롤러에 알림 ← NEW!
        if (abilityUIController != null)
            abilityUIController.OnAbilitySelected(selectedAbility);

        // 3. 카드 UI 종료
        HideCards();

        // 4. 문 닫기
        DoorController[] allDoors = FindObjectsOfType<DoorController>();
        foreach (DoorController door in allDoors)
        {
            door.OnCardSelected();
        }
    }

    /// <summary>
    /// 카드 UI 숨기기
    /// </summary>
    private void HideCards()
    {
        for (int i = 0; i < cardUIObjects.Length; i++)
        {
            if (cardUIObjects[i] != null)
            {
                Destroy(cardUIObjects[i]);
                cardUIObjects[i] = null;
            }
        }

        currentCards = null;
        isSelecting = false;

        Debug.Log("[CardUISystem] 카드 UI 숨김");
    }

    /// <summary>
    /// 선택된 카드 반환
    /// </summary>
    public AbilityData GetSelectedCard()
    {
        if (currentCards != null && selectedIndex < currentCards.Count)
            return currentCards[selectedIndex];
        
        return null;
    }
}