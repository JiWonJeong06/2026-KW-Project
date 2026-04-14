using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int spawnCount;

    [Header("Grid Settings")]
    [SerializeField] private int width;  
    [SerializeField] private int height;     
    [SerializeField] private float tileSize; 
    [SerializeField] private Vector2 center = Vector2.zero; 

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        int maxCount = width * height;
        int finalSpawnCount = Mathf.Min(spawnCount, maxCount);

        List<Vector2> availablePositions = new List<Vector2>();

        // 중앙 기준 시작 오프셋
        float startX = center.x - (width / 2f) * tileSize + tileSize * 0.5f;
        float startY = center.y - (height / 2f) * tileSize + tileSize * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 worldPos = new Vector2(
                    startX + x * tileSize,
                    startY + y * tileSize
                );

                availablePositions.Add(worldPos);
            }
        }

        // Fisher-Yates Shuffle
        for (int i = 0; i < availablePositions.Count; i++)
        {
            int randomIndex = Random.Range(i, availablePositions.Count);
            Vector2 temp = availablePositions[i];
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        for (int i = 0; i < finalSpawnCount; i++)
        {
            Instantiate(enemyPrefab, availablePositions[i], Quaternion.identity);
        }
    }
}