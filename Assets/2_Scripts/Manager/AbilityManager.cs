using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 증강 능력을 Player에 적용하는 클래스
/// 
/// 역할:
/// 1. 14가지 능력을 Dictionary로 관리
/// 2. mainAbility에 따라 자동으로 적용
/// 3. subAbility가 있으면 함께 적용
/// 4. 색깔별 카운트 업데이트
/// 5. 새로운 능력 추가 시 Dictionary만 수정하면 됨
/// 
/// 주의: increase/subIncrease는 float (1.5 같은 소수 가능)
/// </summary>
public class AbilityManager : MonoBehaviour
{
    private Dictionary<string, System.Action<Player, float>> abilityActions;
    private Player player;
    private AbilityDataLoader abilityDataLoader;  // NEW: 선택 기록용

    void Awake()
    {
        player = FindFirstObjectByType<Player>();
        abilityDataLoader = FindFirstObjectByType<AbilityDataLoader>();  // NEW
        
        if (player == null)
        {
            Debug.LogError("[AbilityManager] Player를 찾을 수 없음");
            return;
        }

        if (abilityDataLoader == null)
        {
            Debug.LogError("[AbilityManager] AbilityDataLoader를 찾을 수 없음");
            return;
        }

        InitializeAbilities();
    }

    /// <summary>
    /// 능력 Dictionary 초기화
    /// 각 능력명에 대해 해당하는 Action 정의
    /// 
    /// 14가지 능력:
    /// - Magenta: atk, atkspeed, bulletspeed, addrange, pierce, bleed, addbullet (7개)
    /// - Cyan: addhp, addspeed (2개)
    /// - Yellow: petatk, petcd, pethp, petaddbullet (4개)
    /// 
    /// 확장성 높음: 새로운 능력은 여기에만 추가하면 됨
    /// </summary>
    private void InitializeAbilities()
    {
        abilityActions = new Dictionary<string, System.Action<Player, float>>
        {
            // ===== Magenta 능력 (공격 계열) =====
            {
                "atk", (p, v) =>
                {
                    p.atk += v;
                    Debug.Log($"[Ability] 공격력 +{v} (현재: {p.atk})");
                }
            },
            {
                "atkspeed", (p, v) =>
                {
                    p.atkspeed += v;
                    Debug.Log($"[Ability] 공격속도 +{v} (현재: {p.atkspeed})");
                }
            },
            {
                "bulletspeed", (p, v) =>
                {
                    p.bulletspeed += v;
                    Debug.Log($"[Ability] 총알속도 +{v} (현재: {p.bulletspeed})");
                }
            },
            {
                "addrange", (p, v) =>
                {
                    p.addrange += v;
                    Debug.Log($"[Ability] 사거리 +{v} (현재: {p.addrange})");
                }
            },
            {
                "pierce", (p, v) =>
                {
                    p.pierceLevel += (int)v;
                    Debug.Log($"[Ability] 관통 레벨 +{(int)v} (현재: {p.pierceLevel})");
                }
            },
            {
                "bleed", (p, v) =>
                {
                    p.bleedLevel += (int)v;
                    Debug.Log($"[Ability] 출혈 레벨 +{(int)v} (현재: {p.bleedLevel})");
                }
            },
            {
                "addbullet", (p, v) =>
                {
                    p.addbullet += v;
                    Debug.Log($"[Ability] 총알 개수 +{v} (현재: {p.addbullet})");
                }
            },

            // ===== Cyan 능력 (방어/이동 계열) =====
            {
                "addhp", (p, v) =>
                {
                    p.hp += v;
                    p.currenthp += v;  // 현재 체력도 함께 증가
                    Debug.Log($"[Ability] 체력 +{v} (현재: {p.hp})");
                }
            },
            {
                "addspeed", (p, v) =>
                {
                    p.speed += v;
                    Debug.Log($"[Ability] 이동속도 +{v} (현재: {p.speed})");
                }
            },

            // ===== Yellow 능력 (펫 계열) =====
            {
                "petatk", (p, v) =>
                {
                    p.petAtk += v;
                    Debug.Log($"[Ability] 펫 공격력 +{v} (현재: {p.petAtk})");
                }
            },
            {
                "petcd", (p, v) =>
                {
                    p.petCooldown = Mathf.Max(0, p.petCooldown - v);
                    Debug.Log($"[Ability] 펫 쿨타임 -{v} (현재: {p.petCooldown})");
                }
            },
            {
                "pethp", (p, v) =>
                {
                    p.petHp += v;
                    Debug.Log($"[Ability] 펫 체력 +{v} (현재: {p.petHp})");
                }
            },
            {
                "petaddbullet", (p, v) =>
                {
                    p.petAddbullet += v;
                    Debug.Log($"[Ability] 펫 추가 총알 +{v} (현재: {p.petAddbullet})");
                }
            },
        };

        Debug.Log($"[AbilityManager] 능력 초기화 완료: {abilityActions.Count}개");
    }

    /// <summary>
    /// 선택된 증강을 Player에 적용
    /// 
    /// 과정:
    /// 1. mainAbility 적용
    /// 2. subAbility 적용 (있으면 + 중복 체크)
    /// 3. 색깔별 카운트 업데이트
    /// 4. 무기 변경 시스템에 신호
    /// </summary>
    public void ApplyAbility(AbilityData ability, Player targetPlayer = null)
    {
        if (targetPlayer == null)
            targetPlayer = player;

        if (targetPlayer == null)
        {
            Debug.LogError("[AbilityManager] 적용할 Player가 없음");
            return;
        }

        // 1. 주 능력 적용
        if (abilityActions.ContainsKey(ability.mainAbility))
        {
            abilityActions[ability.mainAbility](targetPlayer, ability.increase);
        }
        else
        {
            Debug.LogWarning($"[AbilityManager] 미지원 능력: {ability.mainAbility}");
            return;
        }

        // 2. 보조 능력 적용 (있으면 + 중복 체크)
        if (ability.HasSubAbility())
        {
            // ===== 중복 체크 =====
            if (ability.mainAbility == ability.subAbility)
            {
                Debug.LogWarning($"[AbilityManager] 중복 능력 감지: {ability.mainAbility}와 {ability.subAbility}가 같음! (증강: {ability.name})");
                Debug.Log($"[AbilityManager] → 주 능력만 적용됨 (subAbility는 무시)");
            }
            else
            {
                // 중복 아님 - 정상 적용
                if (abilityActions.ContainsKey(ability.subAbility))
                {
                    abilityActions[ability.subAbility](targetPlayer, ability.subIncrease);
                    Debug.Log($"[AbilityManager] 보조 능력 적용: {ability.subAbility} +{ability.subIncrease}");
                }
                else
                {
                    Debug.LogWarning($"[AbilityManager] 미지원 보조 능력: {ability.subAbility}");
                }
            }
        }

        // 3. 색깔별 카운트 업데이트
        targetPlayer.AddAbilityColor(ability.type);

        // 4. ===== NEW: 선택 이력 기록 =====
        if (abilityDataLoader != null)
        {
            abilityDataLoader.RecordSelectedAbility(ability.number);
        }

        // 5. 무기 변경 신호
        OnAbilityApplied?.Invoke(ability, targetPlayer);

        Debug.Log($"[AbilityManager] 증강 적용 완료: {ability.name} ({ability.mainAbility} +{ability.increase}" +
                  (ability.HasSubAbility() && ability.mainAbility != ability.subAbility ? $", {ability.subAbility} +{ability.subIncrease}" : "") + ")");
    }

    /// <summary>
    /// 능력 적용 후 호출되는 이벤트
    /// WeaponUpgradeSystem에서 구독
    /// </summary>
    public delegate void OnAbilityAppliedDelegate(AbilityData ability, Player player);
    public event OnAbilityAppliedDelegate OnAbilityApplied;

    // ===== 유틸리티 메서드 =====

    /// <summary>
    /// 능력이 지원되는지 확인
    /// </summary>
    public bool IsAbilitySupported(string abilityName)
    {
        return abilityActions.ContainsKey(abilityName);
    }

    /// <summary>
    /// 지원하는 모든 능력 목록 반환
    /// </summary>
    public string[] GetSupportedAbilities()
    {
        string[] abilities = new string[abilityActions.Count];
        abilityActions.Keys.CopyTo(abilities, 0);
        return abilities;
    }

    /// <summary>
    /// 특정 능력에 대한 설명
    /// </summary>
    public string GetAbilityDescription(string abilityName)
    {
        return abilityName switch
        {
            "atk" => "공격력 증가",
            "atkspeed" => "공격 속도 증가",
            "bulletspeed" => "총알 속도 증가",
            "addrange" => "사거리 증가",
            "pierce" => "관통 효과 레벨 증가",
            "bleed" => "출혈 효과 레벨 증가",
            "addbullet" => "총알 개수 증가",
            "addhp" => "최대 체력 증가",
            "addspeed" => "이동 속도 증가",
            "petatk" => "펫 공격력 증가",
            "petcd" => "펫 공격 쿨타임 감소",
            "pethp" => "펫 체력 증가",
            "petaddbullet" => "펫 추가 총알 증가",
            _ => "알 수 없는 능력"
        };
    }
}