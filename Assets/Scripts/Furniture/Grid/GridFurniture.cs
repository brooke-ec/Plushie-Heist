using cakeslice;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an item, placed in the world or on in an inventory
/// </summary>
[RequireComponent(typeof(Collider))]
public class GridFurniture : MonoBehaviour
{
    /// <summary> Prompt Shown by the UI to let the player know they can interact with it </summary>
    //public string interactionPrompt => empty ? (hasSpace ? "Press F to Pick Up" : "Inventory Full")
    //    + "\nPress R to " + (selling ? "Unmark" : "Mark") + " as Selling" : "Item Contains Sub-Items";
    /// <summary> Whether this item can be picked up or not </summary>
    //public bool canPickup => hasSpace && empty;

    /// <summary> Whether the subgrids of this item are empty </summary>
    public bool empty { get; private set; } = true;
    /// <summary> Any <see cref="FurnitureGrid"/>s attached to children </summary>
    public FurnitureGrid[] subgrids { get; private set; }
    /// <summary> The grid-position of this item on <see cref="grid"/></summary>
    public Vector2Int gridPosition { get; private set; }
    /// <summary> The current Y euler rotation of this object </summary>
    public int gridRotation => Mathf.RoundToInt(transform.rotation.eulerAngles.y);
    /// <summary> The <see cref="FurnitureGrid"/> this item is currently attached to</summary>
    public FurnitureGrid grid { get; set; }
    /// <summary> Whether this item is currently placed on a grid </summary>
    public bool placed => grid != null;
    /// <summary> The region representing this item's current placement on <see cref="grid"/> </summary>
    public Region gridRegion => new Region().FromSize(gridPosition, shape);
    /// <summary> Whether this item is marked as sellable or not </summary>
    public bool selling => sellingMarker.activeSelf && placed;
    /// <summary Whether this item can be marked as sellable </summary>
    public bool canSell => empty;
    /// <summary> The world space bounding volume of this item </summary>
    public Bounds bounds => collider.bounds;
    /// <summary> The shape of this furniture item </summary>
    public Vector2Int shape {  get; private set; }

    public bool interactable => canSell;

    /// <summary> The current <see cref="InventoryController"/> instance </summary>
    private InventoryController inventoryController;
    /// <summary> A <see cref="MaterialSwitcher"/> instance for swapping the material of this item </summary>
    private MaterialSwitcher switcher;
    /// <summary> The marker to denote that this item is being sold </summary>
    private GameObject sellingMarker;
    /// <summary> The collider attached to this item </summary>
    private new Collider collider;

    private void Awake()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        subgrids = GetComponentsInChildren<FurnitureGrid>();
        switcher = new MaterialSwitcher(gameObject);
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        PlaceSellingMarker();
        GetComponentInChildren<Outline>().enabled = false;

        foreach (FurnitureGrid grid in subgrids) grid.onChanged.AddListener(() =>
        {
            empty = subgrids.All(s => s.IsEmpty());
        });
    }

    /// <summary>
    /// Create and place the selling marker on this item.
    /// </summary>
    private void PlaceSellingMarker()
    {
        sellingMarker = Instantiate(FurnitureSettings.instance.defaultSellingMarker, transform);
        sellingMarker.transform.position += new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        sellingMarker.SetActive(false);
    }

    public void SecondaryInteract(Interactor interactor)
    {
        if (!canSell) return;

        subgrids.ForEach(s => s.gameObject.SetActive(selling));
        sellingMarker.SetActive(!selling);
    }

    /// <summary>
    /// Rotates this item by 90 degrees
    /// </summary>
    public void GridRotate()
    {
        shape = new Vector2Int(shape.y, shape.x);
        transform.Rotate(0, 90, 0);
        GridMove(gridPosition);
    }

    /// <summary>
    /// Moves this item as close to <paramref name="target"/> as possible
    /// </summary>
    public void GridMove(Vector2Int target)
    {
        gridPosition = Util.Clamp(target, Vector2Int.zero, grid.size - shape);
        //transform.position = grid.ToWorldspace(gridRegion.center) - transform.rotation * gridOffset;

        if (IsGridValid()) switcher.Reset();
        else switcher.Switch(FurnitureSettings.instance.invalidMaterial);
    }

    /// <returns>True if this item is currently in a valid grid position</returns>
    public bool IsGridValid()
    {
        return grid != null && gridPosition != null && gridRegion.Within(grid.size) && !grid.Intersects(gridRegion);
    }
}
