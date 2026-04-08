using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Vector2 center = Vector2.zero;
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 10;
    [SerializeField] private float tileSize = 1f;

    [Header("UI")]
    [SerializeField] private TMP_Text enemyText;

    private bool isCleared = false;

    void Update()
    {
        if (isCleared) return;

        int enemyCount = CountEnemiesInRoom();

        // TMP 출력 (000 포맷)
        enemyText.text = "Enemy: " + enemyCount.ToString("D1");

        if (enemyCount == 0)
        {
            isCleared = true;
            enemyText.text = "Clear!";  
        }
    }

    int CountEnemiesInRoom()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int count = 0;

        float halfWidth = (width * tileSize) / 2f;
        float halfHeight = (height * tileSize) / 2f;

        foreach (GameObject enemy in enemies)
        {
            Vector2 pos = enemy.transform.position;

            bool isInside =
                pos.x >= center.x - halfWidth &&
                pos.x <= center.x + halfWidth &&
                pos.y >= center.y - halfHeight &&
                pos.y <= center.y + halfHeight;

            if (isInside)
                count++;
        }

        return count;
    }
}