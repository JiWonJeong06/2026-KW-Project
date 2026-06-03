using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// 로딩 씬 UI 관리
/// 1. InGame 씬 비동기 로드 (씬 활성화)
/// 2. 씬 활성화 후 JSON 로더 초기화 대기
/// 3. 모두 완료되면 GameManager.InitGame() 호출
/// </summary>
public class LoadingSceneUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider    loading_bar;
    [SerializeField] private TMP_Text  loading_text;
    [SerializeField] private Image     loading_sprite;

    [Header("Loading Messages")]
    [SerializeField] private string[] loading_messages = {
        "데이터 로드 중...",
        "맵 준비 중...",
        "거의 다 됐어요!",
        "완료!"
    };

    [Header("Sprite Animation")]
    [SerializeField] private Sprite[] anim_sprites;
    [SerializeField] private float    anim_interval = 0.1f;

    [Header("Settings")]
    [SerializeField] private float min_load_time = 1.5f; // 최소 로딩 시간

    private int   anim_index = 0;
    private float anim_timer = 0f;

    private void Start()
    {
        if (loading_bar != null)
        {
            loading_bar.minValue = 0f;
            loading_bar.maxValue = 1f;
            loading_bar.value    = 0f;
        }

        if (loading_text != null)
            loading_text.text = loading_messages[0];

        StartCoroutine(LoadProcess());
    }

    private void Update()
    {
        UpdateSpriteAnimation();
    }

    private IEnumerator LoadProcess()
    {
        float elapsed = 0f;

        string target = SceneController.next_scene;
        if (string.IsNullOrEmpty(target))
        {
            Debug.LogError("[LoadingSceneUI] next_scene 미설정!");
            yield break;
        }

        // ── 1단계: InGame 씬 비동기 로드 (0% ~ 60%) ──────
        SetText(0);

        AsyncOperation op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Additive);
        op.allowSceneActivation = true; // 바로 활성화

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            SetBar(progress * 0.6f);
            yield return null;
        }

        Debug.Log("[Loading] InGame 씬 로드 완료");
        SetBar(0.6f);
        SetText(1);

        // ── 2단계: JSON 로더 초기화 대기 (60% ~ 90%) ──────
        float wait_timer = 0f;
        float max_wait   = 5f;

        while (wait_timer < max_wait)
        {
            wait_timer += Time.deltaTime;
            elapsed    += Time.deltaTime;

            bool ready = CheckDataLoaders();

            float t = Mathf.Clamp01(wait_timer / max_wait);
            SetBar(0.6f + t * 0.3f);

            if (ready)
            {
                Debug.Log("[Loading] 모든 데이터 로드 완료!");
                SetBar(0.9f);
                SetText(2);
                break;
            }

            yield return null;
        }

        // ── 3단계: 최소 로딩 시간 보장 (90% ~ 100%) ───────
        while (elapsed < min_load_time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / min_load_time);
            SetBar(0.9f + t * 0.1f);
            yield return null;
        }

        SetBar(1f);
        SetText(3);
        yield return new WaitForSeconds(0.3f);

        // ── 4단계: 카메라 정리 후 씬 전환 ──────────────────

        // 로딩 씬 카메라 + EventSystem 비활성화
        Scene loading_scene = SceneManager.GetSceneByName(SceneController.SCENE_LOADING);
        foreach (GameObject obj in loading_scene.GetRootGameObjects())
        {
            // 카메라 비활성화
            foreach (Camera cam in obj.GetComponentsInChildren<Camera>(true))
            {
                cam.gameObject.SetActive(false);
                Debug.Log($"[Loading] 로딩 카메라 비활성화: {cam.gameObject.name}");
            }

            // EventSystem 비활성화
            foreach (UnityEngine.EventSystems.EventSystem es in obj.GetComponentsInChildren<UnityEngine.EventSystems.EventSystem>(true))
            {
                es.gameObject.SetActive(false);
                Debug.Log($"[Loading] 로딩 EventSystem 비활성화: {es.gameObject.name}");
            }
        }

        // InGame 씬 카메라 활성화
        Scene ingame_scene = SceneManager.GetSceneByName(target);
        foreach (GameObject obj in ingame_scene.GetRootGameObjects())
        {
            foreach (Camera cam in obj.GetComponentsInChildren<Camera>(true))
            {
                cam.gameObject.SetActive(true);
                Debug.Log($"[Loading] InGame 카메라 활성화: {cam.gameObject.name}");
            }
        }

        // 활성 씬을 InGame으로 전환
        SceneManager.SetActiveScene(ingame_scene);

        // 로딩 씬 제거
        SceneManager.UnloadSceneAsync(SceneController.SCENE_LOADING);

        // GameManager 초기화
        if (GameManager.Instance != null)
            GameManager.Instance.InitGame();
    }

    // ─────────────────────────────────────────
    // 로더 준비 체크
    // ─────────────────────────────────────────
    private bool CheckDataLoaders()
    {
        bool player = PlayerDataLoader.Instance != null;
        bool map    = MapSpawnDataLoader.Instance != null;
        bool enemy  = EnemyDataLoader.Instance != null;

        return player && map && enemy;
    }

    // ─────────────────────────────────────────
    // UI 유틸
    // ─────────────────────────────────────────
    private void SetBar(float value)
    {
        if (loading_bar != null)
            loading_bar.value = value;
    }

    private void SetText(int idx)
    {
        if (loading_text == null || loading_messages.Length == 0) return;
        idx = Mathf.Clamp(idx, 0, loading_messages.Length - 1);
        loading_text.text = loading_messages[idx];
    }

    private void UpdateSpriteAnimation()
    {
        if (loading_sprite == null || anim_sprites == null || anim_sprites.Length == 0) return;

        anim_timer += Time.deltaTime;
        if (anim_timer >= anim_interval)
        {
            anim_timer = 0f;
            anim_index = (anim_index + 1) % anim_sprites.Length;
            loading_sprite.sprite = anim_sprites[anim_index];
        }
    }
}