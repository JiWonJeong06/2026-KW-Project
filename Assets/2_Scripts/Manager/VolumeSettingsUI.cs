using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 볼륨 설정 UI
/// BGM / SFX Slider 조절 → 저장 버튼으로 PlayerPrefs에 저장
/// </summary>
public class VolumeSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider bgm_slider;
    [SerializeField] private Slider sfx_slider;

    [Header("저장 버튼")]
    [SerializeField] private Button save_button;

    private const string KEY_BGM = "BGMVolume";
    private const string KEY_SFX = "SFXVolume";

    private void Start()
    {
        // 저장된 볼륨 불러오기 (없으면 기본값)
        float saved_bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.5f);
        float saved_sfx = PlayerPrefs.GetFloat(KEY_SFX, 1.0f);

        // 슬라이더 초기값 설정
        if (bgm_slider != null) bgm_slider.value = saved_bgm;
        if (sfx_slider != null) sfx_slider.value = saved_sfx;

        // SoundManager에도 즉시 반영
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(saved_bgm);
            SoundManager.Instance.SetSFXVolume(saved_sfx);
        }

        // 슬라이더 이벤트 연결
        if (bgm_slider != null) bgm_slider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (sfx_slider != null) sfx_slider.onValueChanged.AddListener(OnSFXSliderChanged);

        // 저장 버튼 이벤트 연결
        if (save_button != null) save_button.onClick.AddListener(SaveVolume);
    }

    private void OnDestroy()
    {
        if (bgm_slider != null) bgm_slider.onValueChanged.RemoveListener(OnBGMSliderChanged);
        if (sfx_slider != null) sfx_slider.onValueChanged.RemoveListener(OnSFXSliderChanged);
        if (save_button != null) save_button.onClick.RemoveListener(SaveVolume);
    }

    // 슬라이더 조작 시 실시간 반영
    private void OnBGMSliderChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);
    }

    // 저장 버튼 클릭 시 PlayerPrefs에 저장
    private void SaveVolume()
    {
        float bgm_val = bgm_slider != null ? bgm_slider.value : 0.5f;
        float sfx_val = sfx_slider != null ? sfx_slider.value : 1.0f;

        PlayerPrefs.SetFloat(KEY_BGM, bgm_val);
        PlayerPrefs.SetFloat(KEY_SFX, sfx_val);
        PlayerPrefs.Save();

        // 저장 시 UI 버튼 SFX
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayUISelect();

        Debug.Log($"[VolumeSettingsUI] 볼륨 저장 완료 — BGM:{bgm_val:F2}, SFX:{sfx_val:F2}");
    }

    /// <summary>
    /// 게임 시작 시 저장된 볼륨을 SoundManager에 적용 (GameManager.Start에서 호출)
    /// </summary>
    public static void LoadSavedVolume()
    {
        if (SoundManager.Instance == null) return;

        float bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.5f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 1.0f);

        SoundManager.Instance.SetBGMVolume(bgm);
        SoundManager.Instance.SetSFXVolume(sfx);

        Debug.Log($"[VolumeSettingsUI] 저장된 볼륨 로드 — BGM:{bgm:F2}, SFX:{sfx:F2}");
    }
}