using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PlayerData playerData;

    [SerializeField] private GameObject projectile_prefab;
    [SerializeField] private Transform shoot_point;

    [Header("방향별 발사체 스프라이트")]
    [SerializeField] private Sprite sprite_up;
    [SerializeField] private Sprite sprite_down;
    [SerializeField] private Sprite sprite_left;
    [SerializeField] private Sprite sprite_right;

    private float last_shoot_time = 0f;
    private float shoot_cooldown;

    private void Start()
    {
        playerData = PlayerDataLoader.Instance.GetPlayerData();

        if (playerData == null)
        {
            Debug.LogError("Weapon: PlayerData를 가져올 수 없습니다.");
            return;
        }

        shoot_cooldown = 1f / playerData.atk_speed;
    }

    public void Shoot(Vector2 direction)
    {
        if (playerData == null) return;

        if (Time.time - last_shoot_time < shoot_cooldown)
            return;

        if (projectile_prefab == null)
        {
            Debug.LogError("Weapon: Projectile prefab이 할당되지 않았습니다.");
            return;
        }

        CreateProjectile(direction);

        for (int i = 0; i < playerData.add_bullet; i++)
        {
            CreateProjectile(direction);
        }

        last_shoot_time = Time.time;
    }

    private void CreateProjectile(Vector2 direction)
    {
        Vector3 spawn_pos = shoot_point != null ? shoot_point.position : transform.position;
        GameObject projectile_obj = Instantiate(projectile_prefab, spawn_pos, Quaternion.identity);

        Projectile projectile = projectile_obj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                playerData.bullet_speed,
                playerData.atk,
                playerData.range,
                playerData.pierce,
                playerData.bleed,
                GetDirectionSprite(direction)
            );
        }
    }

    private Sprite GetDirectionSprite(Vector2 direction)
    {
        if (direction == Vector2.up)    return sprite_up;
        if (direction == Vector2.down)  return sprite_down;
        if (direction == Vector2.left)  return sprite_left;
        if (direction == Vector2.right) return sprite_right;
        return null;
    }

    public float GetAttackDamage() => playerData?.atk ?? 1f;
    public float GetAttackSpeed() => playerData?.atk_speed ?? 1f;
    public float GetBulletSpeed() => playerData?.bullet_speed ?? 1f;
    public float GetRange() => playerData?.range ?? 1f;
    public bool HasPierce() => playerData?.pierce ?? false;
    public bool HasBleed() => playerData?.bleed ?? false;
}