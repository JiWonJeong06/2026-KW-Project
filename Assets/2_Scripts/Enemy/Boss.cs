using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : Enemy
{
    [Header("Boss Prefabs")]
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject water_drop_prefab;
    [SerializeField] private GameObject danger_zone_prefab;

    [Header("Shot Settings")]
    [SerializeField] private float pattern_delay     = 2.0f;  // 십자 → 대기 → X 사이 간격
    [SerializeField] private float aim_shot_interval = 3.0f;  // 조준 탄막 간격

    [SerializeField] private GameObject orbit_bullet_prefab;
    [SerializeField] private GameObject wave_prefab;

    [Header("Orbit Settings")]
    [SerializeField] private float orbit_radius     = 2f;    // 초기 반경
    [SerializeField] private float orbit_radius_min = 0.5f;  // 최소 반경
    [SerializeField] private float orbit_radius_max = 4.0f;  // 최대 반경
    [SerializeField] private float orbit_cycle      = 8f;    // 반경 변경 주기 (초)
    [SerializeField] private float orbit_speed      = 90f;   // 공전 속도 (도/초)

    [Header("Wave Settings")]
    [SerializeField] private float wave_cooldown    = 8f;
    [SerializeField] private float wave_spawn_inset = 3f;
    [SerializeField] private float wave_area_width  = 20f;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemy_prefabs      = new GameObject[3];
    [SerializeField] private float        spawn_area_width   = 20f;
    [SerializeField] private float        spawn_area_height  = 10f;
    [SerializeField] private float        boss_no_spawn_radius = 4f;

    [Header("Rain Settings")]
    [SerializeField] private float rain_danger_duration = 1.25f;
    [SerializeField] private int   rain_drop_count      = 5;
    [SerializeField] private float rain_total_duration  = 3f;
    [SerializeField] private float rain_area_size       = 3f;
    [SerializeField] private float rain_spawn_height    = 8f;
    [SerializeField] private float rain_cooldown        = 6f;

    private List<OrbitBullet> orbit_bullets = new List<OrbitBullet>();
    private bool orbit_phase_changed = false;

    // HP 단계 소환 플래그
    private bool spawned_75 = false;
    private bool spawned_50 = false;
    private bool spawned_25 = false;

    // 반경 관리 (Boss에서 통합 관리 → 모든 탄막 동일)
    private float orbit_current_radius = 2f;
    private float orbit_prev_radius    = 2f;
    private float orbit_target_radius  = 2f;
    private float orbit_cycle_timer    = 0f;

    protected override void Start()
    {
        base.Start();

        // 혹시 base.Start()에서 못 찾았으면 한 번 더 찾기
        if (player == null)
            player = FindAnyObjectByType<Player>();

        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.ShowBossHealthBar(this);

        StartCoroutine(RainPatternCoroutine());
        orbit_current_radius = orbit_radius;
        orbit_prev_radius    = orbit_radius;
        orbit_target_radius  = PickOrbitRadius(orbit_radius);
        CreateOrbitBullets(4);
        StartCoroutine(PatternShotCoroutine());
        StartCoroutine(AimShotCoroutine());
        StartCoroutine(WavePatternCoroutine());

        Debug.Log($"[Boss] 등장! player={player} / detection_range={detection_range}");
    }

    protected override void Update()
    {
        if (!is_alive) return;

        // 혹시 player가 null이면 매 프레임 다시 찾기
        if (player == null)
            player = FindAnyObjectByType<Player>();

        // HP 단계별 몹 소환
        CheckSpawnThresholds();

        // HP 50% 미만 → 공전 탄막 6개로 전환
        CheckOrbitPhase();

        // 공전 반경 업데이트 (모든 탄막 동일)
        UpdateOrbitRadius();

        // SafeZone 안에 있으면 감지 안 함
        if (SafeZone.Instance != null && SafeZone.Instance.IsPlayerInside())
        {
            is_detected = false;
            return;
        }

        // 거리로 감지
        if (player != null && player.IsAlive())
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            is_detected = dist <= detection_range;
        }
    }

    // ── 공전 탄막 ───────────────────────────────────────────────
    private void CreateOrbitBullets(int count)
    {
        if (orbit_bullet_prefab == null)
        {
            Debug.LogError("[Boss] orbit_bullet_prefab 미할당!");
            return;
        }

        // 기존 탄막 전부 제거
        foreach (var ob in orbit_bullets)
            if (ob != null) Destroy(ob.gameObject);
        orbit_bullets.Clear();

        // count개를 360/count 간격으로 균등 배치
        float step = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = i * step;
            GameObject obj = Instantiate(orbit_bullet_prefab,
                                         transform.position,
                                         Quaternion.identity);
            OrbitBullet ob = obj.GetComponent<OrbitBullet>();
            if (ob != null)
            {
                ob.Initialize(transform, angle, enemy_data.atk,
                              orbit_radius, orbit_speed);
                orbit_bullets.Add(ob);
            }
        }

        Debug.Log($"[Boss] 공전 탄막 {count}개 생성 (간격 {step}°, 반경 {orbit_radius})");
    }

    // ── HP 단계별 몹 소환 ──────────────────────────────────────
    private void CheckSpawnThresholds()
    {
        if (enemy_data == null) return;

        float ratio = current_hp / enemy_data.hp;

        if (!spawned_75 && ratio <= 0.75f)
        {
            spawned_75 = true;
            SpawnEnemies(2);
        }
        if (!spawned_50 && ratio <= 0.50f)
        {
            spawned_50 = true;
            SpawnEnemies(4);
        }
        if (!spawned_25 && ratio <= 0.25f)
        {
            spawned_25 = true;
            SpawnEnemies(8);
        }
    }

    private void SpawnEnemies(int count)
    {
        if (enemy_prefabs == null || enemy_prefabs.Length == 0)
        {
            Debug.LogError("[Boss] enemy_prefabs 미할당!");
            return;
        }

        int spawned      = 0;
        int max_attempts = count * 15;
        int attempt      = 0;

        while (spawned < count && attempt < max_attempts)
        {
            attempt++;

            float rx = Random.Range(-spawn_area_width  / 2f, spawn_area_width  / 2f);
            float ry = Random.Range(-spawn_area_height / 2f, spawn_area_height / 2f);
            Vector2 pos = new Vector2(rx, ry);

            // 보스 주변 제외
            if (Vector2.Distance(pos, transform.position) < boss_no_spawn_radius)
                continue;

            // 3종류 중 랜덤 (중복 가능)
            int idx = Random.Range(0, enemy_prefabs.Length);
            GameObject prefab = enemy_prefabs[idx];

            if (prefab == null)
            {
                Debug.LogWarning($"[Boss] enemy_prefabs[{idx}] null — Inspector 확인");
                continue;
            }

            Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            spawned++;
            Debug.Log($"[Boss] 몹 소환: {prefab.name} pos={pos}");
        }

        Debug.Log($"[Boss] {spawned}마리 소환 완료");
    }

    private void UpdateOrbitRadius()
    {
        if (orbit_bullets.Count == 0) return;

        orbit_cycle_timer += Time.deltaTime;
        float t = Mathf.Clamp01(orbit_cycle_timer / orbit_cycle);
        orbit_current_radius = Mathf.Lerp(orbit_prev_radius, orbit_target_radius, t);

        // 주기 완료 → 새 목표 랜덤
        if (orbit_cycle_timer >= orbit_cycle)
        {
            orbit_prev_radius   = orbit_target_radius;
            orbit_target_radius = PickOrbitRadius(orbit_prev_radius);
            orbit_cycle_timer   = 0f;
            Debug.Log($"[Boss] 공전 반경 새 목표: {orbit_target_radius:F1}");
        }

        // 모든 탄막에 동일한 반경 전달
        foreach (var ob in orbit_bullets)
            if (ob != null) ob.SetRadius(orbit_current_radius);
    }

    /// <summary>이전 값과 1.0 이상 차이나는 랜덤 반경 반환</summary>
    private float PickOrbitRadius(float prev)
    {
        float picked;
        int   safety = 0;
        do
        {
            picked = Random.Range(orbit_radius_min, orbit_radius_max);
            safety++;
        }
        while (Mathf.Abs(picked - prev) < 1.0f && safety < 20);

        return picked;
    }

    private void CheckOrbitPhase()
    {
        if (orbit_phase_changed || enemy_data == null) return;

        float ratio = current_hp / enemy_data.hp;
        if (ratio < 0.5f)
        {
            orbit_phase_changed = true;
            CreateOrbitBullets(6);
            Debug.Log("[Boss] HP 50% 미만 — 공전 탄막 6개로 전환!");
        }
    }

    // ── 십자 → 2초 → X → 십자 반복 ───────────────────────────────
    private IEnumerator PatternShotCoroutine()
    {
        while (is_alive)
        {
            if (!is_detected)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            ShootCrossPattern();
            yield return new WaitForSeconds(pattern_delay);

            if (!is_detected) continue;

            ShootXPattern();
            yield return new WaitForSeconds(pattern_delay);
        }
    }

    // ── 조준 탄막 (3초마다 플레이어 방향 1발) ─────────────────────
    private IEnumerator AimShotCoroutine()
    {
        while (is_alive)
        {
            yield return new WaitForSeconds(aim_shot_interval);

            if (is_detected && player != null)
            {
                Vector2 dir = ((Vector2)player.transform.position
                               - (Vector2)transform.position).normalized;
                ShootBullet(dir);
                Debug.Log($"[Boss] 조준 탄막!");
            }
        }
    }

    // ── 총알 유틸 ──────────────────────────────────────────────────
    private void ShootBullet(Vector2 direction)
    {
        if (bullet_prefab == null) return;
        GameObject bullet = Instantiate(bullet_prefab, transform.position, Quaternion.identity);
        EnemyBullet bs = bullet.GetComponent<EnemyBullet>();
        if (bs != null)
            bs.Initialize(direction, enemy_data.bullet_speed, enemy_data.atk);
    }

    private void ShootCrossPattern()
    {
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var d in dirs) ShootBullet(d);
        Debug.Log("[Boss] 십자 패턴!");
    }

    private void ShootXPattern()
    {
        Vector2[] dirs =
        {
            new Vector2( 1,  1).normalized,
            new Vector2(-1,  1).normalized,
            new Vector2(-1, -1).normalized,
            new Vector2( 1, -1).normalized
        };
        foreach (var d in dirs) ShootBullet(d);
        Debug.Log("[Boss] X 패턴!");
    }

    // ── 파도 패턴 ──────────────────────────────────────────────────
    private IEnumerator WavePatternCoroutine()
    {
        yield return new WaitForSeconds(5f);

        while (is_alive)
        {
            if (is_detected)
                yield return StartCoroutine(WaveAttack());

            yield return new WaitForSeconds(wave_cooldown);
        }
    }

    private IEnumerator WaveAttack()
    {
        if (wave_prefab == null)
        {
            Debug.LogWarning("[Boss] wave_prefab 미할당!");
            yield break;
        }

        // 방향 / Y좌표 미리 결정
        bool from_left = Random.value > 0.5f;
        float x = from_left
            ? -wave_area_width / 2f + wave_spawn_inset
            :  wave_area_width / 2f - wave_spawn_inset;

        float[] y_options = { 0f, 3f, -3f };
        float y = y_options[Random.Range(0, y_options.Length)];

        Vector2 wave_dir = from_left ? Vector2.right : Vector2.left;

        // 1. DangerZone 표시 — 파도가 지나갈 Y 라인 전체를 가로로 길게
        GameObject danger = null;
        if (danger_zone_prefab != null)
        {
            danger = Instantiate(danger_zone_prefab,
                                 new Vector3(0f, y, 0f),   // 맵 중앙 X, 파도 Y
                                 Quaternion.identity);

            // 가로로 길쭉하게 (맵 전체 너비 × 높이 1)
            danger.transform.localScale = new Vector3(wave_area_width, 1f, 1f);
            Debug.Log($"[Boss] 파도 DangerZone 표시: y={y}");
        }

        // 2. 1.25초 대기
        yield return new WaitForSeconds(rain_danger_duration);

        if (danger != null) Destroy(danger);

        // 3. 파도 생성
        GameObject wave_obj = Instantiate(wave_prefab,
                                          new Vector3(x, y, 0f),
                                          Quaternion.identity);
        WaveBullet wb = wave_obj.GetComponent<WaveBullet>();
        if (wb != null)
            wb.Initialize(wave_dir, enemy_data.bullet_speed, enemy_data.atk);

        Debug.Log($"[Boss] 파도 생성! pos=({x:F1}, {y}) 방향={wave_dir}");
    }

    private IEnumerator RainPatternCoroutine()
    {
        yield return new WaitForSeconds(3f);

        while (is_alive)
        {
            if (is_detected && player != null)
            {
                yield return StartCoroutine(RainAttack());
            }

            yield return new WaitForSeconds(rain_cooldown);
        }
    }

    private IEnumerator RainAttack()
    {
        if (water_drop_prefab == null)
        {
            Debug.LogError("[Boss] water_drop_prefab 미할당!");
            yield break;
        }

        Vector2 target_pos = player.transform.position;

        // DangerZone 표시
        GameObject danger = null;
        if (danger_zone_prefab != null)
        {
            danger = Instantiate(danger_zone_prefab,
                                 new Vector3(target_pos.x, target_pos.y, 0f),
                                 Quaternion.identity);
            danger.transform.localScale =
                new Vector3(rain_area_size * 2f, rain_area_size * 2f, 1f);
            Debug.Log("[Boss] DangerZone 표시!");
        }

        yield return new WaitForSeconds(rain_danger_duration);
        if (danger != null) Destroy(danger);

        // 물방울 낙하
        float interval = rain_drop_count > 1
            ? rain_total_duration / (rain_drop_count - 1)
            : 0f;
        float spawn_y = target_pos.y + rain_spawn_height;

        for (int i = 0; i < rain_drop_count; i++)
        {
            float land_x = target_pos.x + Random.Range(-rain_area_size, rain_area_size);
            float land_y = target_pos.y + Random.Range(-rain_area_size, rain_area_size);

            GameObject drop = Instantiate(water_drop_prefab,
                                          new Vector3(land_x, spawn_y, 0f),
                                          Quaternion.identity);
            WaterDrop wd = drop.GetComponent<WaterDrop>();
            if (wd != null)
                wd.Initialize(land_x, land_y, enemy_data.atk);

            Debug.Log($"[Boss] 물방울 {i + 1}/{rain_drop_count}");

            if (i < rain_drop_count - 1)
                yield return new WaitForSeconds(interval);
        }

        Debug.Log("[Boss] 물방울 완료");
    }

    protected override void Move() { }
    protected override void OnPlayerDetected() { }
    protected override void UpdateBehavior() { }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        // ── 보스 피격 사운드 ──
        SoundManager.Instance?.PlayBossHit();

        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.UpdateHealthBar(current_hp, enemy_data.hp);
    }

    protected override void Die()
    {
        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.HideBossHealthBar();

        // ── 보스 사망 사운드 ──
        SoundManager.Instance?.PlayBossDeath();

        Debug.Log("[Boss] 보스 처치!");

        // ── 승리 결산창 ──
        ResultUI.Instance?.ShowWin();

        base.Die();
    }
}