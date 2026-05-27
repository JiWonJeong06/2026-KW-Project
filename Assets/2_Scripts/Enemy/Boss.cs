using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Enemy
{
    // ─────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────
    [Header("Boss Prefabs")]
    [SerializeField] private GameObject orbit_bullet_prefab;
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject water_drop_prefab;   // 물방울 프리팹
    [SerializeField] private GameObject danger_zone_prefab;  // DangerZone 표시 프리팹
    [SerializeField] private GameObject wave_horizontal_prefab; // 가로 파도 프리팹
    [SerializeField] private GameObject wave_vertical_prefab;   // 세로 파도 프리팹

    [Header("Orbit Settings")]
    [SerializeField] private float orbit_radius_min = 1.5f;  // 공전 최소 반경
    [SerializeField] private float orbit_radius_max = 3.0f;  // 공전 최대 반경
    [SerializeField] private float orbit_radius_speed = 0.5f; // 반경 변화 속도

    [Header("Rain Settings")]
    [SerializeField] private float rain_danger_duration = 1.5f;  // DangerZone 표시 시간
    [SerializeField] private float rain_drop_interval = 0.08f;   // 물방울 낙하 간격
    [SerializeField] private int   rain_drop_count = 12;         // 물방울 개수
    [SerializeField] private float rain_area_size = 3f;          // 3×3 반경
    [SerializeField] private float rain_cooldown = 6f;           // 물방울 패턴 쿨다운

    [Header("Wave Settings")]
    [SerializeField] private float wave_cooldown = 8f;           // 파도 패턴 쿨다운

    [Header("Spawn Settings")]
    [SerializeField] private float spawn_area_width  = 20f;
    [SerializeField] private float spawn_area_height = 10f;
    [SerializeField] private float boss_no_spawn_radius = 4f;    // 보스 주변 소환 금지 반경

    // ─────────────────────────────────────────
    // 내부 상태
    // ─────────────────────────────────────────
    private List<OrbitBullet> orbit_bullets = new List<OrbitBullet>();

    // HP 단계별 일반 몹 소환 플래그
    private bool spawned_75 = false;
    private bool spawned_50 = false;
    private bool spawned_25 = false;

    // 50% 이하일 때 추가 공전 탄막 생성 여부
    private bool added_extra_orbits = false;

    // ─────────────────────────────────────────
    // Init
    // ─────────────────────────────────────────
    protected override void Start()
    {
        base.Start();

        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.ShowBossHealthBar(this);

        CreateOrbitBullets(4);

        StartCoroutine(AttackPatternCoroutine());
        StartCoroutine(RainPatternCoroutine());
        StartCoroutine(WavePatternCoroutine());

        Debug.Log($"[Boss] 등장! HP: {current_hp}");
    }

    protected override void Update()
    {
        // 고정형 보스 – 이동 없음
        // HP 단계 체크
        CheckHpThresholds();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HP 단계 체크
    // ─────────────────────────────────────────────────────────────────────────
    private void CheckHpThresholds()
    {
        if (!is_alive || enemy_data == null) return;

        float ratio = current_hp / enemy_data.hp;

        // 75% → 2마리
        if (!spawned_75 && ratio <= 0.75f)
        {
            spawned_75 = true;
            SpawnEnemies(2);
        }

        // 50% → 4마리 + 공전 탄막 2개 추가
        if (!spawned_50 && ratio <= 0.50f)
        {
            spawned_50 = true;
            SpawnEnemies(4);

            if (!added_extra_orbits)
            {
                added_extra_orbits = true;
                CreateOrbitBullets(2); // 추가 2개
                Debug.Log("[Boss] HP 50% - 공전 탄막 2개 추가!");
            }
        }

        // 25% → 8마리
        if (!spawned_25 && ratio <= 0.25f)
        {
            spawned_25 = true;
            SpawnEnemies(8);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 공전 탄막 생성
    // ─────────────────────────────────────────────────────────────────────────
    private void CreateOrbitBullets(int count)
    {
        if (orbit_bullet_prefab == null)
        {
            Debug.LogError("[Boss] OrbitBullet Prefab 미할당!");
            return;
        }

        int current_count = orbit_bullets.Count;
        float angle_offset = current_count > 0 ? (360f / (current_count + count)) * current_count : 0f;

        for (int i = 0; i < count; i++)
        {
            float start_angle = angle_offset + i * (360f / (current_count + count));

            GameObject obj = Instantiate(orbit_bullet_prefab, transform.position, Quaternion.identity, transform);
            OrbitBullet ob = obj.GetComponent<OrbitBullet>();

            if (ob != null)
            {
                ob.Initialize(transform, start_angle, enemy_data.atk,
                              orbit_radius_min, orbit_radius_max, orbit_radius_speed);
                orbit_bullets.Add(ob);
            }
        }

        Debug.Log($"[Boss] 공전 탄막 {count}개 생성 (총 {orbit_bullets.Count}개)");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 기본 공격 패턴 코루틴 (십자 / X 교대)
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator AttackPatternCoroutine()
    {
        while (is_alive)
        {
            if (IsPlayerInSafeZone()) { yield return new WaitForSeconds(0.1f); continue; }

            ShootCrossPattern();
            yield return new WaitForSeconds(enemy_data.atk_speed);

            if (IsPlayerInSafeZone()) { yield return new WaitForSeconds(0.1f); continue; }

            ShootXPattern();
            yield return new WaitForSeconds(enemy_data.atk_speed);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 물방울 패턴 코루틴
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator RainPatternCoroutine()
    {
        // 보스 등장 직후 약간의 딜레이
        yield return new WaitForSeconds(3f);

        while (is_alive)
        {
            if (!IsPlayerInSafeZone() && player != null)
            {
                yield return StartCoroutine(RainAttack());
            }

            yield return new WaitForSeconds(rain_cooldown);
        }
    }

    private IEnumerator RainAttack()
    {
        // 1. 플레이어 위치 기준으로 DangerZone 표시
        Vector2 target_pos = player.transform.position;

        GameObject danger = null;
        if (danger_zone_prefab != null)
        {
            danger = Instantiate(danger_zone_prefab,
                                 new Vector3(target_pos.x, target_pos.y, 0f),
                                 Quaternion.identity);

            // DangerZone 크기를 rain_area_size에 맞춤 (3×3)
            danger.transform.localScale = new Vector3(rain_area_size * 2f, rain_area_size * 2f, 1f);
        }

        Debug.Log($"[Boss] 물방울 DangerZone 표시: {target_pos}");

        // 2. 경고 시간 대기
        yield return new WaitForSeconds(rain_danger_duration);

        if (danger != null)
            Destroy(danger);

        // 3. 물방울 우수수 낙하
        if (water_drop_prefab != null)
        {
            for (int i = 0; i < rain_drop_count; i++)
            {
                // 3×3 반경 내 랜덤 위치
                float rx = target_pos.x + Random.Range(-rain_area_size, rain_area_size);
                float ry = target_pos.y + Random.Range(-rain_area_size, rain_area_size);

                // 화면 위에서 낙하 시작 위치
                float spawn_y = target_pos.y + 8f;
                Vector3 spawn_pos = new Vector3(rx, spawn_y, 0f);
                Vector3 land_pos  = new Vector3(rx, ry, 0f);

                GameObject drop = Instantiate(water_drop_prefab, spawn_pos, Quaternion.identity);
                WaterDrop wd = drop.GetComponent<WaterDrop>();
                if (wd != null)
                {
                    wd.Initialize(land_pos, enemy_data.atk);
                }

                yield return new WaitForSeconds(rain_drop_interval);
            }
        }

        Debug.Log("[Boss] 물방울 낙하 완료");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 파도 패턴 코루틴
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator WavePatternCoroutine()
    {
        yield return new WaitForSeconds(5f);

        while (is_alive)
        {
            if (!IsPlayerInSafeZone())
            {
                ShootWave();
            }

            yield return new WaitForSeconds(wave_cooldown);
        }
    }

    private void ShootWave()
    {
        // 가로 or 세로 랜덤
        bool is_horizontal = Random.value > 0.5f;

        GameObject prefab = is_horizontal ? wave_horizontal_prefab : wave_vertical_prefab;

        if (prefab == null)
        {
            Debug.LogWarning("[Boss] 파도 프리팹 미할당!");
            return;
        }

        // 가로 파도: 왼쪽 또는 오른쪽에서 시작
        // 세로 파도: 위 또는 아래에서 시작
        Vector3 spawn_pos;
        Vector2 wave_dir;

        if (is_horizontal)
        {
            bool from_left = Random.value > 0.5f;
            float x = from_left ? -spawn_area_width / 2f - 1f : spawn_area_width / 2f + 1f;
            float y = player != null ? player.transform.position.y : 0f;
            spawn_pos = new Vector3(x, y, 0f);
            wave_dir  = from_left ? Vector2.right : Vector2.left;
        }
        else
        {
            bool from_top = Random.value > 0.5f;
            float y = from_top ? spawn_area_height / 2f + 1f : -spawn_area_height / 2f - 1f;
            float x = player != null ? player.transform.position.x : 0f;
            spawn_pos = new Vector3(x, y, 0f);
            wave_dir  = from_top ? Vector2.down : Vector2.up;
        }

        GameObject wave_obj = Instantiate(prefab, spawn_pos, Quaternion.identity);
        WaveBullet wb = wave_obj.GetComponent<WaveBullet>();
        if (wb != null)
        {
            wb.Initialize(wave_dir, enemy_data.bullet_speed, enemy_data.atk);
        }

        Debug.Log($"[Boss] 파도 발사! 방향: {wave_dir}, 가로여부: {is_horizontal}");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 일반 몹 소환
    // ─────────────────────────────────────────────────────────────────────────
    private void SpawnEnemies(int count)
    {
        int[] mob_ids = new int[] { 1001, 1002, 1003 };
        int spawned = 0;
        int max_attempts = count * 10;
        int attempt = 0;

        while (spawned < count && attempt < max_attempts)
        {
            attempt++;

            float rx = Random.Range(-spawn_area_width  / 2f, spawn_area_width  / 2f);
            float ry = Random.Range(-spawn_area_height / 2f, spawn_area_height / 2f);
            Vector2 pos = new Vector2(rx, ry);

            // 보스 주변 제외
            if (Vector2.Distance(pos, transform.position) < boss_no_spawn_radius)
                continue;

            // 랜덤 ID 선택
            int id = mob_ids[Random.Range(0, mob_ids.Length)];

            // EnemySpawner 또는 직접 Instantiate 방식 선택
            // 현재 프로젝트에 Spawner가 있다면 아래 주석 해제 후 사용:
            // EnemySpawner.Instance.SpawnEnemy(id, pos);

            // 임시: 씬에서 Enemy 프리팹을 Resources로 로드
            string prefab_path = $"Enemies/{id}";
            GameObject enemy_prefab = Resources.Load<GameObject>(prefab_path);

            if (enemy_prefab == null)
            {
                Debug.LogWarning($"[Boss] Enemy 프리팹 없음: Resources/{prefab_path}");
                // 소환 실패해도 카운트는 올림 (무한루프 방지)
                spawned++;
                continue;
            }

            GameObject enemy_obj = Instantiate(enemy_prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            Enemy enemy_comp = enemy_obj.GetComponent<Enemy>();
            if (enemy_comp != null)
            {
                enemy_comp.Initialize(id);
            }

            spawned++;
        }

        Debug.Log($"[Boss] 일반 몹 {spawned}마리 소환 완료!");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 총알 발사 유틸
    // ─────────────────────────────────────────────────────────────────────────
    private void ShootBullet(Vector2 direction)
    {
        if (bullet_prefab == null) return;

        GameObject bullet = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
        EnemyBullet bs = bullet.GetComponent<EnemyBullet>();
        if (bs != null)
        {
            bs.Initialize(direction, enemy_data.bullet_speed, enemy_data.atk);
        }
    }

    private void ShootCrossPattern()
    {
        Debug.Log("[Boss] 십자 패턴 발사!");
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var d in dirs) ShootBullet(d);
    }

    private void ShootXPattern()
    {
        Debug.Log("[Boss] X 패턴 발사!");
        Vector2[] dirs =
        {
            new Vector2( 1,  1).normalized,
            new Vector2(-1,  1).normalized,
            new Vector2(-1, -1).normalized,
            new Vector2( 1, -1).normalized
        };
        foreach (var d in dirs) ShootBullet(d);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 유틸
    // ─────────────────────────────────────────────────────────────────────────
    private bool IsPlayerInSafeZone()
    {
        return SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Enemy abstract 구현
    // ─────────────────────────────────────────────────────────────────────────
    protected override void Move() { }
    protected override void OnPlayerDetected() { }
    protected override void UpdateBehavior() { }

    // ─────────────────────────────────────────────────────────────────────────
    // 피격 / 사망
    // ─────────────────────────────────────────────────────────────────────────
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.UpdateHealthBar(current_hp, enemy_data.hp);

        Debug.Log($"[Boss] 피격! 남은 HP: {current_hp}/{enemy_data.hp}");
    }

    protected override void Die()
    {
        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.HideBossHealthBar();

        Debug.Log("[Boss] 보스 처치!");
        // GameManager.Instance.OnBossDefeated();

        base.Die();
    }
}