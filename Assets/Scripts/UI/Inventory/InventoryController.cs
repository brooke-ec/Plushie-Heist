using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary> Controls all interaction with all inventory grids (so we can have multiple) </summary>
public class InventoryController : MonoBehaviour
{
    public GameObject itemPrefab;
    /// <summary> The current grid being used to pick up and place items. This is set to null when you click outside of an inventory grid </summary>
    [HideInInspector] public InventoryGrid selectedInventoryGrid;
    /// <summary> The current grid being used to add items EVEN WHEN NOT CURRENTLY VISUALLY ACTIVE. </summary>
    public InventoryGrid inventoryGridToAddItems;
    [HideInInspector] public InventoryItem selectedItem;

    [SerializeField] private GameObject ItemUI;

    private RectTransform selectedItemRectTransform;

    private InventoryItem overlapItem;

    private Vector2 mousePos;

    private void Update()
    {
        ItemIconDragEffect();
    }

    #region Inventory controls
    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    public void InsertItem(ItemClass itemToInsert)
    {
        bool insertedSuccessfully = InsertItem(itemClassToInsert);
        //maybe in the future check if false, do error sound or something
    }

        InventoryItem item = Instantiate(ItemUI,FindAnyObjectByType<Canvas>().transform).GetComponent<InventoryItem>();
        item.Set(itemToInsert);

        Vector2Int? posOnGrid = selectedInventoryGrid.FindSpaceForObject(item);
        if (posOnGrid != null) //space on grid, so place into position found
        {
            selectedInventoryGrid.PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);
        }
        else
        {
            //space on grid, so place into position found
            inventoryGridToAddItems.PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);
            addedItemSuccessfully = true;
            print("placed item");
        }

        //set grid back off if originally not active
        if(gridWasOriginallyOff)
        {
            inventoryGridToAddItems.gameObject.SetActive(false);
        }

        return addedItemSuccessfully;
    }

    /// <summary> Left click </summary>
    private void PickUpOrPlaceItem()
    {
        
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
    #endregion
    #region input
    public void rotateItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            RotateItem();
        }
    }

    public void startDrag(InputAction.CallbackContext ctx) 
    {
        if (ctx.performed) 
        {
            PickUpOrPlaceItem();
        }
    }

    public void getMousePos(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }


    #endregion
}
//MISSING DRAGGING INSTEAD OF CLICK TO-DO