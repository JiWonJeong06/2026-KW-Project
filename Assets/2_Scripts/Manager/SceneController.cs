using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 씬 전환 관리 — 타이틀 → 로딩 → 인게임
/// </summary>
public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    public static SceneController Instance => instance;

    // 씬 이름 상수
    public const string SCENE_TITLE   = "0_Start";
    public const string SCENE_LOADING = "99_Loading";
    public const string SCENE_INGAME  = "2_InGame";
    public const string SCENE_INGAME2 = "3_InGame";

    // 로딩 씬에서 읽어갈 다음 씬 이름
    public static string next_scene { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>타이틀에서 인게임으로</summary>
    public void LoadInGame()
    {
        LoadWithLoading(SCENE_INGAME);
    }

    /// <summary>로딩 씬을 거쳐서 target 씬으로</summary>
    public void LoadWithLoading(string target_scene)
    {
        next_scene = target_scene;
        SceneManager.LoadScene(SCENE_LOADING);
    }

    /// <summary>다음 챕터 (3_InGame)로</summary>
    public void LoadNextChapter()
    {
        LoadWithLoading(SCENE_INGAME2);
    }

    /// <summary>타이틀로 바로 이동</summary>
    public void LoadTitle()
    {
        SceneManager.LoadScene(SCENE_TITLE);
    }
}