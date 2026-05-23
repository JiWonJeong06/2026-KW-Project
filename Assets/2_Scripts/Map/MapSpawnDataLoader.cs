using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapSpawnDataLoader : MonoBehaviour
{
    private static MapSpawnDataLoader instance;
    private SpawnTable[] spawn_tables;

    [SerializeField] private TextAsset map_spawn_json;

    public static MapSpawnDataLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<MapSpawnDataLoader>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("MapSpawnDataLoader");
                    instance = obj.AddComponent<MapSpawnDataLoader>();
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

        LoadMapSpawnData();
    }

    private void LoadMapSpawnData()
    {
        if (map_spawn_json == null)
        {
            Debug.LogError("MapSpawnDataLoader: JSON 파일이 인스펙터에 지정되지 않았습니다.");
            return;
        }

        MapSpawnDataWrapper wrapper = JsonUtility.FromJson<MapSpawnDataWrapper>(map_spawn_json.text);

        if (wrapper == null || wrapper.spawn_tables == null)
        {
            Debug.LogError("MapSpawnDataLoader: JSON 파싱에 실패했습니다.");
            return;
        }

        spawn_tables = wrapper.spawn_tables;
        Debug.Log($"맵 스폰 데이터 로드 완료: {spawn_tables.Length}개");
    }

    public SpawnTable GetSpawnTable(string color, string difficulty)
    {
        if (spawn_tables == null)
        {
            Debug.LogWarning("MapSpawnDataLoader: 데이터가 아직 로드되지 않았습니다.");
            return null;
        }

        var tables = spawn_tables.Where(t => t.color == color && t.difficulty == difficulty).ToArray();
        
        if (tables.Length == 0)
        {
            Debug.LogWarning($"Color {color}, Difficulty {difficulty} 스폰 테이블이 없습니다.");
            return null;
        }

        // 동일한 color와 difficulty에 여러 테이블이 있으면 랜덤 선택
        return tables[Random.Range(0, tables.Length)];
    }

    public SpawnTable GetRandomEasySpawn(string color)
    {
        if (spawn_tables == null)
        {
            Debug.LogWarning("MapSpawnDataLoader: 데이터가 아직 로드되지 않았습니다.");
            return null;
        }

        var easyTables = spawn_tables.Where(t => t.color == color && t.difficulty == "Easy").ToArray();
        
        if (easyTables.Length == 0)
        {
            Debug.LogWarning($"Color {color}의 Easy 난이도 스폰 테이블이 없습니다.");
            return null;
        }

        return easyTables[Random.Range(0, easyTables.Length)];
    }
}