using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class AbilityItem
{
    public int number;
    public string type;              // "Magenta", "Cyan", "Yellow"
    public string name;
    public int icon_index;           // ⭐ 스프라이트 배열 인덱스
    public string rank;              // "C", "B", "A"
    public string mainAbility;
    public string subAbility;
    public float increase;
    public float subIncrease;
    public string explanation;
    public string text;
}

[System.Serializable]
public class AbilityDataList
{
    public List<AbilityItem> abilities;
}

public class AbilityDataLoader : MonoBehaviour
{
    private static AbilityDataLoader instance;
    public static AbilityDataLoader Instance => instance;

    [SerializeField] private TextAsset ability_json;

    private List<AbilityItem> all_abilities = new List<AbilityItem>();
    private HashSet<int> selected_ability_numbers = new HashSet<int>(); // 선택된 증강 ID

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        if (ability_json == null)
        {
            Debug.LogError("Ability JSON 파일이 지정되지 않았습니다.");
            return;
        }

        string json_text = ability_json.text;
        all_abilities = JsonHelper.FromJson<AbilityItem>(json_text);

        Debug.Log($"증강 데이터 로드 완료: {all_abilities.Count}개");
    }

    // 등급별 증강 가져오기 (중복 허용)
    public List<AbilityItem> GetAbilitiesByRank(string rank, int count = 3)
    {
        // 해당 등급의 증강 필터링 (중복 허용)
        List<AbilityItem> available = all_abilities
            .Where(a => a.rank == rank)
            .ToList();

        if (available.Count < count)
        {
            Debug.LogWarning($"{rank}등급 증강이 {count}개 미만입니다. (남은 개수: {available.Count})");
        }

        // 랜덤으로 count개 선택 (중복 가능)
        List<AbilityItem> selected = available.OrderBy(x => Random.value).Take(count).ToList();
        return selected;
    }

    // 등급 + 하위 등급 + 색깔별 증강 가져오기
    public List<AbilityItem> GetAbilitiesByRankOrLower(string rank, string type, int count = 3)
    {
        List<string> allowed_ranks = new List<string>();
        
        if (rank == "C")
        {
            allowed_ranks.Add("C");
        }
        else if (rank == "B")
        {
            allowed_ranks.Add("B");
            allowed_ranks.Add("C");
        }
        else if (rank == "A")
        {
            allowed_ranks.Add("A");
            allowed_ranks.Add("B");
            allowed_ranks.Add("C");
        }

        // 허용된 등급 + 해당 type의 증강 필터링 (중복 허용)
        List<AbilityItem> available = all_abilities
            .Where(a => allowed_ranks.Contains(a.rank) 
                     && a.type == type)
            .ToList();

        if (available.Count < count)
        {
            Debug.LogWarning($"{type} {rank}등급(하위 포함) 증강이 {count}개 미만입니다. (남은 개수: {available.Count})");
        }

        // 랜덤으로 count개 선택 (중복 가능)
        List<AbilityItem> selected = available.OrderBy(x => Random.value).Take(count).ToList();
        
        Debug.Log($"[AbilityDataLoader] {type} {rank}등급 증강 선택: {string.Join(", ", selected.Select(a => $"{a.name}({a.rank}, {a.number})"))}");
        
        return selected;
    }

    // 증강 선택 완료 (중복 방지용)
    public void SelectAbility(int ability_number)
    {
        selected_ability_numbers.Add(ability_number);
        Debug.Log($"증강 선택 완료: {ability_number} (총 {selected_ability_numbers.Count}개 선택됨)");
    }

    // 선택된 증강 초기화 (새 게임 시작 시)
    public void ResetSelectedAbilities()
    {
        selected_ability_numbers.Clear();
        Debug.Log("선택된 증강 초기화");
    }

    public AbilityItem GetAbilityByNumber(int number)
    {
        return all_abilities.FirstOrDefault(a => a.number == number);
    }
}

// JSON 배열 파싱 헬퍼
public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        string wrapped_json = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped_json);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}