using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("Enemy Prefabs")]
    [SerializeField]
    private GameObject[] enemyPrefabs;

    private Dictionary<int, GameObject> prefabDict =
        new Dictionary<int, GameObject>();

    private List<SpawnTableData> spawnTables;

    private void Awake()
    {
        Instance = this;

        LoadEnemyPrefabs();
        LoadSpawnTable();
    }

    private void LoadEnemyPrefabs()
    {
        foreach (GameObject prefab in enemyPrefabs)
        {
            Enemy enemy = prefab.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            prefabDict[enemy.mobID] = prefab;
        }
    }

    private void LoadSpawnTable()
    {
        TextAsset json =
            Resources.Load<TextAsset>("MapMonsterSpawn_DataTable");

        SpawnTableWrapper wrapper =
            JsonUtility.FromJson<SpawnTableWrapper>(json.text);

        spawnTables = wrapper.spawnTable;
    }

    public SpawnTableData GetSpawnTable(int code)
    {
        return spawnTables.Find(x => x.code == code);
    }

    public GameObject GetEnemyPrefab(int monsterId)
    {
        if (prefabDict.ContainsKey(monsterId))
            return prefabDict[monsterId];

        return null;
    }
}