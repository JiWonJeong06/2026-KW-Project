using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Prefabs")]
    [SerializeField] private Button[] cyanCards;
    [SerializeField] private Button[] magentaCards;
    [SerializeField] private Button[] yellowCards;

    [Header("Spawn Parent")]
    [SerializeField] private Transform cardSpawnPoint;

    private readonly List<Button> spawnedCards = new List<Button>();

    public void SpawnDummyCards(DoorColor color, int count = 3)
    {
        ClearCards();

        Button[] sourceCards = GetCardsByColor(color);

        if (sourceCards == null || sourceCards.Length == 0)
        {
            Debug.LogWarning($"{color} 색상의 카드 프리팹이 없음");
            return;
        }

        if (cardSpawnPoint == null)
        {
            Debug.LogWarning("cardSpawnPoint가 비어 있음");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, sourceCards.Length);

            Button cardPrefab = sourceCards[randomIndex];
            Button newCard = Instantiate(cardPrefab, cardSpawnPoint);
            newCard.gameObject.SetActive(true);

            // 기존 클릭 이벤트 제거
            newCard.onClick.RemoveAllListeners();

            // 버튼 클릭 시 다음 방 로드
            newCard.onClick.AddListener(() =>
            {
                ClearCards();

                if (StageManager.Instance != null)
                {
                    StageManager.Instance.LoadNextRoom();
                }
                else
                {
                    Debug.LogWarning("StageManager.Instance가 없음");
                }
            });

            spawnedCards.Add(newCard);
        }

        Debug.Log($"{color} 카드 {count}장 소환 완료");
    }

    private Button[] GetCardsByColor(DoorColor color)
    {
        switch (color)
        {
            case DoorColor.Cyan:
                return cyanCards;

            case DoorColor.Magenta:
                return magentaCards;

            case DoorColor.Yellow:
                return yellowCards;

            default:
                return null;
        }
    }

    public void ClearCards()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                Destroy(spawnedCards[i].gameObject);
            }
        }

        spawnedCards.Clear();
    }
}