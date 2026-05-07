using UnityEngine;

[System.Serializable]
public class CutSceneFrame
{
    public Sprite image;

    [TextArea(3, 5)]
    public string narration;
}