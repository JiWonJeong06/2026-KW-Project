using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] protected int mobID;

    [Header("Info")]
    public string mobName;
    public string mobType;

    [Header("Stat")]
    public float atk;
    public float atkspeed;
    public float bulletspeed;
    public float maxHp;
    public float currentHp;
    public float movespeed;
    public float range;

    protected Transform player;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        FindPlayer();
        LoadData();
    }

    protected virtual void FindPlayer()
    {
        Player foundPlayer = FindFirstObjectByType<Player>();

        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogWarning("Player를 찾지 못함");
        }
    }

    protected virtual void LoadData()
    {
        if (MobDataLoader.Instance == null)
        {
            Debug.LogWarning("MobDataLoader가 씬에 없음");
            return;
        }

        MobData data = MobDataLoader.Instance.GetMobData(mobID);

        if (data == null)
            return;

        ApplyData(data);
    }

    public virtual void ApplyData(MobData data)
    {
        mobName = data.Name;
        mobType = data.Type;

        atk = data.atk;
        atkspeed = data.atkspeed;
        bulletspeed = data.bulletspeed;
        maxHp = data.hp;
        currentHp = maxHp;
        movespeed = data.movespeed;
        range = data.range;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected float GetDistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;

        return Vector2.Distance(transform.position, player.position);
    }

    protected Vector2 GetDirectionToPlayer()
    {
        if (player == null)
            return Vector2.zero;

        return ((Vector2)player.position - (Vector2)transform.position).normalized;
    }
}