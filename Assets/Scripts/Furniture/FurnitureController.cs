using cakeslice;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.AI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class FurnitureController : MonoBehaviour, IInteractable, ISavable
{
    /// <summary> The item this prefab represents </summary>
    public FurnitureItem item = null;
    /// <summary> Prompt Shown by the UI to let the player know they can interact with it </summary>
    string IInteractable.interactionPrompt => empty 
        ? (hasSpace ? "Press F to Pick Up" : "Inventory Full")
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
    public Region gridRegion => new Region().FromSize(gridPosition, gridShape);
    /// <summary> Whether this item is marked as sellable or not </summary>
    public bool selling => sellingMarker.activeSelf && placed;
    /// <summary> The shape of this furniture item </summary>
    public Vector2Int gridShape { get; private set; }
    /// <summary Whether this item can be marked as sellable </summary>
    public bool canSell => empty && ShopManager.instance != null && placed;
    /// <summary> The world space bounding volume of this item </summary>
    public Bounds bounds => collider.bounds;

    public string key => throw new System.NotImplementedException();

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
        GetComponentsInChildren<MeshRenderer>().ForEach(m => m.AddComponent<Outline>().enabled = false);
        this.AddComponent<NavMeshObstacle>().carving = true;
        gridShape = item.gridSize;

        if (inventoryController != null) inventoryController.onChanged.AddListener(() => {
            hasSpace = inventoryController.CanInsert(item);
        });

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

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    public void PrimaryInteract(Interactor interactor)
    {
        if (!canPickup) return;

        if (inventoryController.InsertItem(item))
        {
            if (grid != null) grid.RemoveItem(this);

            Destroy(gameObject);
            Debug.Log("Picked Up" + gameObject.name);
        }
        else Debug.Log("Can't Pick up" + gameObject.name);

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
        sellingMarker.SetActive(!selling);
    }

    /// <summary>
    /// Rotates this item by 90 degrees
    /// </summary>
    public void GridRotate()
    {
        gridShape = new Vector2Int(gridShape.y, gridShape.x);
        transform.Rotate(0, 90, 0);
        GridMove(gridPosition);
    }

    /// <summary>
    /// Moves this item as close to <paramref name="target"/> as possible
    /// </summary>
    public void GridMove(Vector2Int target)
    {
        gridPosition = Util.Clamp(target, Vector2Int.zero, grid.size - gridShape);
        transform.position = grid.ToWorldspace(gridRegion.center) - transform.rotation * item.gridOffset;

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
        UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (item == null || stage == null || stage.prefabContentsRoot != gameObject) return;

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(transform.position + item.gridOffset, new Vector3(item.gridSize.x, 0, item.gridSize.y) * FurnitureSettings.instance.cellSize);
    }

    public void Deserialize(JObject obj)
    {
        throw new System.NotImplementedException();
    }

    public JToken Serialize()
    {
        return new JObject(
            new JProperty("item", item.filename),
            new JProperty("rotation", gridRotation),
            new JProperty("selling", selling),
            new JProperty("position", JsonUtility.ToJson(gridPosition)),
            new JProperty("subgrids", subgrids.Select(s => s.Serialize()))
        );
    }
#endif
}
