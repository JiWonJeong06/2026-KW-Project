using System;
using UnityEngine;

public static class JsonManager
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    public static T[] LoadArray<T>(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogError("JsonManager LoadArray 실패: TextAsset이 비어 있습니다.");
            return null;
        }

        string wrappedJson = "{\"array\":" + jsonFile.text + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);

        if (wrapper == null || wrapper.array == null)
        {
            Debug.LogError($"JsonManager LoadArray 실패: {jsonFile.name} 파싱 중 오류가 발생했습니다.");
            return null;
        }

        return wrapper.array;
    }

    public static string ToJsonArray<T>(T[] dataArray, bool prettyPrint = true)
    {
        if (dataArray == null)
        {
            Debug.LogError("JsonManager ToJsonArray 실패: dataArray가 null입니다.");
            return "[]";
        }

        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = dataArray;

        string json = JsonUtility.ToJson(wrapper, prettyPrint);

        int start = json.IndexOf('[');
        int end = json.LastIndexOf(']');

        if (start >= 0 && end >= 0)
            return json.Substring(start, end - start + 1);

        return "[]";
    }
}