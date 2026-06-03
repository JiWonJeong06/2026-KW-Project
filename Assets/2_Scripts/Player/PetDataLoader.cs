using UnityEngine;

public class PetDataLoader : MonoBehaviour
{
    private static PetDataLoader instance;
    public static PetDataLoader Instance => instance;

    [SerializeField] private TextAsset pet_data_json;

    private PetData pet_data;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    private void LoadData()
    {
        if (pet_data_json == null)
        {
            Debug.LogError("[PetDataLoader] JSON 미할당!");
            return;
        }

        pet_data = JsonUtility.FromJson<PetData>(pet_data_json.text);
        Debug.Log($"[PetDataLoader] 로드 완료: {pet_data.korean_name}");
    }

    public PetData GetPetData() => pet_data;
}