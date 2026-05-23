using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner; // Spawner 참조
    
    private string difficulty = "Easy";
    private MapSpawnTable selected_map_data = null;

    // GameManager에서 호출
    public void SetDifficulty(string diff)
    {
        difficulty = diff;
        LoadRandomMap();
    }

    private void LoadRandomMap()
    {
        // MapSpawnDataLoader에서 난이도에 맞는 맵 랜덤 선택
        selected_map_data = MapSpawnDataLoader.Instance.GetRandomMapByDifficulty(difficulty);

        if (selected_map_data == null)
        {
            Debug.LogError($"[MapManager] {difficulty} 난이도의 맵을 찾을 수 없습니다!");
            return;
        }

        Debug.Log($"[MapManager] 맵 선택: Code {selected_map_data.code} ({difficulty}, {selected_map_data.color})");

        // Spawner에 스폰 데이터 전달
        if (spawner != null)
        {
            spawner.SpawnFromMapData(selected_map_data);
            Debug.Log($"[MapManager] Spawner에 데이터 전달 완료");
        }
        else
        {
            Debug.LogWarning("[MapManager] Spawner가 할당되지 않았습니다!");
        }
    }

    public MapSpawnTable GetMapData() => selected_map_data;
    public string GetDifficulty() => difficulty;
}