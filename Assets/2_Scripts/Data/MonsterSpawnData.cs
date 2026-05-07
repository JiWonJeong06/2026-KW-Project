using System;
using System.Collections.Generic;

[Serializable]
public class MonsterSpawnData
{
    public int monsterId;
    public int minSpawn;
    public int maxSpawn;
}

[Serializable]
public class SpawnTableData
{
    public int code;
    public string color;
    public string difficulty;
    public string room;

    public List<MonsterSpawnData> monsters;
}

[Serializable]
public class SpawnTableWrapper
{
    public List<SpawnTableData> spawnTable;
}