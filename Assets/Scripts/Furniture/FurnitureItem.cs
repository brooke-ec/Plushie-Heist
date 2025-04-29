using cakeslice;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents an item, placed in the world or on in an inventory
/// </summary>
[RequireComponent(typeof(Collider))]
public class FurnitureItem : MonoBehaviour, IInteractable
{
    /// <summary> The name of this item displayed on the stock UI </summary>
    [field: SerializeField] public string itemName { get; private set; }
    /// <summary> The base value of this item </summary>
    [field: SerializeField] public int marketPrice { get; private set; }

    [field: Header("Placement Settings")]
    /// <summary> The size of this item in on the <see cref="FurnitureGrid"/> </summary>
    [field: SerializeField] public Vector2Int gridSize { get; private set; }
    /// <summary> The offset from default when placing on a <see cref="FurnitureGrid"/> </summary>
    [field: SerializeField] public Vector3 gridOffset { get; private set; }

    [field: Header("Inventory Settings")]
    /// <summary> The size of this item in the inventory </summary>
    [field: SerializeField] public Vector2Int inventorySize { get; private set; }
    /// <summary> The icon to use for this item in the inventory </summary>
    [field: SerializeField] public Sprite inventoryIcon { get; private set; }

    /// <summary> Prompt Shown by the UI to let the player know they can interact with it </summary>
    public string interactionPrompt => empty ? (hasSpace ? "Press F to Pick Up" : "Inventory Full")
        + "\nPress R to " + (selling ? "Unmark" : "Mark") + " as Selling" : "Item Contains Sub-Items";
    /// <summary> Whether this item can be picked up or not </summary>
    public bool canPickup => hasSpace && empty;
    /// <summary> If there is space in the players inventory </summary>
    public bool hasSpace { get; private set; } = true;
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
    /// <summary> The region representing this item's current placement on <see cref="grid"/> </summary>
    public Region gridRegion => new Region().FromSize(gridPosition, gridSize);

    /// <summary> The current <see cref="InventoryController"/> instance </summary>
    private InventoryController inventoryController;
    /// <summary> The outline script attached to this object </summary>
    private Outline outline;
    /// <summary> A <see cref="MaterialSwitcher"/> instance for swapping the material of this item </summary>
    private MaterialSwitcher switcher;
    /// <summary> The marker to denote that this item is being sold </summary>
    private GameObject sellingMarker;
    /// <summary> Whether this item is marked as sellable or not </summary>
    private bool selling => sellingMarker.activeSelf;

    // Temporary workaround
    // TODO: Refactor
    [HideInInspector] public FurnitureItem source;

    private void Awake()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        subgrids = GetComponentsInChildren<FurnitureGrid>();
        outline = GetComponentInChildren<Outline>();
        switcher = new MaterialSwitcher(gameObject);
    }

    private void Start()
    {
        PlaceSellingMarker();
        outline.enabled = false;

        inventoryController.onChanged.AddListener(() => {
            hasSpace = inventoryController.CanInsert(this);
        });

        foreach (FurnitureGrid grid in subgrids) grid.onChanged.AddListener(() =>
        {
            empty = subgrids.All(s => s.IsEmpty());
        });
    }

    private void Update()
    {
        outline.color = canPickup ? 0 : 1;
    }

    /// <summary>
    /// Create and place the selling marker on this item.
    /// </summary>
    public void PlaceSellingMarker()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        sellingMarker = Instantiate(FurnitureSettings.instance.defaultSellingMarker, transform);
        sellingMarker.transform.position += new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        sellingMarker.SetActive(false);
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    /// <returns>True if picked up item<returns>
    public bool PrimaryInteract(Interactor interactor)
    {
        if (!canPickup) return false;

        if (inventoryController.InsertItem(source))
        {
            if (grid != null) grid.RemoveItem(this);

            Destroy(gameObject);
            Debug.Log("Picked Up" + gameObject.name);
            return true;
        }
        else { 
            Debug.Log("Can't Pick up"+gameObject.name);
            return false;
        }
        
    }

    public bool SecondaryInteract(Interactor interactor)
    {
        if (!empty) return false;

        subgrids.ForEach(s => s.gameObject.SetActive(selling));
        sellingMarker.SetActive(!selling);
        return true;
    }

    /// <summary>
    /// Rotates this item by 90 degrees
    /// </summary>
    public void GridRotate()
    {
        gridSize = new Vector2Int(gridSize.y, gridSize.x);
        transform.Rotate(0, 90, 0);
        GridMove(gridPosition);
    }

    /// <summary>
    /// Moves this item as close to <paramref name="target"/> as possible
    /// </summary>
    public void GridMove(Vector2Int target)
    {
        gridPosition = Util.Clamp(target, Vector2Int.zero, grid.size - gridSize);
        transform.position = grid.ToWorldspace(gridRegion.center) - transform.rotation * gridOffset;

        if (IsGridValid()) switcher.Reset();
        else switcher.Switch(FurnitureSettings.instance.invalidMaterial);
    }

    /// <returns>True if this item is currently in a valid grid position</returns>
    public bool IsGridValid()
    {
        return grid != null && gridPosition != null && gridRegion.Within(grid.size) && !grid.Intersects(gridRegion);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + gridOffset, new Vector3(gridSize.x, 0, gridSize.y) * FurnitureSettings.instance.cellSize);
    }
#endif
}
