using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Info")]
    public DoorColor doorColor;
    public DoorTier doorTier;

    [Header("Door State")]
    public bool isOpen = false;

    [Header("Tilemap")]
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private Vector3Int doorTilePosition;

    [Header("Closed Tiles")]
    [SerializeField] private TileBase tier1ClosedTile;
    [SerializeField] private TileBase tier2ClosedTile;
    [SerializeField] private TileBase tier3ClosedTile;

    [Header("Open Tiles")]
    [SerializeField] private TileBase tier1OpenTile;
    [SerializeField] private TileBase tier2OpenTile;
    [SerializeField] private TileBase tier3OpenTile;

    private RoomManager roomManager;

    public void Init(RoomManager manager, DoorColor color, DoorTier tier)
    {
        roomManager = manager;
        doorColor = color;
        doorTier = tier;
        isOpen = false;
        RefreshDoorTile();
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        RefreshDoorTile();
    }

    void RefreshDoorTile()
    {
        if (doorTilemap == null) return;

        TileBase tile = null;

        if (isOpen)
        {
            switch (doorTier)
            {
                case DoorTier.Tier1: tile = tier1OpenTile; break;
                case DoorTier.Tier2: tile = tier2OpenTile; break;
                case DoorTier.Tier3: tile = tier3OpenTile; break;
            }
        }
        else
        {
            switch (doorTier)
            {
                case DoorTier.Tier1: tile = tier1ClosedTile; break;
                case DoorTier.Tier2: tile = tier2ClosedTile; break;
                case DoorTier.Tier3: tile = tier3ClosedTile; break;
            }
        }

        doorTilemap.SetTile(doorTilePosition, tile);
    }

    public void Interact()
    {
        if (!isOpen)
        {
            Debug.Log($"{doorColor} 문은 아직 닫혀 있음");
            return;
        }

        if (roomManager != null)
        {
            roomManager.SelectDoor(this);
        }
    }
}