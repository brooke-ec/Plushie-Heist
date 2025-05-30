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
    public InventoryGrid storageGrid;
    /// <summary> The grid for the backpack inventory </summary>
    public InventoryGrid backpackGrid;

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
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        FindObjectsOfType<InventoryGrid>(true).ForEach(g => g.SetupGrid());
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
        AudioManager.instance.PlaySound(open ? AudioManager.SoundEnum.backpackOpen : AudioManager.SoundEnum.backpackClose);
        Transform inventoryTopParent = backpackGrid.transform.parent.parent.parent.parent;
        inventoryTopParent.gameObject.SetActive(open);
        if (NightManager.instance != null) Time.timeScale = open ? 0 : 1;
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
        return backpackGrid.FindSpaceForObject(item) != null;
    }

    /// <summary>
    /// Tries to add given item class to the inventory set MANUALLY in inventoryGridToAddItems variable.
    /// </summary>
    /// <param name="itemClassToInsert">The item class to create the Inventory Item from</param>
    /// <returns>True if it was a successful insertion, false otherwise (like not enough space)</returns>
    public bool InsertItem(FurnitureItem itemClassToInsert, bool fromBackpack=true)
    {
        InventoryGrid gridToUse = storageGrid;
        if (fromBackpack) { gridToUse = backpackGrid; }

        if (gridToUse == null) { return false; }

        bool addedItemSuccessfully = false;

        //if the inventory grid was originally not active, add the item and then set it back off
        bool gridWasOriginallyOff = !gridToUse.gameObject.activeSelf;

        //Instantiate the item
        InventoryItem item = InventoryItem.Factory(itemClassToInsert);

        Vector2Int? posOnGrid = gridToUse.FindSpaceForObject(item);
        if (posOnGrid == null)
        {
            //no space on grid, so destroy item (it was used to see if there was enough space)
            Destroy(item.gameObject);
            print("no space on grid");
        }
        else
        {
            //space on grid, so place into position found
            gridToUse.PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);
            addedItemSuccessfully = true;
            print("placed item");
            onChanged.Invoke();
        }

        //set grid back off if originally not active
        if (gridWasOriginallyOff)
        {
            gridToUse.gameObject.SetActive(false);
        }

        if (ShopManager.instance != null) ShopManager.instance.stocksController.UpdatePricingTable();
        return addedItemSuccessfully;
    }

    /// <summary>
    /// Call when you want to remove a given InventoryItem from the inventory.
    /// For example, when you want to place it, calls this so the grid space is cleared (and potentially removed from the pricing table)
    /// </summary>
    /// <param name=""></param>
    public void RemoveItemFromInventory(InventoryItem item, bool fromBackpack=true)
    {
        InventoryGrid gridToUse = storageGrid;
        if (fromBackpack) { gridToUse = backpackGrid; }

        gridToUse.CleanGridReference(item);
        print("item removed from inventory");

        Destroy(item.gameObject);
        onChanged.Invoke();
        
        if (ShopManager.instance != null) ShopManager.instance.stocksController.UpdatePricingTable();
    }

    /// <summary>
    /// Call to remove the first instance of the item type in the inventory if exists
    /// </summary>
    /// <param name="itemCLass"></param>
    /// <returns>True if exists and was removed, false otherwise</returns>
    public bool RemoveAnItemTypeFromInventory(FurnitureItem itemClass, bool fromBackpack= true)
    {
        InventoryGrid gridToUse = storageGrid;
        if(fromBackpack) { gridToUse = backpackGrid; }

        InventoryItem removedItem = gridToUse.GetFirstItemType(itemClass);
        if(removedItem != null)
        {
            RemoveItemFromInventory(removedItem);
            onChanged.Invoke();
            return true;
        }
        return false;
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
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick);
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

    #region Backpack specific
    /// <summary>
    /// Adds as many items from the backpack as there is space in the storage grid
    /// </summary>
    /// <param name="fromBackpack">If false, it does from storage to backpack. If true, it does from backpack to storage</param>
    public void AddItemsFromBackpackToStorage(bool fromBackpack=true)
    {
        if(backpackGrid==null) { print("backpack grid is null"); return; }
        if(storageGrid==null) { print("storage grid is null"); return; }
        if(backpackGrid.Equals(storageGrid)) { print("Backpack and storage are the same?? Error"); return; }

        InventoryItem[,] originalItems;
        if(fromBackpack) { originalItems = backpackGrid.GetInventorySlots(); }
        else { originalItems = storageGrid.GetInventorySlots(); }

        foreach(InventoryItem originalItem in originalItems)
        {
            if (originalItem != null)
            {
                AddItemFromBackpackToStorage(originalItem, fromBackpack);
            }
        }
        
        if(HoveringManager.currentTooltipOpen != null) { Destroy(HoveringManager.currentTooltipOpen); }
    }

    /// <summary>
    /// Call to try to add an item from backpack to storage
    /// </summary>
    /// <param name="fromBackpack">If false, it does from storage to backpack. If true, it does from backpack to storage</param>
    /// <returns>True if added, false otherwise</returns>
    public bool AddItemFromBackpackToStorage(InventoryItem backpackItem, bool fromBackpack = true)
    {
        //insert in gridToAddItems
        bool insertedItem = InsertItem(backpackItem.itemClass, !fromBackpack);
        if (insertedItem)
        {
            //Then remove from the backpack grid
            RemoveItemFromInventory(backpackItem, fromBackpack);
            //Call here because removing item might do the whole stock stuff
        }
        return insertedItem;
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
