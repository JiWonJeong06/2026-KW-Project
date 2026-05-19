using UnityEngine;

/// <summary>
/// 색깔별 카운트에 따라 무기/탄막을 변경하는 시스템
/// 
/// 역할:
/// 1. AbilityManager의 OnAbilityApplied 이벤트 구독
/// 2. Player의 색깔 카운트 비교
/// 3. 우선순위에 따라 무기 결정 (Magenta > Cyan > Yellow)
/// 4. 무기 스프라이트 변경
/// 5. 탄막 색 변경
/// </summary>
public class WeaponUpgradeSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;  // 무기 스프라이트
    [SerializeField] private Sprite magentaWeaponSprite;
    [SerializeField] private Sprite cyanWeaponSprite;
    [SerializeField] private Sprite yellowWeaponSprite;
    
    [SerializeField] private Color magentaBulletColor = new Color(1, 0, 1, 1);  // 분홍
    [SerializeField] private Color cyanBulletColor = new Color(0, 1, 1, 1);     // 청록
    [SerializeField] private Color yellowBulletColor = new Color(1, 1, 0, 1);   // 노랑
    
    private Player player;
    private AbilityManager abilityManager;
    
    private string currentWeaponType = "Magenta";  // 기본 무기

    void Start()
    {
        player = FindFirstObjectByType<Player>();
        abilityManager = FindFirstObjectByType<AbilityManager>();
        
        if (player == null)
        {
            Debug.LogError("[WeaponUpgradeSystem] Player를 찾을 수 없음");
            return;
        }

        if (abilityManager == null)
        {
            Debug.LogError("[WeaponUpgradeSystem] AbilityManager를 찾을 수 없음");
            return;
        }

        // AbilityManager의 이벤트 구독
        abilityManager.OnAbilityApplied += HandleAbilityApplied;

        // 초기 무기 설정
        UpdateWeapon();

        Debug.Log("[WeaponUpgradeSystem] 초기화 완료");
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        if (abilityManager != null)
        {
            abilityManager.OnAbilityApplied -= HandleAbilityApplied;
        }
    }

    /// <summary>
    /// 능력 적용 후 호출되는 이벤트 핸들러
    /// AbilityManager.OnAbilityApplied에서 호출
    /// </summary>
    private void HandleAbilityApplied(AbilityData ability, Player targetPlayer)
    {
        // 무기 변경 확인
        UpdateWeapon();
    }

    /// <summary>
    /// 색깔 카운트를 비교하여 무기 변경
    /// 우선순위: Magenta > Cyan > Yellow
    /// </summary>
    public void UpdateWeapon()
    {
        if (player == null)
            return;

        // 1. 색깔 카운트 비교
        int magentaCount = player.magentaCount;
        int cyanCount = player.cyanCount;
        int yellowCount = player.yellowCount;

        Debug.Log($"[WeaponUpgradeSystem] 색깔 카운트: Magenta={magentaCount}, Cyan={cyanCount}, Yellow={yellowCount}");

        // 2. 우선순위에 따라 무기 결정
        string newWeaponType;

        if (magentaCount >= cyanCount && magentaCount >= yellowCount)
        {
            newWeaponType = "Magenta";
        }
        else if (cyanCount >= yellowCount)
        {
            newWeaponType = "Cyan";
        }
        else
        {
            newWeaponType = "Yellow";
        }

        // 3. 무기 변경 확인
        if (newWeaponType != currentWeaponType)
        {
            ChangeWeapon(newWeaponType);
        }
        else
        {
            Debug.Log($"[WeaponUpgradeSystem] 무기 유지: {currentWeaponType}");
        }
    }

    /// <summary>
    /// 실제 무기 변경 수행
    /// 무기 스프라이트와 탄막 색 변경
    /// </summary>
    private void ChangeWeapon(string weaponType)
    {
        currentWeaponType = weaponType;

        // 1. 무기 스프라이트 변경
        switch (weaponType)
        {
            case "Magenta":
                if (weaponSpriteRenderer != null && magentaWeaponSprite != null)
                    weaponSpriteRenderer.sprite = magentaWeaponSprite;
                
                // 탄막 색 변경
                ChangeBulletColor(magentaBulletColor);
                break;

            case "Cyan":
                if (weaponSpriteRenderer != null && cyanWeaponSprite != null)
                    weaponSpriteRenderer.sprite = cyanWeaponSprite;
                
                // 탄막 색 변경
                ChangeBulletColor(cyanBulletColor);
                break;

            case "Yellow":
                if (weaponSpriteRenderer != null && yellowWeaponSprite != null)
                    weaponSpriteRenderer.sprite = yellowWeaponSprite;
                
                // 탄막 색 변경
                ChangeBulletColor(yellowBulletColor);
                break;

            default:
                Debug.LogWarning($"[WeaponUpgradeSystem] 미지원 무기: {weaponType}");
                break;
        }

        Debug.Log($"[WeaponUpgradeSystem] 무기 변경: {weaponType}");
    }

    /// <summary>
    /// 탄막 색 변경
    /// 기존 탄막과 생성될 탄막의 색을 변경
    /// </summary>
    private void ChangeBulletColor(Color color)
    {
        // 방법 1: Bullet prefab의 색 변경 (추후 구현)
        // BulletManager bulletManager = FindFirstObjectByType<BulletManager>();
        // if (bulletManager != null)
        //     bulletManager.SetBulletColor(color);

        // 방법 2: 기존 탄막의 색 변경
        Bullet[] existingBullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in existingBullets)
        {
            SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
            if (bulletRenderer != null)
            {
                bulletRenderer.color = color;
            }
        }

        Debug.Log($"[WeaponUpgradeSystem] 탄막 색 변경: {color}");
    }

    /// <summary>
    /// 현재 무기 타입 반환
    /// </summary>
    public string GetCurrentWeapon()
    {
        return currentWeaponType;
    }

    /// <summary>
    /// 무기 변경 시뮬레이션 (디버그용)
    /// 특정 색깔의 카운트를 증가시켜서 테스트
    /// </summary>
    public void TestChangeWeapon(string targetColor)
    {
        if (player == null)
            return;

        switch (targetColor)
        {
            case "Magenta":
                player.magentaCount += 10;
                break;
            case "Cyan":
                player.cyanCount += 10;
                break;
            case "Yellow":
                player.yellowCount += 10;
                break;
        }

        UpdateWeapon();
        Debug.Log($"[WeaponUpgradeSystem] 테스트: {targetColor} 무기로 변경");
    }
}