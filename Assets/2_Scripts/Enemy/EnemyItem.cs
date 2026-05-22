using UnityEngine;

[System.Serializable]
public class EnemyItem
{
    public int id;
    public string name;
    public string type;
    public float atk;
    public float atk_speed;
    public float bullet_speed;
    public float hp;
    public float move_speed;
    public float range;
}

[System.Serializable]
public class EnemyDataWrapper
{
    public EnemyItem[] items;
}