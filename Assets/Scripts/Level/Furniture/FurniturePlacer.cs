using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10;
    [SerializeField] new private Camera camera;
    [SerializeField] CharacterController controller;
    
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

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, gridLayer))
            {
                FurnitureGrid grid = hit.transform.GetComponent<FurnitureGrid>();
                Vector2 coordinates = grid.FromWorldspace(hit.point);
                item.Place(grid, coordinates);
            }
        }
        else
        {
            camera.cullingMask &= ~gridLayer;
            controller.excludeLayers &= ~itemLayer;
        }
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
            item.grid.AddItem(item);
            item = null;
        }
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValueAsButton() || item == null) return;
        item.Rotate();
    }
}
