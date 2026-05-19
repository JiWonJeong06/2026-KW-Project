using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ability_DataTable.json을 로드하고 관리하는 클래스
/// 
/// 개선사항:
/// 1. 선택한 증강 이력 저장
/// 2. 같은 증강 중복 선택 방지
/// 3. 색깔별 + 티어별 필터링
/// 4. 랜덤 카드 선택
/// </summary>
public class AbilityDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset abilityJsonFile;
    
    private AbilityData[] allAbilities;
    private Dictionary<string, List<AbilityData>> abilitiesByType;  // 색깔별 분류
    
    // ===== NEW: 선택 이력 저장 =====
    private HashSet<int> selectedAbilityNumbers = new HashSet<int>();  // 선택한 증강 number 저장
    
    void Awake()
    {
        LoadAbilities();
    }

    /// <summary>
    /// JSON 파일에서 증강 데이터 로드
    /// </summary>
    public void LoadAbilities()
    {
        if (abilityJsonFile == null)
        {
            Debug.LogError("[AbilityDataLoader] Ability_DataTable.json 파일이 할당되지 않음");
            return;
        }

        // JSON 파싱
        string json = abilityJsonFile.text;
        string wrappedJson = "{\"abilities\":" + json + "}";
        
        AbilityDataWrapper wrapper = JsonUtility.FromJson<AbilityDataWrapper>(wrappedJson);
        
        if (wrapper == null || wrapper.abilities == null)
        {
            Debug.LogError("[AbilityDataLoader] JSON 파싱 실패");
            return;
        }

        allAbilities = wrapper.abilities;
        InitializeTypeIndex();
        
        Debug.Log($"[AbilityDataLoader] 증강 데이터 로드 완료: {allAbilities.Length}개");
    }

    /// <summary>
    /// 색깔별로 증강 데이터 분류
    /// </summary>
    private void InitializeTypeIndex()
    {
        abilitiesByType = new Dictionary<string, List<AbilityData>>
        {
            { "Magenta", new List<AbilityData>() },
            { "Cyan", new List<AbilityData>() },
            { "Yellow", new List<AbilityData>() }
        };

        foreach (AbilityData ability in allAbilities)
        {
            if (abilitiesByType.ContainsKey(ability.type))
            {
                abilitiesByType[ability.type].Add(ability);
            }
        }

        Debug.Log($"[AbilityDataLoader] 타입별 분류 완료");
    }

    /// <summary>
    /// 색깔과 티어에 맞는 랜덤 카드 3개 선택 (중복 제외)
    /// </summary>
    public List<AbilityData> GetRandomAbilities(string doorColor, int doorTier, int cardCount = 3)
    {
        if (!abilitiesByType.ContainsKey(doorColor))
        {
            Debug.LogError($"[AbilityDataLoader] 미지원 색깔: {doorColor}");
            return new List<AbilityData>();
        }

        List<AbilityData> colorAbilities = abilitiesByType[doorColor];
        List<AbilityData> filtered = FilterByTier(colorAbilities, doorTier);

        // ===== NEW: 이미 선택한 증강 제외 =====
        List<AbilityData> available = filtered.Where(a => !selectedAbilityNumbers.Contains(a.number)).ToList();

        if (available.Count == 0)
        {
            Debug.LogWarning($"[AbilityDataLoader] {doorColor} {doorTier}별 중복되지 않은 카드가 없음. 모든 카드 사용 가능하게 초기화.");
            ResetSelectedAbilities();
            available = filtered;
        }

        return SelectRandomCards(available, cardCount);
    }

    /// <summary>
    /// 티어에 따라 필터링
    /// </summary>
    private List<AbilityData> FilterByTier(List<AbilityData> abilities, int tier)
    {
        List<AbilityData> filtered = new List<AbilityData>();

        foreach (AbilityData ability in abilities)
        {
            bool matches = false;

            if (tier == 1)
            {
                if (ability.rank == "C")
                    matches = true;
            }
            else if (tier == 2)
            {
                if (ability.rank == "C" || ability.rank == "B")
                    matches = true;
            }
            else if (tier == 3)
            {
                matches = true;
            }

            if (matches)
                filtered.Add(ability);
        }

        return filtered;
    }

    /// <summary>
    /// 필터된 카드 중 중복 없이 랜덤 선택
    /// </summary>
    private List<AbilityData> SelectRandomCards(List<AbilityData> candidates, int count)
    {
        List<AbilityData> result = new List<AbilityData>();
        List<AbilityData> temp = new List<AbilityData>(candidates);

        int selectCount = Mathf.Min(count, temp.Count);

        for (int i = 0; i < selectCount; i++)
        {
            int randomIndex = Random.Range(0, temp.Count);
            result.Add(temp[randomIndex]);
            temp.RemoveAt(randomIndex);
        }

        return result;
    }

    // ===== NEW: 선택 이력 관리 메서드 =====

    /// <summary>
    /// 증강 선택 기록 (AbilityManager에서 호출)
    /// </summary>
    public void RecordSelectedAbility(int abilityNumber)
    {
        selectedAbilityNumbers.Add(abilityNumber);
        Debug.Log($"[AbilityDataLoader] 증강 선택 기록: {abilityNumber} (총 {selectedAbilityNumbers.Count}개 선택)");
    }

    /// <summary>
    /// 선택 이력 초기화 (게임 끝날 때 등)
    /// </summary>
    public void ResetSelectedAbilities()
    {
        selectedAbilityNumbers.Clear();
        Debug.Log($"[AbilityDataLoader] 선택 이력 초기화");
    }

    /// <summary>
    /// 선택한 증강 개수 반환
    /// </summary>
    public int GetSelectedAbilityCount()
    {
        return selectedAbilityNumbers.Count;
    }

    /// <summary>
    /// 특정 증강이 이미 선택되었는지 확인
    /// </summary>
    public bool IsAbilitySelected(int abilityNumber)
    {
        return selectedAbilityNumbers.Contains(abilityNumber);
    }

    /// <summary>
    /// 특정 number의 증강 데이터 찾기
    /// </summary>
    public AbilityData GetAbilityByNumber(int number)
    {
        return allAbilities.FirstOrDefault(a => a.number == number);
    }

    /// <summary>
    /// 모든 증강 데이터 반환
    /// </summary>
    public AbilityData[] GetAllAbilities()
    {
        return allAbilities;
    }

    /// <summary>
    /// 색깔별 증강 개수
    /// </summary>
    public int GetAbilityCount(string type)
    {
        return abilitiesByType.ContainsKey(type) ? abilitiesByType[type].Count : 0;
    }
}