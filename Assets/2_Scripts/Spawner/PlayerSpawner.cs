using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3 spawnPosition;
    public Transform parentTransform;

    void Awake()
    {
        if (playerPrefab == null) return;

        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, parentTransform);

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(player.transform);
        }
    }
}