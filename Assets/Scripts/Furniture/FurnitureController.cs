using cakeslice;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AI;
using Newtonsoft.Json;
using static UnityEngine.GraphicsBuffer;

public class FurnitureController : MonoBehaviour, IInteractable
{
    /// <summary> The item this prefab represents </summary>
    [JsonProperty("item"), Unwitable] public FurnitureItem item = null;
    /// <summary> Prompt Shown by the UI to let the player know they can interact with it </summary>
    string IInteractable.interactionPrompt => empty 
        ? (selling ? "Press F to set Price" : (hasSpace ? "Press F to Pick Up" : "Inventory Full"))
        + (canSell ? "\nPress R to " + (selling ? "Unmark" : "Mark") + " as Selling" : "")
        : "Item Contains Sub-Items";
    bool IInteractable.outline => canPickup || canSell;
    /// <summary> Whether this item can be picked up or not </summary>
    public bool canPickup => hasSpace && empty;
    /// <summary> If there is space in the players inventory </summary>
    public bool hasSpace { get; private set; } = true;
    /// <summary> Whether the subgrids of this item are empty </summary>
    public bool empty { get; private set; } = true;
    /// <summary> Any <see cref="FurnitureGrid"/>s attached to children </summary>
    [JsonProperty("subgrids"), Populate] public FurnitureGrid[] subgrids { get; private set; }
    /// <summary> The grid-position of this item on <see cref="grid"/></summary>
    [JsonProperty("position")] public Vector2Int gridPosition { get; private set; }
    /// <summary> The current Y euler rotation of this object </summary>
    [JsonProperty("rotation")] public int gridRotation { 
        get { return Mathf.RoundToInt(transform.rotation.eulerAngles.y); } 
        set { transform.rotation = Quaternion.Euler(transform.eulerAngles.x, value, transform.eulerAngles.z); }
    }
    /// <summary> The <see cref="FurnitureGrid"/> this item is currently attached to</summary>
    public FurnitureGrid grid { get; set; }
    /// <summary> Whether this item is currently placed on a grid </summary>
    public bool placed => grid != null;
    /// <summary> The region representing this item's current placement on <see cref="grid"/> </summary>
    public Region gridRegion => new Region().FromSize(gridPosition, gridShape);
    /// <summary> Whether this item is marked as sellable or not </summary>
    [JsonProperty("selling")] public bool selling { get; private set; } = false;
    /// <summary> The shape of this furniture item </summary>
    public Vector2Int gridShape => gridRotation % 180 < 90 ? item.gridSize.Rotate() : item.gridSize;
    /// <summary Whether this item can be marked as sellable </summary>
    public bool canSell => empty && ShopManager.instance != null && placed;
    /// <summary> The world space bounding volume of this item </summary>
    public Bounds bounds => collider.bounds;
    public Vector3 gridOffset => new Vector3(gridShape.x, 0, gridShape.y) / 2 * FurnitureSettings.instance.cellSize;

    public string key => throw new System.NotImplementedException();

    /// <summary> The current <see cref="InventoryController"/> instance </summary>
    private InventoryController inventoryController;
    /// <summary> A <see cref="MaterialSwitcher"/> instance for swapping the material of this item </summary>
    private MaterialSwitcher switcher;
    /// <summary> The marker to denote that this item is being sold </summary>
    private GameObject sellingMarker;
    /// <summary> The collider attached to this item </summary>
    private new Collider collider;

    [DeserializationFactory]
    protected static FurnitureController Factory(FurnitureItem item)
    {
        FurnitureController instance = Instantiate(item.prefab);
        return instance;
    }

    private void Awake()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        subgrids = GetComponentsInChildren<FurnitureGrid>();
        switcher = new MaterialSwitcher(gameObject);
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        GetComponentsInChildren<MeshRenderer>().ForEach(m => m.AddComponent<Outline>().enabled = false);
        if (inventoryController != null) inventoryController.onChanged.AddListener(() => {
            hasSpace = inventoryController.CanInsert(item);
        });

        if (ShopManager.instance != null)
        {
            gameObject.AddComponent<NavMeshObstacle>().carving = true;

            foreach (FurnitureGrid grid in subgrids) grid.onChanged.AddListener(() =>
            {
                empty = subgrids.All(s => s.IsEmpty());
            });

            PlaceSellingMarker();
        }
    }

    /// <summary>
    /// Create and place the selling marker on this item.
    /// </summary>
    private void PlaceSellingMarker()
    {
        sellingMarker = Instantiate(FurnitureSettings.instance.defaultSellingMarker, transform);
        sellingMarker.SetActive(selling);
        sellingMarker.transform.localPosition += new Vector3(
            item.gridOffset.x,
            bounds.max.y - transform.position.y,
            item.gridOffset.z
        );
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    public void PrimaryInteract(Interactor interactor)
    {
        if (selling)
        {
            ShopManager.instance.stocksController.CreateSetPricingUI(item);
        } else
        {
            if (!canPickup) return;

            if (inventoryController.InsertItem(item))
            {
                Remove();
                Debug.Log("Picked Up" + gameObject.name);
            }
            else Debug.Log("Can't Pick up" + gameObject.name);
        }

    }

    public void Remove()
    {
        if (grid != null) grid.RemoveItem(this);
        Instantiate(FurnitureSettings.instance.effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Marks the item as sellable if applicable.
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    public void SecondaryInteract(Interactor interactor)
    {
        if (!canSell) return;

        subgrids.ForEach(s => s.gameObject.SetActive(selling));
        selling = !selling;
        sellingMarker.SetActive(selling);
    }

    /// <summary>
    /// Rotates this item by 90 degrees
    /// </summary>
    public void GridRotate()
    {
        transform.Rotate(0, 90, 0);
        GridMove(gridPosition);
    }

    public void GridMoveWorld(Vector3 target)
    {
        GridMove(Vector2Int.RoundToInt(grid.FromWorldspace(target - gridOffset)));
    }

    /// <summary>
    /// Moves this item as close to <paramref name="target"/> as possible
    /// </summary>
    public void GridMove(Vector2Int target)
    {
        GridPlace(grid, target);
        if (IsGridValid()) switcher.Reset();
        else switcher.Switch(FurnitureSettings.instance.invalidMaterial);
    }

    /// <summary>
    /// Sets this item to the specified position on the specified grid
    /// </summary>
    public void GridPlace(FurnitureGrid grid, Vector2Int target)
    {
        this.grid = grid;
        gridPosition = Util.Clamp(target, Vector2Int.zero, grid.size - gridShape);
        
        transform.parent = grid.transform;
        transform.localPosition = grid.ToLocalspace(gridRegion.center) - transform.rotation * item.gridOffset;
    }

    /// <returns>True if this item is currently in a valid grid position</returns>
    public bool IsGridValid()
    {
        return grid != null && gridPosition != null && gridRegion.Within(grid.size) && !grid.Intersects(gridRegion);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (item == null || stage == null || stage.prefabContentsRoot != gameObject) return;

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + item.gridOffset, new Vector3(item.gridSize.x, 0, item.gridSize.y) * FurnitureSettings.instance.cellSize);
    }
#endif
}
