    using System.Collections.Generic;
    using UnityEngine;

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField]
        private int width = 20;

        [SerializeField]
        private int height = 10;

        [SerializeField]
        private float tileSize = 1f;

        [SerializeField]
        private Vector2 center;

        private int mapCode;

        private void Awake()
        {
            RandomMapCode();

            Debug.Log($"선택된 맵 코드 : {mapCode}");

            SpawnEnemies();
        }

        private void RandomMapCode()
        {
            // Cyan 맵만 랜덤
            mapCode = Random.Range(1001, 1008);
        }

        public void SpawnEnemies()
        {
            SpawnTableData table =
                DataManager.Instance.GetSpawnTable(mapCode);

            if (table == null)
            {
                Debug.LogError($"맵 코드 {mapCode} 없음");
                return;
            }

            List<Vector2> positions = GeneratePositions();

            int positionIndex = 0;

            foreach (var monster in table.monsters)
            {
                int spawnCount =
                    Random.Range(monster.minSpawn,
                                monster.maxSpawn + 1);

                for (int i = 0; i < spawnCount; i++)
                {
                    if (positionIndex >= positions.Count)
                    {
                        Debug.LogWarning("소환 위치 부족");
                        return;
                    }

                    GameObject prefab =
                        DataManager.Instance.GetEnemyPrefab(
                            monster.monsterId);

                    if (prefab == null)
                    {
                        Debug.LogWarning(
                            $"몬스터 ID {monster.monsterId} 프리팹 없음");
                        continue;
                    }

                    Instantiate(
                        prefab,
                        positions[positionIndex],
                        Quaternion.identity
                    );

                    positionIndex++;
                }
            }
        }

        private List<Vector2> GeneratePositions()
        {
            List<Vector2> positions = new List<Vector2>();

            float startX =
                center.x - (width / 2f) * tileSize + tileSize * 0.5f;

            float startY =
                center.y - (height / 2f) * tileSize + tileSize * 0.5f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    positions.Add(
                        new Vector2(
                            startX + x * tileSize,
                            startY + y * tileSize
                        )
                    );
                }
            }

            Shuffle(positions);

            return positions;
        }

        private void Shuffle(List<Vector2> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);

                (list[i], list[rand]) =
                    (list[rand], list[i]);
            }
        }
    }