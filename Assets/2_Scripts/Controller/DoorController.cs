using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Info")]
    public DoorColor doorColor;
    public DoorTier doorTier;

    [Header("Door State")]
    public bool isOpen = false;

    [Header("Door Tilemap Objects")]
    [SerializeField] private GameObject tier1ClosedObject;
    [SerializeField] private GameObject tier1OpenObject;
    [SerializeField] private GameObject tier2ClosedObject;
    [SerializeField] private GameObject tier2OpenObject;
    [SerializeField] private GameObject tier3ClosedObject;
    [SerializeField] private GameObject tier3OpenObject;

    private RoomManager roomManager;

    public void Init(RoomManager manager, DoorColor color, DoorTier tier)
    {
        roomManager = manager;
        doorColor = color;
        doorTier = tier;
        isOpen = false;
        RefreshDoorView();
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        RefreshDoorView();
    }

    void RefreshDoorView()
    {
        HideAllDoorObjects();

        GameObject target = GetCurrentDoorObject();

        if (target != null)
            target.SetActive(true);
    }

    void HideAllDoorObjects()
    {
        if (tier1ClosedObject != null) tier1ClosedObject.SetActive(false);
        if (tier1OpenObject != null) tier1OpenObject.SetActive(false);
        if (tier2ClosedObject != null) tier2ClosedObject.SetActive(false);
        if (tier2OpenObject != null) tier2OpenObject.SetActive(false);
        if (tier3ClosedObject != null) tier3ClosedObject.SetActive(false);
        if (tier3OpenObject != null) tier3OpenObject.SetActive(false);
    }

    GameObject GetCurrentDoorObject()
    {
        switch (doorTier)
        {
            case DoorTier.Tier1:
                return isOpen ? tier1OpenObject : tier1ClosedObject;

            case DoorTier.Tier2:
                return isOpen ? tier2OpenObject : tier2ClosedObject;

            case DoorTier.Tier3:
                return isOpen ? tier3OpenObject : tier3ClosedObject;
        }

        return null;
    }

   
}