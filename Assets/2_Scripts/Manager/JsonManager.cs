using UnityEngine;

public static class JsonManager
{
    public static T[] LoadArray<T>(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일이 비어 있습니다.");
            return null;
        }

        string json = jsonFile.text;

        JsonArrayWrapper<T> wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(json);

        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON 배열 변환 실패");
            return null;
        }

        return wrapper.items;
    }

    [System.Serializable]
    private class JsonArrayWrapper<T>
    {
        public T[] items;
    }
}