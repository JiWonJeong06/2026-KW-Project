using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Button firstButton;

        void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        firstButton.Select();
    }

    public void OnStartButton()
    {
        // 게임 시작 로직 추가
        Debug.Log("게임 시작");
        SceneManager.LoadScene("2_InGame");
    }
}
