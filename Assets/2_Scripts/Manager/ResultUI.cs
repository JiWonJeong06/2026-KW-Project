using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 결산창 UI
/// 승리 / 패배에 따라 다른 패널 표시
/// </summary>
public class ResultUI : MonoBehaviour
{
    private static ResultUI instance;
    public static ResultUI Instance => instance;

    // ─────────────────────────────────────────
    // 공통 요소
    // ─────────────────────────────────────────
    [Header("공통")]
    [SerializeField] private GameObject result_panel;       // 결산창 전체 패널
    [SerializeField] private TMP_Text   play_time_text;     // 플레이 타임 [00:00:00]
    [SerializeField] private Button     title_button;       // 타이틀 화면으로
    [SerializeField] private Button     context_button;     // 승리/패배에 따라 바뀌는 버튼
    [SerializeField] private TMP_Text   context_button_text;// 버튼 텍스트

    // ─────────────────────────────────────────
    // 승리 요소
    // ─────────────────────────────────────────
    [Header("승리")]
    [SerializeField] private GameObject win_panel;          // 승리 패널
    [SerializeField] private TMP_Text   win_text;           // 승리! 텍스트
    [SerializeField] private Button     next_chapter_button;// 다음 챕터로

    // ─────────────────────────────────────────
    // 패배 요소
    // ─────────────────────────────────────────
    [Header("패배")]
    [SerializeField] private GameObject lose_panel;         // 패배 패널
    [SerializeField] private TMP_Text   lose_text;          // 패배... 텍스트
    [SerializeField] private Button     retry_button;       // 다시하기

    // ─────────────────────────────────────────
    // 내부 상태
    // ─────────────────────────────────────────
    private float   play_time    = 0f;
    private bool    is_counting  = true;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        // 시작 시 패널 닫기
        result_panel.SetActive(false);
    }

    private void Start()
    {
        // 버튼 이벤트 연결
        if (title_button        != null) title_button.onClick.AddListener(OnTitleButton);
        if (next_chapter_button != null) next_chapter_button.onClick.AddListener(OnNextChapterButton);
        if (retry_button        != null) retry_button.onClick.AddListener(OnRetryButton);
    }

    private void Update()
    {
        if (!is_counting) return;

        // 플레이 타임 카운트
        play_time += Time.deltaTime;
        UpdatePlayTimeText();
    }

    // ─────────────────────────────────────────
    // 플레이 타임 텍스트 업데이트
    // ─────────────────────────────────────────
    private void UpdatePlayTimeText()
    {
        if (play_time_text == null) return;

        int hours   = (int)(play_time / 3600);
        int minutes = (int)(play_time % 3600 / 60);
        int seconds = (int)(play_time % 60);

        play_time_text.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    // ─────────────────────────────────────────
    // 승리 결산창 표시
    // ─────────────────────────────────────────
    public void ShowWin()
    {
        is_counting = false;

        result_panel.SetActive(true);
        win_panel?.SetActive(true);
        lose_panel?.SetActive(false);

        // context_button → 다음 챕터로
        if (context_button != null)
        {
            context_button.gameObject.SetActive(true);
            context_button.onClick.RemoveAllListeners();
            context_button.onClick.AddListener(OnNextChapterButton);
            if (context_button_text != null)
                context_button_text.text = "다음 챕터로";
        }

        Debug.Log("[ResultUI] 승리 결산창 표시!");
    }

    // ─────────────────────────────────────────
    // 패배 결산창 표시
    // ─────────────────────────────────────────
    public void ShowLose()
    {
        is_counting = false;

        result_panel.SetActive(true);
        win_panel?.SetActive(false);
        lose_panel?.SetActive(true);

        // context_button → 다시하기
        if (context_button != null)
        {
            context_button.gameObject.SetActive(true);
            context_button.onClick.RemoveAllListeners();
            context_button.onClick.AddListener(OnRetryButton);
            if (context_button_text != null)
                context_button_text.text = "다시하기";
        }

        Debug.Log("[ResultUI] 패배 결산창 표시!");
    }

    // ─────────────────────────────────────────
    // 버튼 이벤트
    // ─────────────────────────────────────────
    private void OnTitleButton()
    {
        SoundManager.Instance?.PlayUISelect();
        GameManager.Instance?.ResetGame();
        result_panel.SetActive(false);
        SceneController.Instance.LoadTitle();
    }

    private void OnNextChapterButton()
    {
        SoundManager.Instance?.PlayUISelect();
        GameManager.Instance?.ResetGame();
        result_panel.SetActive(false);
        SceneController.Instance.LoadNextChapter();
        Debug.Log("[ResultUI] 다음 챕터로!");
    }

    private void OnRetryButton()
    {
        SoundManager.Instance?.PlayUISelect();
        GameManager.Instance?.ResetGame();
        result_panel.SetActive(false);
        SceneController.Instance.LoadInGame();
        Debug.Log("[ResultUI] 다시하기!");
    }
}