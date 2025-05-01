using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary> Controls all interaction with all inventory grids (so we can have multiple) </summary>
public class InventoryController : MonoBehaviour, IUIMenu
{
    public GameObject itemPrefab;
    /// <summary> The current grid being used to pick up and place items. This is set to null when you click outside of an inventory grid </summary>
    [HideInInspector] public InventoryGrid selectedInventoryGrid;
    /// <summary> The current grid being used to add items EVEN WHEN NOT CURRENTLY VISUALLY ACTIVE. </summary>
    public InventoryGrid inventoryGridToAddItems;
    [HideInInspector] public InventoryItem selectedItem;

    /// <summary> Event fired whenever the inventory is changed </summary>
    public readonly UnityEvent onChanged = new UnityEvent();

    private RectTransform selectedItemRectTransform;

    private InventoryItem overlapItem;

    private Vector2 mousePos;

    public static InventoryController instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.LogError("Inventory controller instance already in scene");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        inventoryGridToAddItems.StartInventory();
#if UNITY_EDITOR
        PlaceTestItems();
#endif
    }

    private void Update()
    {
        ItemIconDragEffect();
    }

    /// <summary>
    /// Toggle the inventory grid on or off.
    /// </summary>
    /// <returns>Whether the inventory is visible</returns>
    public bool OpenOrCloseInventory()
    {
        SharedUIManager.instance.ToggleMenu(this);
        return SharedUIManager.instance.isMenuOpen;
    }

    public void SetOpenState(bool open)
    {
        Transform inventoryTopParent = inventoryGridToAddItems.transform.parent.parent.parent.parent;
        inventoryTopParent.gameObject.SetActive(open);
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
    public void TryInsertItem(FurnitureItem itemClassToInsert)
    {
        bool insertedSuccessfully = InsertItem(itemClassToInsert);
        //maybe in the future check if false, do error sound or something
    }

    /// <summary>
    /// Checks if the item can be inserted into the inventory grid
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns></returns>
    public bool CanInsert(FurnitureItem item)
    {
        return inventoryGridToAddItems.FindSpaceForObject(item) != null;
    }

    /// <summary>
    /// Tries to add given item class to the inventory set MANUALLY in inventoryGridToAddItems variable.
    /// </summary>
    /// <param name="itemClassToInsert">The item class to create the Inventory Item from</param>
    /// <returns>True if it was a successful insertion, false otherwise (like not enough space)</returns>
    public bool InsertItem(FurnitureItem itemClassToInsert)
    {
        if (inventoryGridToAddItems == null) { return false; }

        bool addedItemSuccessfully = false;

        //if the inventory grid was originally not active, add the item and then set it back off
        bool gridWasOriginallyOff = !inventoryGridToAddItems.gameObject.activeSelf;

        //Instantiate the item
        Transform rootCanvas = SharedUIManager.instance.rootCanvas.transform;
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
            onChanged.Invoke();
        }

        //set grid back off if originally not active
        if (gridWasOriginallyOff)
        {
            inventoryGridToAddItems.gameObject.SetActive(false);
        }

        return addedItemSuccessfully;
    }

    /// <summary>
    /// Call when you want to remove a given InventoryItem from the inventory.
    /// For example, when you want to place it, calls this so the grid space is cleared (and potentially removed from the pricing table)
    /// </summary>
    /// <param name=""></param>
    public void RemoveItemFromInventory(InventoryItem item)
    {
        inventoryGridToAddItems.CleanGridReference(item);
        print("item removed from inventory");

        //see if there is another of this in the inventory, if there isn't then call try remove
        if(!inventoryGridToAddItems.IsThisItemTypeInTheInventory(item.itemClass) && ShopManager.instance!=null)
        {
            ShopManager.instance.stocksController.TryRemoveFurnitureFromPricingTable(item.itemClass);
        }

        Destroy(item.gameObject);
    }

    /// <summary> Left click </summary>
    private void PickUpOrPlaceItem()
    {
        if (selectedInventoryGrid == null) { return; } //if not on a grid, do nothing

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

        onChanged.Invoke();
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

    #region Test
#if UNITY_EDITOR
    public List<FurnitureItem> itemsToTest = new List<FurnitureItem>();
    //
    public void PlaceTestItems()
    {
        Transform rootCanvas = SharedUIManager.instance.rootCanvas.transform;
        
        foreach (FurnitureItem item in itemsToTest) InsertItem(item);
        itemsToTest.Clear();
    }
#endif
#endregion

    //MISSING DRAGGING INSTEAD OF CLICK TO-DO
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
