using UnityEngine;

public class MyckaDataLoader : MonoBehaviour
{
     [SerializeField] private TextAsset jsonFile;
    [SerializeField] private Player player;
    [SerializeField] private Pet pet;
    [SerializeField] private HomingBullet homingBullet;
    [SerializeField] private Bullet[] bullet;
    void Awake()
    {
        MyckaData data = JsonUtility.FromJson<MyckaData>(jsonFile.text);
        player.ApplyData(data);
        for (int i = 0; i < bullet.Length; i++)
        {
            bullet[i].ApplyData(data);
        }
        pet.ApplyPetData(data);
        homingBullet.ApplyBulletData(data);

    }
}
