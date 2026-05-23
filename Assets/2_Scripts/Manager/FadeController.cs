using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fade_image;  // 검은색 이미지
    [SerializeField] private float fade_duration = 1f;

    private void Awake()
    {
        if (fade_image == null)
        {
            fade_image = GetComponent<Image>();
        }

        // 초기 상태: 투명
        if (fade_image != null)
        {
            Color color = fade_image.color;
            color.a = 0f;
            fade_image.color = color;
        }
    }

    // 페이드 아웃 (화면 어두워짐)
    public IEnumerator FadeOut()
    {
        if (fade_image == null)
        {
            Debug.LogWarning("[FadeController] Fade Image가 없습니다!");
            yield break;
        }

        float elapsed = 0f;
        Color color = fade_image.color;

        while (elapsed < fade_duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fade_duration);
            fade_image.color = color;
            yield return null;
        }

        // 최종 값 보장
        color.a = 1f;
        fade_image.color = color;
    }

    // 페이드 인 (화면 밝아짐)
    public IEnumerator FadeIn()
    {
        if (fade_image == null)
        {
            Debug.LogWarning("[FadeController] Fade Image가 없습니다!");
            yield break;
        }

        float elapsed = 0f;
        Color color = fade_image.color;

        while (elapsed < fade_duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fade_duration);
            fade_image.color = color;
            yield return null;
        }

        // 최종 값 보장
        color.a = 0f;
        fade_image.color = color;
    }
}