using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{

    [SerializeField] private float maxDistance = 10;
    
    private FurnitureItem item;
    private new Camera camera;
    private int mask;

    [Header("Testing")]
    [SerializeField] FurnitureItem[] test;
    private int testIndex = 0;

    private void Start()
    {
        mask = LayerMask.GetMask("Furniture Grid");
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (item != null)
        {
            camera.cullingMask = camera.cullingMask | mask;

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask))
            {
                FurnitureGrid grid = hit.transform.GetComponent<FurnitureGrid>();

                Vector2 coordinates = grid.FromWorldspace(hit.point);
                Region region = Region.FromSize(
                    Mathf.FloorToInt(coordinates.x),
                    Mathf.FloorToInt(coordinates.y),
                    item.size.x, item.size.y
                );

                item.transform.position = grid.ToWorldspace(region.center);
            }
        }
        else camera.cullingMask = camera.cullingMask & ~mask;
    }

    public void OnPlace(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton()) return;
        if (item == null)
        {
            item = Instantiate(test[testIndex % test.Length]);
            testIndex++;
        }
        else item = null;
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton() || item == null) return;
        item.Rotate();
    }
}
