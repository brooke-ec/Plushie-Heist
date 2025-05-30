using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10;
    [SerializeField] CharacterController controller;
    [SerializeField] new private Camera camera;
    [SerializeField] private Interactor interactor;
    [SerializeField] private GameObject prompt;

    public static FurniturePlacer instance;

    private UnityEvent onPlaced;
    private FurnitureController item;
    private int gridLayer;
    private int itemLayer;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }

    private void Start()
    {
        gridLayer = LayerMask.GetMask("Furniture Grid");
        itemLayer = LayerMask.GetMask("Furniture Item");
        controller.excludeLayers |= gridLayer;
        if (ShopManager.instance == null) Destroy(this);
    }

    private void Update()
    {
        prompt.SetActive(item != null);
        if (item != null)
        {
            camera.cullingMask |= gridLayer;
            controller.excludeLayers |= itemLayer;
            interactor.interactorLayerMask &= ~itemLayer;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, gridLayer))
            {
                if (hit.transform.TryGetComponent(out FurnitureGrid grid)
                    && !grid.transform.IsChildOf(item.transform)) item.grid = grid;
                Move(hit.point);
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~itemLayer)) Move(hit.point);
            else Move(transform.position + transform.forward * maxDistance);
        }
        else
        {
            camera.cullingMask &= ~gridLayer;
            controller.excludeLayers &= ~itemLayer;
            interactor.interactorLayerMask |= itemLayer;
        }
    }

    private void Move(Vector3 target)
    {
        if (item == null || item.grid == null) return;
        item.GridMoveWorld(target);
    }

    /// <summary>
    /// Starts placing the provided item.
    /// </summary>
    /// <param name="item">A prefab of the item to place</param>
    /// <returns>An event invoked when this item is succesfully placed</returns>
    public UnityEvent Place(FurnitureItem item)
    {
        Debug.Log($"Placing {item.itemName}");
        if (this.item != null) Destroy(this.item.gameObject);

        this.item = Instantiate(item.prefab);
        return onPlaced = new UnityEvent();
    }

    public void OnPlace(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (item != null && item.IsGridValid())
        {
            Instantiate(FurnitureSettings.instance.effect, item.transform.position, Quaternion.identity);
            onPlaced.Invoke();
            item.grid.AddItem(item);
            item = null;
        }
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton() || item == null) return;
        item.GridRotate();
    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton() || item == null) return;
        Destroy(item.gameObject);
    }

}
