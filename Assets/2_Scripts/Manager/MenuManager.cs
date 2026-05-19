using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Button firstButton;
    public GameObject SettingsPanel;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        firstButton.Select();
    }
    public void OnSettingsButton()
    {
        // 설정 메뉴 로직 추가
        SettingsPanel.SetActive(true);
        Debug.Log("설정 메뉴");
    }

    public void OnExitButton()
    {
        // 게임 종료 로직 추가
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
