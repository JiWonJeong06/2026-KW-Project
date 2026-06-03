using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 씬 UI
/// </summary>
public class TitleUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button start_button;   // 게임 시작
    [SerializeField] private Button quit_button;    // 게임 종료

    private void Start()
    {
        // 타이틀 BGM
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM_Title();

        if (start_button != null) start_button.onClick.AddListener(OnStartButton);
        if (quit_button  != null) quit_button.onClick.AddListener(OnQuitButton);
    }

    private void OnStartButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUISelect();

        // 로딩 씬을 거쳐 InGame으로
        SceneController.Instance.LoadInGame();
    }

    private void OnQuitButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUISelect();

        Application.Quit();
        Debug.Log("[TitleUI] 게임 종료");
    }

    // 버튼 호버 시 (EventSystem의 OnPointerEnter에서 호출)
    public void OnButtonHover()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUIHover();
    }
}