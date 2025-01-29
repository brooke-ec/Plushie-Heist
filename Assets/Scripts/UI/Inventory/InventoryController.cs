using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> Controls all interaction with all inventory grids (so we can have multiple) </summary>
public class InventoryController : MonoBehaviour
{
    [HideInInspector] public InventoryGrid selectedInventoryGrid;

    [HideInInspector] public InventoryItem selectedItem;
    private RectTransform selectedItemRectTransform;

    private void Update()
    {
        ItemIconDragEffect();

        if (selectedInventoryGrid == null) { return; }

        //to start dragging
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int posOnGrid = selectedInventoryGrid.GetTileGridPosition(Input.mousePosition); //tile grid position
        if (selectedItem == null)
        {
            PickUpItem(posOnGrid);
        }
        else
        {
            PlaceItem(posOnGrid);
        }
    }

    private void PlaceItem(Vector2Int posOnGrid)
    {
        selectedInventoryGrid.PlaceItem(selectedItem, posOnGrid.x, posOnGrid.y);
        selectedItem = null;
    }

    private void PickUpItem(Vector2Int posOnGrid)
    {
        selectedItem = selectedInventoryGrid.PickUpItem(posOnGrid.x, posOnGrid.y);
        if (selectedItem != null)
        {
            selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    /// <summary> to visualise a drag effect </summary>
    private void ItemIconDragEffect()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = Input.mousePosition;
        }
    }
}

//MISSING DRAGGING INSTEAD OF CLICK TO-DO