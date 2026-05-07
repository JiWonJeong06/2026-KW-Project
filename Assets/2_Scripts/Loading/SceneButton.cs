using UnityEngine;

public class SceneButton : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName;

    public void LoadScene()
    {
        if (LoadManager.Instance == null)
        {
            Debug.LogError("LoadManager가 존재하지 않습니다.");
            return;
        }

        LoadManager.Instance.LoadScene(targetSceneName);
    }
}