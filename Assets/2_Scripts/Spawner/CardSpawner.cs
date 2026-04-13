using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private List<GameObject> cardPrefabs = new List<GameObject>();
    [SerializeField] private Transform spawnParent;

    [Header("Spawn Positions")]
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnRandomCards(int count)
    {
        if (cardPrefabs == null || cardPrefabs.Count == 0)
        {
            Debug.LogWarning("카드 프리팹이 비어 있음");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("카드 스폰 위치가 없음");
            return;
        }

        ClearSpawnedCards();

        List<int> randomIndexes = new List<int>();
        int spawnCount = Mathf.Min(count, Mathf.Min(cardPrefabs.Count, spawnPoints.Length));

        while (randomIndexes.Count < spawnCount)
        {
            int randomIndex = Random.Range(0, cardPrefabs.Count);

            if (!randomIndexes.Contains(randomIndex))
            {
                randomIndexes.Add(randomIndex);
            }
        }

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject cardPrefab = cardPrefabs[randomIndexes[i]];
            Transform point = spawnPoints[i];

            Instantiate(cardPrefab, point.position, Quaternion.identity, spawnParent);
        }

        Debug.Log($"{spawnCount}장의 카드 생성 완료");
    }

    void ClearSpawnedCards()
    {
        if (spawnParent == null)
            return;

        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            Destroy(spawnParent.GetChild(i).gameObject);
        }
    }
}