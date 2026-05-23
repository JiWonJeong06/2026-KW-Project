using UnityEngine;
using System.Collections.Generic;

public class EnemyDataLoader : MonoBehaviour
{
    private static EnemyDataLoader instance;
    private Dictionary<int, EnemyItem> enemy_data_dict;

    [SerializeField] private TextAsset enemy_data_json;

    public static EnemyDataLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<EnemyDataLoader>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemyDataLoader");
                    instance = obj.AddComponent<EnemyDataLoader>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadEnemyData();
    }

    private void LoadEnemyData()
    {
        if (enemy_data_json == null)
        {
            Debug.LogError("EnemyDataLoader: JSON 파일이 인스펙터에 지정되지 않았습니다.");
            return;
        }

        EnemyDataWrapper wrapper = JsonUtility.FromJson<EnemyDataWrapper>(enemy_data_json.text);

        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("EnemyDataLoader: JSON 파싱에 실패했습니다.");
            return;
        }

        enemy_data_dict = new Dictionary<int, EnemyItem>();
        foreach (EnemyItem item in wrapper.items)
        {
            enemy_data_dict[item.id] = item;
        }

        Debug.Log($"적 데이터 로드 완료: {enemy_data_dict.Count}개");
    }

    public EnemyItem GetEnemyData(int id)
    {
        if (enemy_data_dict == null)
        {
            Debug.LogWarning("EnemyDataLoader: 데이터가 아직 로드되지 않았습니다.");
            return null;
        }

        if (enemy_data_dict.TryGetValue(id, out EnemyItem data))
        {
            return data;
        }

        Debug.LogWarning($"EnemyDataLoader: ID {id}에 해당하는 적 데이터가 없습니다.");
        return null;
    }
}