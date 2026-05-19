using UnityEngine;

/// <summary>
/// CardUISystem과 StageManager를 연동하는 UI 컨트롤러
/// 
/// 역할:
/// 1. CardUISystem에서 카드 선택 감지
/// 2. StageManager에 선택 완료 신호
/// 3. UI 전환
/// </summary>
public class AbilityUIController : MonoBehaviour
{
    private CardUISystem cardUISystem;
    private StageManager stageManager;
    private AbilityManager abilityManager;
    
    private AbilityData selectedAbility;
    private bool isWaitingForSelection = false;

    void Start()
    {
        cardUISystem = FindFirstObjectByType<CardUISystem>();
        stageManager = FindFirstObjectByType<StageManager>();
        abilityManager = FindFirstObjectByType<AbilityManager>();

        if (cardUISystem == null)
            Debug.LogError("[AbilityUIController] CardUISystem을 찾을 수 없음");
        if (stageManager == null)
            Debug.LogError("[AbilityUIController] StageManager를 찾을 수 없음");
        if (abilityManager == null)
            Debug.LogError("[AbilityUIController] AbilityManager를 찾을 수 없음");
    }

    /// <summary>
    /// CardUISystem에서 호출 (카드 선택 완료)
    /// </summary>
    public void OnAbilitySelected(AbilityData ability)
    {
        if (isWaitingForSelection)
            return;  // 중복 방지

        selectedAbility = ability;
        isWaitingForSelection = true;

        Debug.Log($"[AbilityUIController] 증강 선택: {ability.name}");

        // 1.5초 후 다음 스테이지로 진행
        Invoke(nameof(ProceedToNextStage), 1.5f);
    }

    /// <summary>
    /// 다음 스테이지로 진행
    /// </summary>
    private void ProceedToNextStage()
    {
        if (stageManager != null)
        {
            stageManager.OnAbilitySelected();
        }

        isWaitingForSelection = false;

        Debug.Log("[AbilityUIController] 다음 스테이지로 진행");
    }

    /// <summary>
    /// 선택된 증강 반환
    /// </summary>
    public AbilityData GetSelectedAbility()
    {
        return selectedAbility;
    }
}