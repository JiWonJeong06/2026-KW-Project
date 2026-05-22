using UnityEngine;

public class PlayerDataLoader : MonoBehaviour
{
    private static PlayerDataLoader instance;
    private PlayerData playerData;

    [SerializeField] private TextAsset player_data_json;

    public static PlayerDataLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PlayerDataLoader>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("PlayerDataLoader");
                    instance = obj.AddComponent<PlayerDataLoader>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        if (player_data_json == null)
        {
            Debug.LogError("PlayerDataLoader: JSON 파일이 인스펙터에 지정되지 않았습니다.");
            return;
        }

        playerData = JsonUtility.FromJson<PlayerData>(player_data_json.text);

        if (playerData == null)
        {
            Debug.LogError("PlayerDataLoader: JSON 파싱에 실패했습니다.");
            return;
        }

        Debug.Log($"플레이어 데이터 로드 완료: {playerData.name}");
    }

    public PlayerData GetPlayerData()
    {
        if (playerData == null)
            Debug.LogWarning("PlayerDataLoader: 데이터가 아직 로드되지 않았습니다.");

        return playerData;
    }
}