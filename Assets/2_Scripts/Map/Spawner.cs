using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField] private string map_color = "Cyan"; // "Cyan", "Magenta", "Yellow"
    [SerializeField] private Transform spawn_area_center;
    [SerializeField] private Vector2 spawn_area_size = new Vector2(20f, 10f);
    
    [Header("적 프리팹")]
    [SerializeField] private GameObject ranged_enemy_prefab;
    [SerializeField] private GameObject turret_enemy_prefab;
    [SerializeField] private GameObject bomb_enemy_prefab;
    [SerializeField] private GameObject boss_prefab; // 보스 프리팹

    private List<Enemy> spawned_enemies = new List<Enemy>();
    private bool has_spawned = false;

    public void SpawnEnemies()
    {
        if (has_spawned)
        {
            Debug.LogWarning("Spawner: 이미 적이 스폰되었습니다.");
            return;
        }

        // 첫 스테이지는 무조건 Easy 중 1개 랜덤
        SpawnTable table = MapSpawnDataLoader.Instance.GetRandomEasySpawn(map_color);

        if (table == null)
        {
            Debug.LogError($"Spawner: Color {map_color} Easy 스폰 테이블을 찾을 수 없습니다.");
            return;
        }

        SpawnFromTable(table);
        has_spawned = true;
    }

    public void SpawnEnemiesWithDifficulty(string difficulty)
    {
        ClearAllEnemies();

        SpawnTable table = MapSpawnDataLoader.Instance.GetSpawnTable(map_color, difficulty);

        if (table == null)
        {
            Debug.LogError($"Spawner: Color {map_color} {difficulty} 스폰 테이블을 찾을 수 없습니다.");
            return;
        }

        SpawnFromTable(table);
        has_spawned = true;
    }

    private void SpawnFromTable(SpawnTable table)
    {
        foreach (SpawnInfo spawn_info in table.spawns)
        {
            // min_count와 max_count 사이에서 랜덤하게 스폰
            int count = Random.Range(spawn_info.min_count, spawn_info.max_count + 1);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy(spawn_info.enemy_id);
            }
        }

        Debug.Log($"적 스폰 완료: {table.color} {table.difficulty}, 총 {spawned_enemies.Count}마리");
    }

    private void SpawnEnemy(int enemy_id)
    {
        GameObject prefab = GetEnemyPrefab(enemy_id);
        
        if (prefab == null)
        {
            Debug.LogError($"Spawner: Enemy ID {enemy_id}에 해당하는 프리팹이 없습니다.");
            return;
        }

        Vector3 spawn_position = GetRandomSpawnPosition();
        GameObject enemy_obj = Instantiate(prefab, spawn_position, Quaternion.identity);

        Enemy enemy = enemy_obj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(enemy_id);
            spawned_enemies.Add(enemy);
        }
    }

    private GameObject GetEnemyPrefab(int enemy_id)
    {
        switch (enemy_id)
        {
            case 1001: return ranged_enemy_prefab;
            case 1002: return turret_enemy_prefab;
            case 1003: return bomb_enemy_prefab;
            case 9001: return boss_prefab; // Cyan Boss
            case 9002: return boss_prefab; // Magenta Boss (차후)
            case 9003: return boss_prefab; // Yellow Boss (차후)
            default: return null;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = spawn_area_center != null ? spawn_area_center.position : transform.position;
        
        float x = Random.Range(-spawn_area_size.x / 2f, spawn_area_size.x / 2f);
        float y = Random.Range(-spawn_area_size.y / 2f, spawn_area_size.y / 2f);

        return center + new Vector3(x, y, 0f);
    }

    public void ClearAllEnemies()
    {
        foreach (Enemy enemy in spawned_enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        spawned_enemies.Clear();
        has_spawned = false;
    }

    public bool AreAllEnemiesDead()
    {
        spawned_enemies.RemoveAll(e => e == null || !e.IsAlive());
        return spawned_enemies.Count == 0;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = spawn_area_center != null ? spawn_area_center.position : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(spawn_area_size.x, spawn_area_size.y, 0f));
    }
}