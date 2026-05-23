using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public int enemy_id;
    public int min_count;
    public int max_count;
}

[System.Serializable]
public class SpawnTable
{
    public int code;
    public string color; // "Cyan", "Magenta", "Yellow"
    public string difficulty; // "Easy", "Normal", "Hard", "Boss"
    public string room; // "Common", "Boss"
    public SpawnInfo[] spawns;
}

[System.Serializable]
public class MapSpawnDataWrapper
{
    public SpawnTable[] spawn_tables;
}