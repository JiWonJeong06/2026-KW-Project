using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectile_prefab;
    [SerializeField] private Transform shoot_point;

    [Header("Cyan 총알 스프라이트")]
    [SerializeField] private Sprite cyan_sprite_up;
    [SerializeField] private Sprite cyan_sprite_down;
    [SerializeField] private Sprite cyan_sprite_left;
    [SerializeField] private Sprite cyan_sprite_right;

    [Header("Magenta 총알 스프라이트")]
    [SerializeField] private Sprite magenta_sprite_up;
    [SerializeField] private Sprite magenta_sprite_down;
    [SerializeField] private Sprite magenta_sprite_left;
    [SerializeField] private Sprite magenta_sprite_right;

    [Header("Yellow 총알 스프라이트")]
    [SerializeField] private Sprite yellow_sprite_up;
    [SerializeField] private Sprite yellow_sprite_down;
    [SerializeField] private Sprite yellow_sprite_left;
    [SerializeField] private Sprite yellow_sprite_right;

    private float last_shoot_time = 0f;
    [SerializeField] private string current_weapon_type = "Cyan"; // 현재 무기 타입 (기본값: Cyan)

    public void Shoot(Vector2 direction)
    {
        // PlayerStats에서 현재 공격속도 가져오기
        float shoot_cooldown = 1f / PlayerStats.Instance.current_atk_speed;

        if (Time.time - last_shoot_time < shoot_cooldown)
            return;

        if (projectile_prefab == null)
        {
            Debug.LogError("Weapon: Projectile prefab이 할당되지 않았습니다.");
            return;
        }

        // 기본 총알 1발
        CreateProjectile(direction);

        // 추가 총알 (current_bullet_count - 1)
        for (int i = 0; i < PlayerStats.Instance.current_bullet_count - 1; i++)
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
            // PlayerStats에서 현재 스탯 가져오기
            projectile.Initialize(
                direction,
                PlayerStats.Instance.current_bullet_speed,
                PlayerStats.Instance.current_atk,
                PlayerStats.Instance.current_range,
                PlayerStats.Instance.has_pierce,
                PlayerStats.Instance.has_bleed,
                GetDirectionSprite(direction)
            );
        }
    }

    private Sprite GetDirectionSprite(Vector2 direction)
    {
        // 현재 무기 타입에 따라 스프라이트 선택
        if (current_weapon_type == "Cyan")
        {
            if (direction == Vector2.up)    return cyan_sprite_up;
            if (direction == Vector2.down)  return cyan_sprite_down;
            if (direction == Vector2.left)  return cyan_sprite_left;
            if (direction == Vector2.right) return cyan_sprite_right;
        }
        else if (current_weapon_type == "Magenta")
        {
            if (direction == Vector2.up)    return magenta_sprite_up;
            if (direction == Vector2.down)  return magenta_sprite_down;
            if (direction == Vector2.left)  return magenta_sprite_left;
            if (direction == Vector2.right) return magenta_sprite_right;
        }
        else if (current_weapon_type == "Yellow")
        {
            if (direction == Vector2.up)    return yellow_sprite_up;
            if (direction == Vector2.down)  return yellow_sprite_down;
            if (direction == Vector2.left)  return yellow_sprite_left;
            if (direction == Vector2.right) return yellow_sprite_right;
        }

        return null;
    }

    // 무기 타입 설정 (Player에서 호출)
    public void SetWeaponType(string weapon_type)
    {
        current_weapon_type = weapon_type;
        Debug.Log($"[Weapon] 무기 타입 변경: {weapon_type}");
    }

    // PlayerStats에서 값 가져오기
    public float GetAttackDamage() => PlayerStats.Instance.current_atk;
    public float GetAttackSpeed() => PlayerStats.Instance.current_atk_speed;
    public float GetBulletSpeed() => PlayerStats.Instance.current_bullet_speed;
    public float GetRange() => PlayerStats.Instance.current_range;
    public bool HasPierce() => PlayerStats.Instance.has_pierce;
    public bool HasBleed() => PlayerStats.Instance.has_bleed;
}