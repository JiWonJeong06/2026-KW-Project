using UnityEngine;

public class AbilityLoader : MonoBehaviour
{
    public TextAsset abilityJsonFile;
    private AbilityData[] abilityList;

    void Awake()
    {
        abilityList = JsonManager.LoadArray<AbilityData>(abilityJsonFile);

        if (abilityList == null) return;

        for (int i = 0; i < abilityList.Length; i++)
        {
            Debug.Log($"Ability [{abilityList[i].Number}] {abilityList[i].Name}");
        }
    }
}