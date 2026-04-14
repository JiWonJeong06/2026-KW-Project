using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    public TextAsset playerJsonFile;
    private PlayerData[] playerList;

    void Awake()
    {
        playerList = JsonManager.LoadArray<PlayerData>(playerJsonFile);

        if (playerList == null) return;

        for (int i = 0; i < playerList.Length; i++)
        {
            Debug.Log($"Player ATK: {playerList[i].Atk}, HP: {playerList[i].hp}, Speed: {playerList[i].speed}");
        }
    }
}