using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CutSceneManager : MonoBehaviour
{
    [Header("UI")]
    public Image backgroundImage;
    public TextMeshProUGUI narrationText;

    [Header("CutScene")]
    public CutSceneFrame[] frames;

    private int currentIndex = 0;

    private void Start()
    {
        ShowFrame();
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            NextFrame();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextFrame();
        }
    }

    void ShowFrame()
    {
        backgroundImage.sprite = frames[currentIndex].image;
        narrationText.text = frames[currentIndex].narration;
    }

    void NextFrame()
    {
        currentIndex++;

        if (currentIndex >= frames.Length)
        {
            EndCutScene();
            return;
        }

        ShowFrame();
    }

    void EndCutScene()
    {
        SceneManager.LoadScene("2_InGame");
    }
}