using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 각 문의 상호작용을 처리하는 클래스
/// </summary>
public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color magentaColor = new Color(1, 0, 1, 1);
    [SerializeField] private Color cyanColor = new Color(0, 1, 1, 1);
    [SerializeField] private Color yellowColor = new Color(1, 1, 0, 1);
    
    private string color;
    private int tier;
    
    private AbilityDataLoader abilityDataLoader;
    private CardUISystem cardUISystem;
    
    private bool isSelected = false;
    
    public string Color => color;
    public int Tier => tier;

    void Start()
    {
        abilityDataLoader = FindFirstObjectByType<AbilityDataLoader>();
        cardUISystem = FindFirstObjectByType<CardUISystem>();
        
        if (abilityDataLoader == null)
        {
            Debug.LogError("[DoorController] AbilityDataLoader를 찾을 수 없음");
        }
        
        if (cardUISystem == null)
        {
            Debug.LogError("[DoorController] CardUISystem을 찾을 수 없음");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnDoorSelected();
        }
    }

    /// <summary>
    /// 문 초기화 (DoorSystem에서 호출)
    /// </summary>
    public void Initialize(string doorColor, int doorTier)
    {
        color = doorColor;
        tier = doorTier;
        
        UpdateDoorColor();
        
        Debug.Log($"[DoorController] 문 초기화: {color} {tier}별");
    }

    /// <summary>
    /// 호환성: Init으로도 호출 가능 (RoomManager용)
    /// </summary>
    public void Init(string doorColor, int doorTier)
    {
        Initialize(doorColor, doorTier);
    }

    /// <summary>
    /// 문을 열기/닫기 (기존 코드 호환)
    /// </summary>
    public void SetOpen(bool isOpen)
    {
        if (isOpen)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 색깔에 따라 문의 색상 변경
    /// </summary>
    private void UpdateDoorColor()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        switch (color)
        {
            case "Magenta":
                spriteRenderer.color = magentaColor;
                break;
            case "Cyan":
                spriteRenderer.color = cyanColor;
                break;
            case "Yellow":
                spriteRenderer.color = yellowColor;
                break;
            default:
                Debug.LogWarning($"[DoorController] 미지원 색깔: {color}");
                break;
        }
    }

    /// <summary>
    /// 문 선택 (F키 누를 때)
    /// </summary>
    private void OnDoorSelected()
    {
        if (isSelected)
            return;

        isSelected = true;

        Debug.Log($"[DoorController] 문 선택: {color} {tier}별");

        if (abilityDataLoader == null)
        {
            Debug.LogError("[DoorController] AbilityDataLoader가 없음");
            return;
        }

        List<AbilityData> cards = abilityDataLoader.GetRandomAbilities(color, tier, 3);

        if (cards.Count == 0)
        {
            Debug.LogError($"[DoorController] {color} {tier}별 카드를 생성할 수 없음");
            return;
        }

        if (cardUISystem != null)
        {
            cardUISystem.DisplayCards(cards);
        }
        else
        {
            Debug.LogError("[DoorController] CardUISystem이 없음");
        }

        DisableOtherDoors();
    }

    /// <summary>
    /// 다른 문들 비활성화
    /// </summary>
    private void DisableOtherDoors()
    {
        DoorController[] allDoors = FindObjectsOfType<DoorController>();
        
        foreach (DoorController door in allDoors)
        {
            if (door != this)
            {
                door.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 선택 완료 후 호출
    /// </summary>
    public void OnCardSelected()
    {
        isSelected = false;
        gameObject.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (!isSelected)
        {
            spriteRenderer.color = spriteRenderer.color * 1.2f;
        }
    }

    void OnMouseExit()
    {
        if (!isSelected)
        {
            UpdateDoorColor();
        }
    }
}