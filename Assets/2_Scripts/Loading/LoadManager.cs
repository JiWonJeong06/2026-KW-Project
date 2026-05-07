using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance;

    [Header("Loading Scene")]
    [SerializeField] private string loadingSceneName = "99_Loading";

    // 이동할 목표 씬
    private string targetSceneName;

    // 현재 로딩 중 여부
    private bool isLoading = false;

    // 로딩 진행도
    public float Progress { get; private set; }

    void Awake()
    {
        // 싱글톤 생성
        if (Instance == null)
        {
            Instance = this;

            // 씬이 바뀌어도 유지
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 외부에서 호출
    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        targetSceneName = sceneName;

        SceneManager.LoadScene(loadingSceneName);
    }

    // LoadingScene에서 호출
    public void StartLoading()
    {
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        isLoading = true;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(targetSceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            Progress = Mathf.Clamp01(operation.progress / 0.9f);

            // 로딩 완료
            if (operation.progress >= 0.9f)
            {
                Progress = 1f;

                yield return new WaitForSeconds(0.3f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        isLoading = false;
    }
}