using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 페이드 인/아웃 컨트롤러
/// 씬 전환 이벤트를 스스로 구독해서 자동 페이드 처리
/// GameManager 불필요 — 프리팹으로 씬에 놓기만 하면 됨
/// </summary>
public class FadeController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fade_image;
    [SerializeField] private float fade_duration = 0.5f;

    private static FadeController instance;
    public static  FadeController Instance => instance;

    private bool is_fading = false;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Canvas SortingOrder 최고값 → 모든 UI 위에
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder    = 9999;
        }

        // 씬 전환 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 시작 시 검정 준비
        SetAlpha(1f);
        if (fade_image != null)
            fade_image.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 첫 씬 시작 시 페이드 인
        StartCoroutine(FadeInDelayed());
    }

    /// <summary>씬 로드 완료 시 자동 페이드 인</summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Additive 로드는 페이드 처리 안 함
        if (mode == LoadSceneMode.Additive) return;

        StartCoroutine(FadeInDelayed());
    }

    private IEnumerator FadeInDelayed()
    {
        yield return null; // 한 프레임 대기 (뚝 켜지는 현상 방지)
        yield return StartCoroutine(FadeIn());
    }

    // ─────────────────────────────────────────
    // 화면 → 검정 (어두워짐)
    // ─────────────────────────────────────────
    public IEnumerator FadeOut()
    {
        if (fade_image != null)
            fade_image.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(0f, 1f)); // 투명 → 검정
    }

    // ─────────────────────────────────────────
    // 검정 → 화면 (밝아짐)
    // ─────────────────────────────────────────
    public IEnumerator FadeIn()
    {
        if (fade_image != null)
            fade_image.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(1f, 0f)); // 검정 → 투명

        if (fade_image != null)
            fade_image.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    // 페이드 공통
    // ─────────────────────────────────────────
    private IEnumerator Fade(float from, float to)
    {
        if (fade_image == null) yield break;

        // 이미 페이드 중이면 완료될 때까지 대기
        while (is_fading)
            yield return null;

        is_fading = true;
        SetAlpha(from);

        float elapsed = 0f;
        while (elapsed < fade_duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fade_duration);
            SetAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetAlpha(to);
        is_fading = false;
    }

    private void SetAlpha(float alpha)
    {
        if (fade_image == null) return;
        Color c = fade_image.color;
        c.a = alpha;
        fade_image.color = c;
    }
}