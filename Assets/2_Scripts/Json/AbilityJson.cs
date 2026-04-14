using System;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public int Number;
    public string Type;
    public string Name;
    public string Rank;
    public string MainAblility;   // JSON 키와 동일하게 유지
    public string SubAbility;
    public float Increase;
    public string Explanation;
    public string Text;
}