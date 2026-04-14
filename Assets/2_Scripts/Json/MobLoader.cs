using UnityEngine;

public class MobLoader : MonoBehaviour
{
    public TextAsset mobJsonFile;
    private MobData[] mobList;

    void Awake()
    {
        mobList = JsonManager.LoadArray<MobData>(mobJsonFile);

        if (mobList == null) return;

        for (int i = 0; i < mobList.Length; i++)
        {
            Debug.Log($"Mob {mobList[i].Name} / HP: {mobList[i].hp}");
        }
    }
}