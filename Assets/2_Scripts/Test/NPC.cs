using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Outline")]
    [SerializeField] private GameObject outlineObject;

    public void Interact()
    {
        Debug.Log("Door와 상호작용");
    }

    public void SetHighlight(bool isOn)
    {
        if (outlineObject != null)
            outlineObject.SetActive(isOn);
    }
}