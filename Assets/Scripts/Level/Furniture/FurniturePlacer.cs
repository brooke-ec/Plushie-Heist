using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10;
    [SerializeField] CharacterController controller;
    [SerializeField] new private Camera camera;
    [SerializeField] private Interactor interactor;
    
    private FurnitureItem item;
    private int gridLayer;
    private int itemLayer;

    [Header("Testing")]
    [SerializeField] FurnitureItem[] test;
    private int testIndex = 0;

    private void Start()
    {
        gridLayer = LayerMask.GetMask("Furniture Grid");
        itemLayer = LayerMask.GetMask("Furniture Item");
        controller.excludeLayers |= gridLayer;
    }

    private void Update()
    {
        if (item != null)
        {
            camera.cullingMask |= gridLayer;
            controller.excludeLayers |= itemLayer;
            interactor.interactorLayerMask &= ~itemLayer;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, gridLayer))
            {
                if (hit.transform.TryGetComponent(out FurnitureGrid grid)
                    && !grid.transform.IsChildOf(item.transform)) item.owner = grid;
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
        if (item == null || item.owner == null) return;
        target -= new Vector3(item.size.x, 0, item.size.y) / 2 * FurnitureSettings.instance.cellSize;
        item.Move(Vector2Int.RoundToInt(item.owner.FromWorldspace(target)));
    }

    public void OnPlace(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton()) return;

        if (item == null)
        {
            item = Instantiate(test[testIndex % test.Length]);
            testIndex++;
        }
        else if (item.IsValid())
        {
            item.owner.AddItem(item);
            item = null;
        }
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton() || item == null) return;
        item.Rotate();
    }
}
