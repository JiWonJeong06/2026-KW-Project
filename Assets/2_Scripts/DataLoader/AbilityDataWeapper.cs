using System.Collections.Generic;

/// <summary>
/// JSON 배열을 파싱하기 위한 래퍼 클래스
/// JsonUtility.FromJson은 배열을 직접 파싱할 수 없으므로
/// 배열을 감싸는 클래스가 필요합니다.
/// </summary>
[System.Serializable]
public class AbilityDataWrapper
{
    public AbilityData[] abilities;

    // 편의 메서드: 배열을 List로 변환
    public List<AbilityData> ToList()
    {
        return new List<AbilityData>(abilities);
    }
}