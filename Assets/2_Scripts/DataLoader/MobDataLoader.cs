using System.Collections.Generic;
using UnityEngine;

public class MobDataLoader : MonoBehaviour
{
    public static MobDataLoader Instance;

    [Header("Mob Data")]
    [SerializeField] private TextAsset mobJsonFile;

    private Dictionary<int, MobData> mobDataTable = new Dictionary<int, MobData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadMobData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadMobData()
    {
        if (mobJsonFile == null)
        {
            Debug.LogWarning("Mob Json File이 비어 있음");
            return;
        }

        MobData[] mobList = JsonManager.LoadArray<MobData>(mobJsonFile);

        if (mobList == null)
        {
            Debug.LogWarning("MobData 로드 실패");
            return;
        }

        mobDataTable.Clear();

        for (int i = 0; i < mobList.Length; i++)
        {
            MobData data = mobList[i];

            if (mobDataTable.ContainsKey(data.ID))
            {
                Debug.LogWarning($"중복된 Mob ID 발견: {data.ID}");
                continue;
            }

            mobDataTable.Add(data.ID, data);
        }

        Debug.Log($"MobData 로드 완료: {mobDataTable.Count}개");
    }

    public MobData GetMobData(int id)
    {
        if (mobDataTable.TryGetValue(id, out MobData data))
        {
            return data;
        }

        Debug.LogWarning($"해당 ID의 MobData가 없음: {id}");
        return null;
    }
}