using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats instance;
    public static PlayerStats Instance => instance;

    [Header("기본 스탯")]
    public float base_hp = 10f;
    public float base_move_speed = 5f;
    public float base_atk = 1f;
    public float base_atk_speed = 1f;
    public float base_bullet_speed = 10f;
    public float base_range = 10f;

    [Header("현재 스탯 (증강 적용 후)")]
    public float current_max_hp;
    public float current_move_speed;
    public float current_atk;
    public float current_atk_speed;
    public float current_bullet_speed;
    public float current_range;
    public int current_bullet_count = 1;  // 추가 발사 개수
    public bool has_pierce = false;       // 관통 여부
    public bool has_bleed = false;        // 출혈 여부

    [Header("펫 스탯")]
    public float pet_atk = 1f;
    public float pet_cd = 5f;
    public float pet_hp_restore = 0f;
    public float pet_add_bullet_chance = 0f; // %

    private List<AbilityItem> applied_abilities = new List<AbilityItem>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadBaseStatsFromPlayerData();
        InitializeStats();
    }

    private void LoadBaseStatsFromPlayerData()
    {
        PlayerData player_data = PlayerDataLoader.Instance.GetPlayerData();
        
        if (player_data == null)
        {
            Debug.LogError("[PlayerStats] PlayerData를 로드할 수 없습니다. 기본값 사용.");
            return;
        }

        // Player_DataTable.json에서 기본 스탯 가져오기
        base_hp = player_data.hp;
        base_move_speed = player_data.speed;
        base_atk = player_data.atk;
        base_atk_speed = player_data.atk_speed;
        base_bullet_speed = player_data.bullet_speed;
        base_range = player_data.range;

        // 초기 상태
        current_bullet_count = player_data.add_bullet;
        has_pierce = player_data.pierce;
        has_bleed = player_data.bleed;

        Debug.Log($"[PlayerStats] 기본 스탯 로드 완료: HP={base_hp}, ATK={base_atk}, Speed={base_move_speed}");
    }

    private void InitializeStats()
    {
        current_max_hp = base_hp;
        current_move_speed = base_move_speed;
        current_atk = base_atk;
        current_atk_speed = base_atk_speed;
        current_bullet_speed = base_bullet_speed;
        current_range = base_range;
        // current_bullet_count, has_pierce, has_bleed는 LoadBaseStatsFromPlayerData에서 설정됨
    }

    // 증강 적용
    public void ApplyAbility(AbilityItem ability)
    {
        if (ability == null) return;

        applied_abilities.Add(ability);

        // 주 효과 적용
        ApplyEffect(ability.mainAbility, ability.increase);

        // 부 효과 적용
        if (ability.subAbility != "none")
        {
            ApplyEffect(ability.subAbility, ability.subIncrease);
        }

        Debug.Log($"[PlayerStats] 증강 적용: {ability.name} ({ability.mainAbility} +{ability.increase})");
    }

    private void ApplyEffect(string ability_type, float value)
    {
        switch (ability_type)
        {
            // 플레이어 스탯
            case "atk":
                current_atk += value;
                break;
            case "atkspeed":
                current_atk_speed += value;
                break;
            case "bulletspeed":
                current_bullet_speed += value;
                break;
            case "addrange":
                current_range += value;
                break;
            case "addhp":
                current_max_hp += value;
                break;
            case "addspeed":
                current_move_speed += value;
                break;
            case "addbullet":
                current_bullet_count += (int)value;
                break;
            case "pierce":
                has_pierce = true;
                break;
            case "bleed":
                has_bleed = true;
                break;

            // 펫 스탯
            case "petatk":
                pet_atk += value;
                break;
            case "petcd":
                pet_cd -= value;
                if (pet_cd < 0.5f) pet_cd = 0.5f; // 최소 쿨타임
                break;
            case "pethp":
                pet_hp_restore += value;
                break;
            case "petaddbullet":
                pet_add_bullet_chance += value;
                break;

            default:
                Debug.LogWarning($"[PlayerStats] 알 수 없는 증강 타입: {ability_type}");
                break;
        }
    }

    // 스탯 초기화 (새 게임 시작 시)
    public void ResetStats()
    {
        LoadBaseStatsFromPlayerData();
        InitializeStats();
        applied_abilities.Clear();
        
        // 펫 스탯 초기화
        pet_atk = 1f;
        pet_cd = 5f;
        pet_hp_restore = 0f;
        pet_add_bullet_chance = 0f;
        
        Debug.Log("[PlayerStats] 스탯 초기화 완료");
    }

    // 타입별 증강 개수 가져오기
    public int GetAbilityCountByType(string type)
    {
        int count = 0;
        foreach (AbilityItem ability in applied_abilities)
        {
            // number 범위로 타입 판별
            if (type == "Cyan" && ability.number >= 2000 && ability.number < 3000)
            {
                count++;
            }
            else if (type == "Magenta" && ability.number >= 1000 && ability.number < 2000)
            {
                count++;
            }
            else if (type == "Yellow" && ability.number >= 3000 && ability.number < 4000)
            {
                count++;
            }
        }
        return count;
    }

    // 가장 많은 증강 타입 가져오기 (우선순위: Cyan > Magenta > Yellow)
    public string GetDominantAbilityType()
    {
        int cyan_count = GetAbilityCountByType("Cyan");
        int magenta_count = GetAbilityCountByType("Magenta");
        int yellow_count = GetAbilityCountByType("Yellow");

        // 최대값 찾기
        int max_count = Mathf.Max(cyan_count, magenta_count, yellow_count);

        // 동일하면 우선순위: Cyan > Magenta > Yellow
        if (cyan_count == max_count) return "Cyan";
        if (magenta_count == max_count) return "Magenta";
        if (yellow_count == max_count) return "Yellow";

        return "Cyan"; // 기본값
    }

    public List<AbilityItem> GetAppliedAbilities() => applied_abilities;
}