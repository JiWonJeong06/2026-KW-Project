using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    private static HUDManager instance;
    public static HUDManager Instance => instance;

    [Header("Hearts")]
    [SerializeField] private GameObject heart_prefab;    // 하트 프리팹 ⭐
    [SerializeField] private Sprite heart_full;          // 채워진 하트 스프라이트
    [SerializeField] private Sprite heart_empty;         // 빈 하트 스프라이트
    [SerializeField] private Transform hearts_container; // 하트들의 부모
    private List<Image> heart_images = new List<Image>(); // 동적 생성된 하트 리스트

    [Header("Stage Info")]
    [SerializeField] private TextMeshProUGUI stage_text;  // Stage 텍스트

    [Header("Ability Counter")]
    [SerializeField] private TextMeshProUGUI cyan_text;    // Cyan 증강 개수
    [SerializeField] private TextMeshProUGUI magenta_text; // Magenta 증강 개수
    [SerializeField] private TextMeshProUGUI yellow_text;  // Yellow 증강 개수

    private Player player;
    private int last_max_hp = 0; // 최대 HP 변화 감지용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 초기 UI 업데이트
        UpdateStageUI();
        UpdateAbilityCounterUI();
    }

    private void Update()
    {
        UpdateAbilityCounterUI();
         UpdateStageUI();
        // Player 찾기
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null) return;
        }

        int current_max_hp = Mathf.CeilToInt(player.GetMaxHp());

        // 최대 HP가 변경되었으면 하트 재생성
        if (current_max_hp != last_max_hp)
        {
            CreateHearts(current_max_hp);
            last_max_hp = current_max_hp;
        }

        // 하트 업데이트
        UpdateHearts();
    }

    // 하트 동적 생성
    private void CreateHearts(int count)
    {
        if (heart_prefab == null)
        {
            Debug.LogError("[HUDManager] Heart Prefab이 할당되지 않았습니다!");
            return;
        }

        // 기존 하트 전부 삭제
        foreach (Image heart in heart_images)
        {
            if (heart != null)
            {
                Destroy(heart.gameObject);
            }
        }
        heart_images.Clear();

        // 새로운 하트 생성
        for (int i = 0; i < count; i++)
        {
            GameObject heart_obj = Instantiate(heart_prefab, hearts_container);
            Image heart_img = heart_obj.GetComponent<Image>();
            
            if (heart_img != null)
            {
                heart_images.Add(heart_img);
            }
        }

        Debug.Log($"[HUDManager] 하트 {count}개 생성 완료");
    }

    // 하트 UI 업데이트
    private void UpdateHearts()
    {
        if (player == null) return;
        if (heart_full == null || heart_empty == null) return;

        int current_hp = Mathf.CeilToInt(player.GetCurrentHp());

        // 각 하트 스프라이트 업데이트
        for (int i = 0; i < heart_images.Count; i++)
        {
            if (i < current_hp)
            {
                // 현재 HP 이하 → 채워진 하트 ❤️
                heart_images[i].sprite = heart_full;
            }
            else
            {
                // 현재 HP 초과 → 빈 하트 🤍
                heart_images[i].sprite = heart_empty;
            }
        }
    }

    // 스테이지 UI 업데이트
    public void UpdateStageUI()
    {
        if (stage_text == null) return;
        if (GameManager.Instance == null) return;

        int current_stage = GameManager.Instance.GetCurrentStage();
        stage_text.text = $"스테이지: {current_stage}/8";
    }

    // 증강 개수 UI 업데이트
    public void UpdateAbilityCounterUI()
    {
        if (PlayerStats.Instance == null) return;

        int cyan_count = PlayerStats.Instance.GetAbilityCountByType("Cyan");
        int magenta_count = PlayerStats.Instance.GetAbilityCountByType("Magenta");
        int yellow_count = PlayerStats.Instance.GetAbilityCountByType("Yellow");

        if (cyan_text != null)
        {
            cyan_text.text = $"{cyan_count}/7";
        }

        if (magenta_text != null)
        {
            magenta_text.text = $"{magenta_count}/7";
        }

        if (yellow_text != null)
        {
            yellow_text.text = $"{yellow_count}/7";
        }
    }
}