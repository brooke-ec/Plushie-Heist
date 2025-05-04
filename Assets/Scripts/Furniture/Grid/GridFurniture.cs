using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an item, placed in the world or on in an inventory
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(FurnitureSource))]
public class GridFurniture : MonoBehaviour, IInteractable
{
    /// <summary> Prompt Shown by the UI to let the player know they can interact with it </summary>
    string IInteractable.interactionPrompt => empty ? "Press R to " + (selling? "Unmark" : "Mark") + " as Selling" : "Item Contains Sub-Items";
    /// <summary> Whether this the interable outline should be red </summary>
    bool IInteractable.outline => canSell;
    /// <summary> Whether the subgrids of this item are empty </summary>
    public bool empty { get; private set; } = true;
    /// <summary> Any <see cref="FurnitureGrid"/>s attached to children </summary>
    public FurnitureGrid[] subgrids { get; private set; }
    /// <summary> The grid-position of this item on <see cref="grid"/></summary>
    public Vector2Int position { get; private set; }
    /// <summary> The current Y euler rotation of this object </summary>
    public int rotation => Mathf.RoundToInt(transform.rotation.eulerAngles.y);
    /// <summary> The <see cref="FurnitureGrid"/> this item is currently attached to</summary>
    public FurnitureGrid grid { get; set; }
    /// <summary> Whether this item is currently placed on a grid </summary>
    public bool placed => grid != null;
    /// <summary> The region representing this item's current placement on <see cref="grid"/> </summary>
    public Region region => new Region().FromSize(position, shape);
    /// <summary> Whether this item is marked as sellable or not </summary>
    public bool selling => sellingMarker.activeSelf && placed;
    /// <summary Whether this item can be marked as sellable </summary>
    public bool canSell => empty;
    /// <summary> The world space bounding volume of this item </summary>
    public Bounds bounds => collider.bounds;
    /// <summary> The shape of this furniture item </summary>
    public Vector2Int shape {  get; private set; }
    /// <summary> The furniture controller attached to this object </summary>
    public FurnitureSource controller { get; private set; }
    /// <summary> The item this furniture instance represents </summary>
    public FurnitureItem item => controller.item;

    /// <summary> A <see cref="MaterialSwitcher"/> instance for swapping the material of this item </summary>
    private MaterialSwitcher switcher;
    /// <summary> The marker to denote that this item is being sold </summary>
    private GameObject sellingMarker;
    /// <summary> The collider attached to this item </summary>
    private new Collider collider;

    private void Awake()
    {
        subgrids = GetComponentsInChildren<FurnitureGrid>();
        controller = GetComponent<FurnitureSource>();
        switcher = new MaterialSwitcher(gameObject);
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        PlaceSellingMarker();
        shape = item.gridSize;
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
    public void Rotate()
    {
        shape = new Vector2Int(shape.y, shape.x);
        transform.Rotate(0, 90, 0);
        Move(position);
    }

    /// <summary>
    /// Moves this item as close to <paramref name="target"/> as possible
    /// </summary>
    public void Move(Vector2Int target)
    {
        position = Util.Clamp(target, Vector2Int.zero, grid.size - shape);
        transform.position = grid.ToWorldspace(region.center) - transform.rotation * item.gridOffset;

        if (IsValid()) switcher.Reset();
        else switcher.Switch(FurnitureSettings.instance.invalidMaterial);
    }

    /// <returns>True if this item is currently in a valid grid position</returns>
    public bool IsValid()
    {
        return grid != null && position != null && region.Within(grid.size) && !grid.Intersects(region);
    }

    private void OnDestroy()
    {
        if (grid != null) grid.RemoveItem(this);
    }
}
