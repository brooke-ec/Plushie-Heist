using UnityEngine;

/// <summary> Controls all interaction with all inventory grids (so we can have multiple) </summary>
public class InventoryController : MonoBehaviour
{
    public GameObject itemPrefab;
    /// <summary> The current grid being used to pick up and place items. This is set to null when you click outside of an inventory grid </summary>
    [HideInInspector] public InventoryGrid selectedInventoryGrid;
    /// <summary> The current grid being used to add items EVEN WHEN NOT CURRENTLY VISUALLY ACTIVE. </summary>
    public InventoryGrid inventoryGridToAddItems;
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

    /// <summary>
    /// Tries to add given item class to the inventory already set MANUALLY in inventoryGridToAddItems variable.
    /// Wrapper method for InsertItem, so that it can be passed to a button (but might want to check if successful or not in the future)
    /// </summary>
    /// <param name="itemClassToInsert">The item class to create the Inventory Item from</param>
    public void TryInsertItem(ItemClass itemClassToInsert)
    {
        bool insertedSuccessfully = InsertItem(itemClassToInsert);
        //maybe in the future check if false, do error sound or something
    }

    /// <summary>
    /// Tries to add given item class to the inventory set MANUALLY in inventoryGridToAddItems variable.
    /// </summary>
    /// <param name="itemClassToInsert">The item class to create the Inventory Item from</param>
    /// <returns>True if it was a successful insertion, false otherwise (like not enough space)</returns>
    private bool InsertItem(ItemClass itemClassToInsert)
    {
        if (inventoryGridToAddItems == null) { return false; }

        bool addedItemSuccessfully = false;

        //if the inventory grid was originally not active, add the item and then set it back off
        bool gridWasOriginallyOff = !inventoryGridToAddItems.gameObject.activeSelf;

        //Instantiate the item
        Transform rootCanvas = FindAnyObjectByType<UIManager>().rootCanvas.transform;
        InventoryItem item = Instantiate(itemPrefab, rootCanvas).GetComponent<InventoryItem>();
        item.Set(itemClassToInsert);

        Vector2Int? posOnGrid = inventoryGridToAddItems.FindSpaceForObject(item);
        if (posOnGrid == null)
        {            
            //no space on grid, so destroy item (it was used to see if there was enough space)
            Destroy(item.gameObject);
            print("no space on grid");
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