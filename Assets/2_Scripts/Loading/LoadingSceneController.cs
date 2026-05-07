using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text progressText;

    void Start()
    {
        LoadManager.Instance.StartLoading();
    }

    void Update()
    {
        float progress = LoadManager.Instance.Progress;

        slider.value = progress;

        progressText.text =
            $"{Mathf.RoundToInt(progress * 100f)}%";
    }
}