using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [SerializeField] float maxDistance = 10;
    [SerializeField] FurnitureItem[] test;

    private FurnitureItem item;
    private int testIndex = 0;

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

    private void Update()
    {
        if (item != null)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, LayerMask.GetMask("Furniture Grid")))
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
    }
}
