using UnityEngine;
using System;

/// <summary>
/// MyckaData를 JSON에서 로드
/// </summary>
public class MyckaDataLoader : MonoBehaviour
{
    public static MyckaDataLoader Instance { get; private set; }

    [SerializeField]
    private TextAsset jsonFile;

    private MyckaData loadedData;
    private bool isLoaded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (jsonFile != null)
        {
            LoadData();
        }
        else
        {
            Debug.LogError("[MyckaDataLoader] JSON 파일이 할당되지 않았습니다");
        }
    }

    public void LoadData()
    {
        if (isLoaded)
        {
            Debug.LogWarning("[MyckaDataLoader] 이미 로드됨");
            return;
        }

        if (jsonFile == null)
        {
            Debug.LogError("[MyckaDataLoader] JSON 파일이 없습니다");
            return;
        }

        try
        {
            // JSON 파싱
            loadedData = JsonUtility.FromJson<MyckaData>(jsonFile.text);

            if (loadedData == null)
            {
                Debug.LogError("[MyckaDataLoader] JSON 파싱 실패");
                return;
            }

            isLoaded = true;

            Debug.Log($"[MyckaDataLoader] 데이터 로드 완료: {loadedData.name}");

            // Player에 적용
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                player.ApplyMyckaData(loadedData);
                Debug.Log("[MyckaDataLoader] Player에 데이터 적용 완료");
            }
            else
            {
                Debug.LogWarning("[MyckaDataLoader] Player를 찾을 수 없습니다");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[MyckaDataLoader] 오류: {e.Message}");
            isLoaded = false;
        }
    }

    public MyckaData GetLoadedData() => loadedData;
    public bool IsLoaded() => isLoaded;
}