using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 볼륨 설정 UI
/// BGM / SFX Slider 조절 → 저장 버튼으로 PlayerPrefs에 저장
/// 슬라이더 범위: 0~100 표기, 실제 볼륨: 0~1 변환
/// </summary>
public class VolumeSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider   bgm_slider;
    [SerializeField] private Slider   sfx_slider;

    [Header("볼륨 텍스트 (TMP)")]
    [SerializeField] private TMP_Text bgm_text;   // BGM 수치 표시 (0~100)
    [SerializeField] private TMP_Text sfx_text;   // SFX 수치 표시 (0~100)

    [Header("버튼")]
    [SerializeField] private Button save_button;
    [SerializeField] private Button open_button;

    [Header("Settings 패널")]
    [SerializeField] private GameObject settings_panel;

    private const string KEY_BGM     = "BGMVolume";
    private const string KEY_SFX     = "SFXVolume";
    private const float  DEFAULT_VOL = 0.5f; // 기본값 50%

    private void Awake()
    {
        // 슬라이더 범위 0~100으로 설정
        if (bgm_slider != null) { bgm_slider.minValue = 0f; bgm_slider.maxValue = 100f; }
        if (sfx_slider != null) { sfx_slider.minValue = 0f; sfx_slider.maxValue = 100f; }

        // 시작 시 패널 닫기
        CloseSettings();
    }

    private void Start()
    {
        // 저장된 볼륨 불러오기 (없으면 기본값 50)
        float saved_bgm = PlayerPrefs.GetFloat(KEY_BGM, DEFAULT_VOL);
        float saved_sfx = PlayerPrefs.GetFloat(KEY_SFX, DEFAULT_VOL);

        // 슬라이더 초기값 (0~1 → 0~100)
        if (bgm_slider != null) bgm_slider.value = saved_bgm * 100f;
        if (sfx_slider != null) sfx_slider.value = saved_sfx * 100f;

        // 텍스트 초기값
        UpdateBGMText(saved_bgm * 100f);
        UpdateSFXText(saved_sfx * 100f);

        // SoundManager 적용
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(saved_bgm);
            SoundManager.Instance.SetSFXVolume(saved_sfx);
        }
        else
        {
            Debug.LogWarning("[VolumeSettingsUI] SoundManager.Instance가 없습니다!");
        }

        // 이벤트 연결
        if (bgm_slider  != null) bgm_slider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (sfx_slider  != null) sfx_slider.onValueChanged.AddListener(OnSFXSliderChanged);
        if (save_button != null) save_button.onClick.AddListener(SaveVolume);
        if (open_button != null) open_button.onClick.AddListener(OpenSettings);
    }

    private void OnDestroy()
    {
        if (bgm_slider  != null) bgm_slider.onValueChanged.RemoveListener(OnBGMSliderChanged);
        if (sfx_slider  != null) sfx_slider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        if (save_button != null) save_button.onClick.RemoveListener(SaveVolume);
        if (open_button != null) open_button.onClick.RemoveListener(OpenSettings);
    }

    // ─────────────────────────────────────────
    // 패널 열기 / 닫기
    // ─────────────────────────────────────────
    public void OpenSettings()
    {
        if (settings_panel != null)
            settings_panel.SetActive(true);

        SoundManager.Instance?.PlayUISelect();
    }

    public void CloseSettings()
    {
        if (settings_panel != null)
            settings_panel.SetActive(false);
    }

    // ─────────────────────────────────────────
    // 슬라이더 실시간 반영 (0~100 → 0~1 변환)
    // ─────────────────────────────────────────
    private void OnBGMSliderChanged(float value)
    {
        float volume = value / 100f;
        SoundManager.Instance?.SetBGMVolume(volume);
        UpdateBGMText(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        float volume = value / 100f;
        SoundManager.Instance?.SetSFXVolume(volume);
        UpdateSFXText(value);
    }

    // ─────────────────────────────────────────
    // 텍스트 업데이트
    // ─────────────────────────────────────────
    private void UpdateBGMText(float value)
    {
        if (bgm_text != null)
            bgm_text.text = $"{Mathf.RoundToInt(value)}%";
    }

    private void UpdateSFXText(float value)
    {
        if (sfx_text != null)
            sfx_text.text = $"{Mathf.RoundToInt(value)}%";
    }

    // ─────────────────────────────────────────
    // 저장 버튼 → 저장 후 패널 닫기
    // ─────────────────────────────────────────
    private void SaveVolume()
    {
        // 슬라이더 값 0~100 → 0~1 변환해서 저장
        float bgm_val = bgm_slider != null ? bgm_slider.value / 100f : DEFAULT_VOL;
        float sfx_val = sfx_slider != null ? sfx_slider.value / 100f : DEFAULT_VOL;

        PlayerPrefs.SetFloat(KEY_BGM, bgm_val);
        PlayerPrefs.SetFloat(KEY_SFX, sfx_val);
        PlayerPrefs.Save();

        SoundManager.Instance?.PlayUISelect();

        Debug.Log($"[VolumeSettingsUI] 저장 완료 — BGM:{Mathf.RoundToInt(bgm_val * 100f)}, SFX:{Mathf.RoundToInt(sfx_val * 100f)}");

        CloseSettings();
    }

    // ─────────────────────────────────────────
    // 게임 시작 시 저장된 볼륨 적용
    // ─────────────────────────────────────────
    public static void LoadSavedVolume()
    {
        if (SoundManager.Instance == null) return;

        float bgm = PlayerPrefs.GetFloat(KEY_BGM, DEFAULT_VOL);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, DEFAULT_VOL);

        SoundManager.Instance.SetBGMVolume(bgm);
        SoundManager.Instance.SetSFXVolume(sfx);

        Debug.Log($"[VolumeSettingsUI] 볼륨 로드 — BGM:{Mathf.RoundToInt(bgm * 100f)}, SFX:{Mathf.RoundToInt(sfx * 100f)}");
    }
}