using UnityEngine;

/// <summary> Controls all interaction with all inventory grids (so we can have multiple) </summary>
public class InventoryController : MonoBehaviour
{
    [HideInInspector] public InventoryGrid selectedInventoryGrid;
    [HideInInspector] public InventoryItem selectedItem;
    private RectTransform selectedItemRectTransform;

    private InventoryItem overlapItem;

    private void Update()
    {
        ItemIconDragEffect();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (selectedInventoryGrid == null) { return; }

        //to start dragging
        if (Input.GetMouseButtonDown(0))
        {
            PickUpOrPlaceItem();
        }
    }

    #region Inventory controls
    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    public void InsertItem(InventoryItem itemToInsert)
    {
        if (selectedInventoryGrid == null) { return; }

        Vector2Int? posOnGrid = selectedInventoryGrid.FindSpaceForObject(itemToInsert);
        if (posOnGrid != null) //space on grid, so place into position found
        {
            selectedInventoryGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }
    }

    /// <summary> Left click </summary>
    private void PickUpOrPlaceItem()
    {
        Vector2 mousePos = Input.mousePosition;
        if (selectedItem != null)
        {
            mousePos.x -= (selectedItem.Width - 1) * InventoryGrid.tileSize / 2;
            mousePos.y += (selectedItem.Height - 1) * InventoryGrid.tileSize / 2;
        }

        Vector2Int posOnGrid = selectedInventoryGrid.GetTileGridPosition(mousePos); //tile grid position
        if (selectedItem == null)
        {
            PickUpItem(posOnGrid);
        }
        else
        {
            PlaceItem(posOnGrid);
        }
    }
    #endregion

    #region Inventory inner workings
    private void PlaceItem(Vector2Int posOnGrid)
    {
        bool itemPlaced = selectedInventoryGrid.PlaceItem(selectedItem, posOnGrid.x, posOnGrid.y, ref overlapItem);
        if (itemPlaced)
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                //this is so you now pick up the overlap item
                selectedItem = overlapItem;
                overlapItem = null;
                selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
                selectedItemRectTransform.SetAsLastSibling();
            }
        }
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
#endregion

//MISSING DRAGGING INSTEAD OF CLICK TO-DO