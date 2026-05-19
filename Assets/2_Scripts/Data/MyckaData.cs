[System.Serializable]
public class MyckaData
{
    // ===== 기본 정보 =====
    public string name;
    public string koreanName;
    public string japaneseName;

    // ===== 전투 스탯 (변수명 통일: camelCase) =====
    public float attackDamage;      // 공격력
    public float attackRange;       // 사거리
    public float attackSpeed;       // 공격속도
    public float bulletSpeed;       // 총알속도
    public float additionalBullets; // 추가총알
    
    // ===== 기본 스탯 =====
    public float maxHp;             // 최대체력
    public float moveSpeed;         // 이동속도

    // ===== 특수 효과 =====
    public bool bleed;              // 출혈 효과
    public bool pierce;             // 관통 효과
}