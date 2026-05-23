using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapSpawnDataLoader : MonoBehaviour
{
    private static MapSpawnDataLoader instance;
    private MapSpawnTable[] spawn_tables;

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

    // 색깔 + 난이도로 스폰 테이블 가져오기 (기존 메서드)
    public MapSpawnTable GetSpawnTable(string color, string difficulty)
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

        // 여러 테이블이 있으면 랜덤 선택
        return tables[Random.Range(0, tables.Length)];
    }

    // Easy 난이도 랜덤 스폰 테이블 가져오기 (기존 메서드)
    public MapSpawnTable GetRandomEasySpawn(string color)
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

    // 난이도별 랜덤 맵 가져오기 (GameManager용)
    public MapSpawnTable GetRandomMapByDifficulty(string difficulty)
    {
        if (spawn_tables == null)
        {
            Debug.LogWarning("MapSpawnDataLoader: 데이터가 아직 로드되지 않았습니다.");
            return null;
        }

        // 해당 난이도의 Common 맵만 필터링 (Boss 제외)
        var maps = spawn_tables.Where(t => t.difficulty == difficulty && t.room == "Common").ToArray();

        if (maps.Length == 0)
        {
            Debug.LogWarning($"난이도 {difficulty}의 Common 맵이 없습니다.");
            return null;
        }

        // 랜덤 선택
        MapSpawnTable selected = maps[Random.Range(0, maps.Length)];

        Debug.Log($"[MapSpawnDataLoader] 맵 선택: Code {selected.code} ({selected.difficulty}, {selected.color})");

        return selected;
    }

    // 보스 맵 가져오기
    public MapSpawnTable GetBossMap()
    {
        if (spawn_tables == null)
        {
            Debug.LogWarning("MapSpawnDataLoader: 데이터가 아직 로드되지 않았습니다.");
            return null;
        }

        var bossMaps = spawn_tables.Where(t => t.difficulty == "Boss" && t.room == "Boss").ToArray();

        if (bossMaps.Length == 0)
        {
            Debug.LogWarning("보스 맵이 없습니다.");
            return null;
        }

        return bossMaps[0]; // 보스 맵은 1개만 있다고 가정
    }
}

// JSON 파싱용 Wrapper 클래스
[System.Serializable]
public class MapSpawnDataWrapper
{
    public MapSpawnTable[] spawn_tables;
}

// 스폰 테이블 (맵 데이터)
[System.Serializable]
public class MapSpawnTable
{
    public int code;            // 맵 코드
    public string color;        // Cyan, Magenta, Yellow
    public string difficulty;   // Easy, Normal, Hard, Boss
    public string room;         // Common, Boss
    public EnemySpawnInfo[] spawns;  // 스폰할 적 리스트
}

// 개별 적 스폰 데이터
[System.Serializable]
public class EnemySpawnInfo
{
    public int enemy_id;    // 적 ID (1001~1006, 9001)
    public int min_count;   // 최소 스폰 개수
    public int max_count;   // 최대 스폰 개수
}