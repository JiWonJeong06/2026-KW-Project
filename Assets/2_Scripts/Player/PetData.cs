using UnityEngine;

[System.Serializable]
public class PetData
{
    public string name;
    public string korean_name;
    public string japanese_name;
    public float  atk;
    public float  cooldown;
    public float  hp;
    public int    additional_bullet; // 0~100 확률
    public float  bullet_speed;
}