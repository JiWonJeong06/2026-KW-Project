using UnityEngine;

public class MyckaDataLoader : MonoBehaviour
{
     [SerializeField] private TextAsset jsonFile;
    [SerializeField] private Player player;
    [SerializeField] private Bullet bullet;
    [SerializeField] private Pet pet;
    [SerializeField] private HomingBullet homingBullet;
    void Start()
    {
        MyckaData data = JsonUtility.FromJson<MyckaData>(jsonFile.text);
        player.ApplyData(data);
        bullet.ApplyData(data);
        pet.ApplyPetData(data);
        homingBullet.ApplyBulletData(data);

    }
}
