using UnityEngine;

/// <summary>
/// 펫 데이터를 JSON에서 로드하고 적용하는 클래스
/// 역할:
/// - Pet_DataTable.json 파일 로드
/// - PetData 파싱
/// - Pet, HomingBullet에 데이터 적용
/// 
/// 주의: HomingBullet은 펫 총알이므로 여기서 처리됨
/// </summary>
public class PetDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private Pet pet;
    [SerializeField] private HomingBullet homingBullet;

    void Awake()
    {
        // 1. JSON 파일에서 데이터 로드
        if (jsonFile == null)
        {
            Debug.LogError("Pet_DataTable.json 파일이 할당되지 않음");
            return;
        }

        PetData data = JsonUtility.FromJson<PetData>(jsonFile.text);

        if (data == null)
        {
            Debug.LogError("PetData 파싱 실패");
            return;
        }

        // 2. Pet에 데이터 적용
        if (pet != null)
        {
            pet.ApplyData(data);
            Debug.Log($"Pet 데이터 적용: {data.name}");
        }
        else
        {
            Debug.LogWarning("Pet이 할당되지 않음");
        }

        // 3. HomingBullet(펫 총알)에 데이터 적용
        if (homingBullet != null)
        {
            homingBullet.ApplyBulletData(data);
            Debug.Log("HomingBullet 데이터 적용");
        }
        else
        {
            Debug.LogWarning("HomingBullet이 할당되지 않음");
        }
    }
}