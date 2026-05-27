using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    private static BossHealthBar instance;
    public static BossHealthBar Instance => instance;

    [Header("Boss Health Bar")]
    [SerializeField] private GameObject boss_health_panel;      // 보스 체력바 Panel
    [SerializeField] private Slider health_slider;              // 체력바 Slider ⭐
    [SerializeField] private TextMeshProUGUI boss_name_text;    // 보스 이름
    [SerializeField] private TextMeshProUGUI health_text;       // HP 텍스트

    private Boss current_boss;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 초기 상태: 숨김
        if (boss_health_panel != null)
        {
            boss_health_panel.SetActive(false);
        }
    }

    // 보스 체력바 표시
    public void ShowBossHealthBar(Boss boss)
    {
        current_boss = boss;
        
        if (boss_health_panel != null)
        {
            boss_health_panel.SetActive(true);
        }

        if (boss_name_text != null)
        {
            boss_name_text.text = "시안 보스";
        }

        // Slider 초기 설정
        if (health_slider != null)
        {
            health_slider.maxValue = boss.GetMaxHp();
            health_slider.minValue = 0;
            health_slider.value = boss.GetCurrentHp();
        }

        // 초기 체력바
        UpdateHealthBar(boss.GetCurrentHp(), boss.GetMaxHp());
    }

    // 보스 체력바 업데이트
    public void UpdateHealthBar(float current_hp, float max_hp)
    {
        // Slider 값 업데이트
        if (health_slider != null)
        {
            health_slider.maxValue = max_hp;
            health_slider.value = current_hp;
        }

        // HP 텍스트 업데이트
        if (health_text != null)
        {
            health_text.text = $"{Mathf.CeilToInt(current_hp)} / {Mathf.CeilToInt(max_hp)}";
        }
    }

    // 보스 체력바 숨김
    public void HideBossHealthBar()
    {
        if (boss_health_panel != null)
        {
            boss_health_panel.SetActive(false);
        }

        current_boss = null;
    }
}