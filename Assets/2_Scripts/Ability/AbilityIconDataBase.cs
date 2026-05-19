using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 증강 아이콘을 관리하는 ScriptableObject
/// 
/// 역할:
/// 1. 308개 증강의 아이콘을 저장
/// 2. Number → Sprite 매핑
/// 3. 캐싱으로 성능 최적화
/// 4. 에디터에서 자동화 지원
/// </summary>
[CreateAssetMenu(fileName = "AbilityIconDatabase", menuName = "Ability/Icon Database")]
public class AbilityIconDatabase : ScriptableObject
{
    /// <summary>
    /// 아이콘 데이터 구조
    /// </summary>
    [System.Serializable]
    public class IconData
    {
        public int number;      // 증강 number (1001~3203)
        public Sprite sprite;     // 해당 스프라이트
    }

    [SerializeField] private List<IconData> icons = new List<IconData>();
    
    // 캐시 (빠른 검색용)
    private Dictionary<int, Sprite> cache;

    /// <summary>
    /// 캐시 초기화 (접근할 때마다 실행)
    /// </summary>
    private void InitializeCache()
    {
        if (cache == null || cache.Count == 0)
        {
            cache = new Dictionary<int, Sprite>();

            foreach (IconData iconData in icons)
            {
                if (!cache.ContainsKey(iconData.number))
                {
                    cache[iconData.number] = iconData.sprite;
                }
                else
                {
                    Debug.LogWarning($"[AbilityIconDatabase] 중복된 아이콘 number: {iconData.number}");
                }
            }

            Debug.Log($"[AbilityIconDatabase] 캐시 초기화 완료: {cache.Count}개");
        }
    }

    /// <summary>
    /// 특정 number의 아이콘 반환
    /// </summary>
    public Sprite GetIcon(int abilityNumber)
    {
        InitializeCache();

        if (cache.TryGetValue(abilityNumber, out Sprite icon))
        {
            return icon;
        }

        Debug.LogWarning($"[AbilityIconDatabase] 아이콘을 찾을 수 없음: {abilityNumber}");
        return null;
    }

    /// <summary>
    /// 모든 아이콘 데이터 반환
    /// </summary>
    public List<IconData> GetAllIcons()
    {
        return new List<IconData>(icons);
    }

    /// <summary>
    /// 아이콘 데이터 추가 (에디터에서 사용)
    /// </summary>
    public void AddIcon(int number, Sprite sprite)
    {
        // 중복 확인
        foreach (IconData iconData in icons)
        {
            if (iconData.number == number)
            {
                Debug.LogWarning($"[AbilityIconDatabase] 이미 존재하는 number: {number}");
                return;
            }
        }

        // 추가
        icons.Add(new IconData { number = number, sprite = sprite });

        // 캐시 무효화
        cache = null;

        Debug.Log($"[AbilityIconDatabase] 아이콘 추가: {number}");
    }

    /// <summary>
    /// 아이콘 데이터 제거 (에디터에서 사용)
    /// </summary>
    public void RemoveIcon(int number)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            if (icons[i].number == number)
            {
                icons.RemoveAt(i);
                cache = null;
                Debug.Log($"[AbilityIconDatabase] 아이콘 제거: {number}");
                return;
            }
        }

        Debug.LogWarning($"[AbilityIconDatabase] 제거할 아이콘 없음: {number}");
    }

    /// <summary>
    /// 모든 아이콘 데이터 초기화
    /// </summary>
    public void ClearAllIcons()
    {
        icons.Clear();
        cache = null;
        Debug.Log("[AbilityIconDatabase] 모든 아이콘 초기화");
    }

    /// <summary>
    /// 현재 저장된 아이콘 개수 반환
    /// </summary>
    public int GetIconCount()
    {
        return icons.Count;
    }

    /// <summary>
    /// 특정 number의 아이콘이 있는지 확인
    /// </summary>
    public bool HasIcon(int abilityNumber)
    {
        InitializeCache();
        return cache.ContainsKey(abilityNumber);
    }
}