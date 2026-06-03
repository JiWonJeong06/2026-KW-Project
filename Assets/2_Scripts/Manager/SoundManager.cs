using UnityEngine;

/// <summary>
/// BGM / SFX 통합 사운드 매니저
/// BGM: .ogg / SFX: .wav
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    [Header("BGM Source")]
    [SerializeField] private AudioSource bgm_source;

    [Header("SFX Source")]
    [SerializeField] private AudioSource sfx_source;
    [SerializeField] private AudioSource walk_source;       // 걷기 전용 (루프)

    // ─────────────────────────────────────────
    // BGM 클립 (.ogg)
    // ─────────────────────────────────────────
    [Header("BGM Clips (.ogg)")]
    [SerializeField] private AudioClip bgm_title;           // 타이틀
    [SerializeField] private AudioClip bgm_stage;           // 스테이지 1~7
    [SerializeField] private AudioClip bgm_boss;            // 스테이지 8 보스

    // ─────────────────────────────────────────
    // SFX 클립 (.wav)
    // ─────────────────────────────────────────
    [Header("Player SFX (.wav)")]
    [SerializeField] private AudioClip sfx_player_walk;     // 걷기 (루프)
    [SerializeField] private AudioClip sfx_player_hit;      // 피격
    [SerializeField] private AudioClip sfx_player_attack;   // 공격
    [SerializeField] private AudioClip sfx_player_death;    // 죽음

    [Header("Enemy SFX (.wav)")]
    [SerializeField] private AudioClip sfx_enemy_hit;       // 적 피격

    [Header("Boss SFX (.wav)")]
    [SerializeField] private AudioClip sfx_boss_hit;        // 보스 피격
    [SerializeField] private AudioClip sfx_boss_death;      // 보스 죽음

    [Header("UI SFX (.wav)")]
    [SerializeField] private AudioClip sfx_ui_hover;        // 버튼 호버
    [SerializeField] private AudioClip sfx_ui_select;       // 버튼 셀렉트

    // ─────────────────────────────────────────
    // 볼륨
    // ─────────────────────────────────────────
    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float bgm_volume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float sfx_volume = 1.0f;

    // =========================================================
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        bgm_source.loop   = true;
        bgm_source.volume = bgm_volume;

        walk_source.loop   = true;
        walk_source.volume = sfx_volume;
        walk_source.clip   = sfx_player_walk;
    }

    // =========================================================
    // BGM
    // =========================================================
    public void PlayBGM_Title() => PlayBGM(bgm_title);
    public void PlayBGM_Stage() => PlayBGM(bgm_stage);
    public void PlayBGM_Boss()  => PlayBGM(bgm_boss);

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("[SoundManager] BGM 클립 미할당!"); return; }
        if (bgm_source.clip == clip && bgm_source.isPlaying) return;

        bgm_source.clip   = clip;
        bgm_source.volume = bgm_volume;
        bgm_source.Play();
    }

    public void StopBGM() => bgm_source.Stop();

    // =========================================================
    // Player SFX
    // =========================================================
    public void PlayPlayerAttack() => PlaySFX(sfx_player_attack);
    public void PlayPlayerHit()    => PlaySFX(sfx_player_hit);
    public void PlayPlayerDeath()  => PlaySFX(sfx_player_death);

    // 걷기 — 움직이면 재생, 멈추면 즉시 정지
    public void StartWalk()
    {
        if (sfx_player_walk == null || walk_source.isPlaying) return;
        walk_source.volume = sfx_volume;
        walk_source.Play();
    }

    public void StopWalk()
    {
        if (walk_source.isPlaying) walk_source.Stop();
    }

    // =========================================================
    // Enemy SFX
    // =========================================================
    public void PlayEnemyHit()   => PlaySFX(sfx_enemy_hit);

    // =========================================================
    // Boss SFX
    // =========================================================
    public void PlayBossHit()    => PlaySFX(sfx_boss_hit);
    public void PlayBossDeath()  => PlaySFX(sfx_boss_death);

    // =========================================================
    // UI SFX
    // =========================================================
    public void PlayUIHover()    => PlaySFX(sfx_ui_hover);
    public void PlayUISelect()   => PlaySFX(sfx_ui_select);

    // =========================================================
    // 공통 SFX 재생
    // =========================================================
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) { Debug.LogWarning("[SoundManager] SFX 클립 미할당!"); return; }
        sfx_source.PlayOneShot(clip, sfx_volume);
    }

    // =========================================================
    // 볼륨 조절
    // =========================================================
    public void SetBGMVolume(float volume)
    {
        bgm_volume        = Mathf.Clamp01(volume);
        bgm_source.volume = bgm_volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfx_volume         = Mathf.Clamp01(volume);
        walk_source.volume = sfx_volume;
    }

    public float GetBGMVolume() => bgm_volume;
    public float GetSFXVolume() => sfx_volume;
}