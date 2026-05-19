#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// AbilityIconDatabase를 자동으로 구성하는 에디터 스크립트
/// 
/// 역할:
/// 1. Assets/Resources/Abilities/Icons 폴더에서 스프라이트 자동 찾기
/// 2. 파일명(number)으로 자동 매핑
/// 3. Database에 308개 아이콘 자동 추가
/// 4. 메뉴: Tools → Ability → Rebuild Icon Database
/// </summary>
public class AbilityIconDatabaseEditor
{
    private const string DATABASE_PATH = "Assets/ScriptableObjects/AbilityIconDatabase.asset";
    private const string ICONS_FOLDER_PATH = "Assets/Resources/Abilities/Icons";

    /// <summary>
    /// 메뉴: Tools → Ability → Rebuild Icon Database
    /// </summary>
    [MenuItem("Tools/Ability/Rebuild Icon Database")]
    public static void RebuildIconDatabase()
    {
        Debug.Log("[AbilityIconDatabaseEditor] 아이콘 데이터베이스 재구성 시작...");

        // 1. Database 로드 또는 생성
        AbilityIconDatabase database = AssetDatabase.LoadAssetAtPath<AbilityIconDatabase>(DATABASE_PATH);

        if (database == null)
        {
            Debug.LogError($"[AbilityIconDatabaseEditor] Database를 찾을 수 없음: {DATABASE_PATH}");
            Debug.Log("[AbilityIconDatabaseEditor] Database를 생성하세요: Assets/ScriptableObjects/AbilityIconDatabase.asset");
            return;
        }

        // 2. 기존 아이콘 모두 제거
        database.ClearAllIcons();

        // 3. Icons 폴더에서 모든 스프라이트 찾기
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { ICONS_FOLDER_PATH });

        if (guids.Length == 0)
        {
            Debug.LogError($"[AbilityIconDatabaseEditor] Icons 폴더에 스프라이트가 없음: {ICONS_FOLDER_PATH}");
            return;
        }

        // 4. 각 스프라이트를 Database에 추가
        int addedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"[AbilityIconDatabaseEditor] 스프라이트 로드 실패: {path}");
                continue;
            }

            // 파일명에서 number 추출
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (int.TryParse(fileName, out int number))
            {
                database.AddIcon(number, sprite);
                addedCount++;
            }
            else
            {
                Debug.LogWarning($"[AbilityIconDatabaseEditor] 파일명이 숫자가 아님: {fileName}");
            }
        }

        // 5. Database 저장
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[AbilityIconDatabaseEditor] 아이콘 데이터베이스 재구성 완료: {addedCount}개");
        Debug.Log($"[AbilityIconDatabaseEditor] 총 {database.GetIconCount()}개 아이콘 저장됨");

        // 6. 결과 표시
        EditorUtility.DisplayDialog(
            "아이콘 데이터베이스 재구성 완료",
            $"{addedCount}개의 아이콘이 추가되었습니다.\n총 {database.GetIconCount()}개 아이콘",
            "확인"
        );
    }

    /// <summary>
    /// 메뉴: Tools → Ability → Clear Icon Database
    /// </summary>
    [MenuItem("Tools/Ability/Clear Icon Database")]
    public static void ClearIconDatabase()
    {
        AbilityIconDatabase database = AssetDatabase.LoadAssetAtPath<AbilityIconDatabase>(DATABASE_PATH);

        if (database == null)
        {
            Debug.LogError($"[AbilityIconDatabaseEditor] Database를 찾을 수 없음: {DATABASE_PATH}");
            return;
        }

        if (EditorUtility.DisplayDialog(
            "아이콘 데이터베이스 초기화",
            "모든 아이콘을 제거하시겠습니까?",
            "확인",
            "취소"
        ))
        {
            database.ClearAllIcons();
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();

            Debug.Log("[AbilityIconDatabaseEditor] 아이콘 데이터베이스 초기화 완료");
        }
    }

    /// <summary>
    /// 메뉴: Tools → Ability → Show Icon Database Info
    /// </summary>
    [MenuItem("Tools/Ability/Show Icon Database Info")]
    public static void ShowDatabaseInfo()
    {
        AbilityIconDatabase database = AssetDatabase.LoadAssetAtPath<AbilityIconDatabase>(DATABASE_PATH);

        if (database == null)
        {
            Debug.LogError($"[AbilityIconDatabaseEditor] Database를 찾을 수 없음: {DATABASE_PATH}");
            return;
        }

        int count = database.GetIconCount();
        Debug.Log($"[AbilityIconDatabaseEditor] 아이콘 데이터베이스 정보");
        Debug.Log($"  - 경로: {DATABASE_PATH}");
        Debug.Log($"  - 저장된 아이콘: {count}개");
        Debug.Log($"  - 목표: 308개");
        Debug.Log($"  - 진행률: {(float)count / 308 * 100:F1}%");

        if (count < 308)
        {
            Debug.LogWarning($"  - 아직 {308 - count}개 부족");
        }
    }

    /// <summary>
    /// 메뉴: Tools → Ability → Create Icon Database
    /// Database를 생성하지 않았을 때 사용
    /// </summary>
    [MenuItem("Tools/Ability/Create Icon Database")]
    public static void CreateIconDatabase()
    {
        // 폴더 확인/생성
        string folderPath = System.IO.Path.GetDirectoryName(DATABASE_PATH);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(
                System.IO.Path.GetDirectoryName(folderPath),
                System.IO.Path.GetFileName(folderPath)
            );
            AssetDatabase.Refresh();
        }

        // Database 생성
        AbilityIconDatabase database = ScriptableObject.CreateInstance<AbilityIconDatabase>();

        if (database == null)
        {
            Debug.LogError("[AbilityIconDatabaseEditor] Database 생성 실패");
            return;
        }

        AssetDatabase.CreateAsset(database, DATABASE_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[AbilityIconDatabaseEditor] 아이콘 데이터베이스 생성 완료: {DATABASE_PATH}");
        EditorUtility.DisplayDialog(
            "아이콘 데이터베이스 생성 완료",
            $"경로: {DATABASE_PATH}\n\n다음 단계:\n1. Assets/Resources/Abilities/Icons 폴더를 생성하세요\n2. 308개의 아이콘(1001.png~3203.png)을 추가하세요\n3. Tools → Ability → Rebuild Icon Database를 실행하세요",
            "확인"
        );
    }
}

#endif

///