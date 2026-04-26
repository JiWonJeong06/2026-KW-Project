using UnityEngine;

public class AbilityDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;
    void Awake()
    {
        AbilityData data = JsonUtility.FromJson<AbilityData>(jsonFile.text);
    }
}
