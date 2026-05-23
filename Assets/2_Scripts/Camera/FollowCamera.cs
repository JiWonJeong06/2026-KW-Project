using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("타겟 설정")]
    [SerializeField] private Transform target; // 플레이어

    [Header("카메라 설정")]
    [SerializeField] private float smooth_speed = 5f; // 부드러운 이동 속도
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // 카메라 오프셋

    [Header("경계 제한 (옵션)")]
    [SerializeField] private bool use_bounds = false;
    [SerializeField] private float min_x = -10f;
    [SerializeField] private float max_x = 10f;
    [SerializeField] private float min_y = -5f;
    [SerializeField] private float max_y = 5f;

    private void FixedUpdate()
    {
        if (target == null) return;

        // 목표 위치 = 타겟 위치 + 오프셋
        Vector3 desired_position = target.position + offset;

        // 경계 제한 적용
        if (use_bounds)
        {
            desired_position.x = Mathf.Clamp(desired_position.x, min_x, max_x);
            desired_position.y = Mathf.Clamp(desired_position.y, min_y, max_y);
        }

        // 부드럽게 이동
        Vector3 smoothed_position = Vector3.Lerp(transform.position, desired_position, smooth_speed * Time.deltaTime);

        // Z축 고정 (2D 게임)
        smoothed_position.z = offset.z;

        transform.position = smoothed_position;
    }

    // 타겟을 동적으로 설정하는 메서드 (옵션)
    public void SetTarget(Transform new_target)
    {
        target = new_target;
    }

    // 기즈모로 경계 표시 (씬 뷰에서 확인용)
    private void OnDrawGizmosSelected()
    {
        if (!use_bounds) return;

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((min_x + max_x) / 2f, (min_y + max_y) / 2f, 0f);
        Vector3 size = new Vector3(max_x - min_x, max_y - min_y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}