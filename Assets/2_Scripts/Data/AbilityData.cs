/// <summary>
/// 단일 증강 데이터 클래스
/// CSV에서 변환된 JSON의 각 항목에 대응
/// 
/// 주의: increase, subIncrease는 float (1.5 같은 소수 가능)
/// </summary>
[System.Serializable]
public class AbilityData
{
    public int number;                  // 고유 ID (1001~3203)
    public string type;                 // Magenta, Cyan, Yellow
    public string name;                 // 증강 이름
    public string rank;                 // C, B, A
    public string mainAbility;          // 주 능력 (atk, atkspeed, pierce, petatk 등)
    public float increase;              // 주 능력 증가값 (float - 소수 가능)
    public string subAbility;           // 보조 능력 ("none"이면 없음)
    public float subIncrease;           // 보조 능력 증가값 (float - 소수 가능)
    public string explanation;          // 설명
    public string text;                 // 추가 텍스트

    // ===== 편의 메서드 =====
    
    /// <summary>
    /// 보조 능력이 있는지 확인
    /// </summary>
    public bool HasSubAbility()
    {
        return !string.IsNullOrEmpty(subAbility) && subAbility != "none";
    }

    /// <summary>
    /// 증강 정보를 문자열로 반환 (디버그용)
    /// </summary>
    public override string ToString()
    {
        string result = $"{name} ({rank}) - {mainAbility} +{increase}";
        if (HasSubAbility())
            result += $", {subAbility} +{subIncrease}";
        return result;
    }
}