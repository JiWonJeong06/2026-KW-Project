using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 애니메이션 리스트
    enum Anime { Idle, Forward, Backward, Left, Right, Die }
    public Animator animator; 
    //플레이어 능력치, 이동속도, 최대체력, 현재체력
    public float speed;
    public int maxHealth;
    public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        Move();
    }
    





















    void Move()
    {
         // 상하좌우 움직임
        Vector2 input = Vector2.zero;
        //오른쪽 방향
        if (Keyboard.current.dKey.isPressed)
            input.x = 1;

        //왼쪽 방향
        if (Keyboard.current.aKey.isPressed)
            input.x = -1;


        //윗 방향
        if (Keyboard.current.wKey.isPressed)
            input.y = 1;
        //아래 방향
        if (Keyboard.current.sKey.isPressed)
            input.y = -1;

        Vector3 movement = new Vector3(input.x, input.y, 0.0f).normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("MoveX", input.x);
        animator.SetBool("isWalk", input != Vector2.zero);
    }
}