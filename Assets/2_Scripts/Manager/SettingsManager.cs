using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SettingsManager Instance;

    private void Awake()
    {
        // 이미 존재하면 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 현재 객체를 Instance로 지정
        Instance = this;

        // 씬이 바뀌어도 유지
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}