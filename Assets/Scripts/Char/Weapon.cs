using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.15f;
    public float bulletSpeed = 10f;

    float nextFireTime;

    void  FixedUpdate()
    {
        Vector2 dir = Vector2.zero;

        if (Keyboard.current.rightArrowKey.isPressed)
            dir.x += 1;

        if (Keyboard.current.leftArrowKey.isPressed)
            dir.x -= 1;

        if (Keyboard.current.upArrowKey.isPressed)
            dir.y += 1;

        if (Keyboard.current.downArrowKey.isPressed)
            dir.y -= 1;

     
        if (dir.x != 0)
            dir.y = 0;


        if (dir != Vector2.zero && Time.time >= nextFireTime)
        {
            Fire(dir.normalized);
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(dir, bulletSpeed);
    }
}