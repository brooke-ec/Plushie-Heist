using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> Controls a single inventory grid functionality </summary>
public class InventoryGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region References
    private InventoryController inventoryController;
    private RectTransform rectTransform;
    #endregion
    /// <summary> Used both for width and height of ENTIRE tile (including shadow and offsets) </summary>
    public const int tileSize = 130;
    /// <summary> Used both for width and height of USABLE tile (white, not shadow) </summary>
    public const int usableTileSize = 100;
    public const int offsetFromShadow = 10;
    public const int offsetFromImage = 20;

    [SerializeField] private int inventoryWidth;
    [SerializeField] private int inventoryHeight;

    InventoryItem[,] inventorySlots;
    private float scaleFactor;

    public void StartInventory()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        rectTransform = GetComponent<RectTransform>();

        scaleFactor = SharedUIManager.instance.scaleFactor;

        CreateInventoryGrid(inventoryWidth, inventoryHeight);
        //PlaceTestItems();
    }

    #region Setup
    /// <summary> Create inventory grid of width and height, such as 3x3  /// </summary>
    public void CreateInventoryGrid(int width, int height)
    {
        inventorySlots = new InventoryItem[width, height];
        rectTransform.sizeDelta = new Vector2((width * tileSize) - offsetFromImage, (height * tileSize) - offsetFromImage); //actual size of inventory
    }

    /// <summary> Gets the grid position of the mouse position, where the grid is anchored top left </summary>
    public Vector2Int GetTileGridPosition(Vector2 mousePos)
    {
        Vector2 mousePosOnGrid = new Vector2();
        Vector2Int tileGridPosition = new Vector2Int(0, 0);

        mousePosOnGrid.x = mousePos.x - rectTransform.position.x;
        mousePosOnGrid.y = rectTransform.position.y - mousePos.y;

        tileGridPosition.x = (int)(mousePosOnGrid.x / (tileSize * scaleFactor));
        tileGridPosition.y = (int)(mousePosOnGrid.y / (tileSize * scaleFactor));

        return tileGridPosition;

    }
    #endregion

    #region Set a grid as interactable or not
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.selectedInventoryGrid = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //maybe it's better if it's only re-set to a new selected Inventory Grid if that one calls it, not sure
        inventoryController.selectedInventoryGrid = null;
    }
    #endregion

    #region Storing items on grid

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        return GetSpaceAvailable(new Vector2Int(itemToInsert.Width, itemToInsert.Height));
    }

    public Vector2Int? FindSpaceForObject(FurnitureItem itemToInsert)
    {
        return GetSpaceAvailable(new Vector2Int(itemToInsert.inventorySize.x, itemToInsert.inventorySize.y));
    }

    /// <summary> Place item in inventory in tile units. Eg: [2, 5] INCLUDES OVERLAP ITEM </summary>
    public bool PlaceItem(InventoryItem item, int xPos, int yPos, ref InventoryItem overlapItem)
    {
        bool isWithinBoundaries = IsWithinBoundaries(xPos, yPos, new Vector2Int(item.Width, item.Height));
        if (!isWithinBoundaries) { return false; }

        bool isNotOverlapping = IsNotOverlapping(xPos, yPos, new Vector2Int(item.Width, item.Height), ref overlapItem);
        if (!isNotOverlapping) { overlapItem = null; return false; }

        if (overlapItem != null) //clean grid as you're picking it up again
        {
            CleanGridReference(overlapItem);
        }

        PlaceItem(item, xPos, yPos);

        return true;
    }
    /// <summary> Place item in inventory in tile units. Eg: [2, 5] </summary>
    public void PlaceItem(InventoryItem item, int xPos, int yPos)
    {
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);

        //telling the grid that this item is present on each of these tiles of the grid (if bigger than 1x1)
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                inventorySlots[xPos + x, yPos + y] = item;
            }
        }
        item.mainPositionOnGrid.x = xPos;
        item.mainPositionOnGrid.y = yPos;

        //PLACE CENTERED
        Vector2 itemPosition = new Vector2();

        int horizontalOffsetNeeded = (offsetFromImage + offsetFromShadow) * (item.Width - 1); //this is so objects of size 1 have no extra-tile spacing offsets
        int verticalOffsetNeeded = (offsetFromImage + offsetFromShadow) * (item.Height - 1);

        itemPosition.x = (xPos * tileSize) + ((usableTileSize * item.Width) + horizontalOffsetNeeded) / 2; //+ is the offset so it's in the centre
        itemPosition.y = -((yPos * tileSize) + ((usableTileSize * item.Height) + verticalOffsetNeeded) / 2);

        itemRectTransform.localPosition = itemPosition;
    }

    public InventoryItem PickUpItem(int xPos, int yPos)
    {
        InventoryItem item = inventorySlots[xPos, yPos];

        if (item == null) { return null; }

        CleanGridReference(item);
        return item;
    }

    public bool IsThisItemTypeInTheInventory(FurnitureItem itemClass)
    {
        for(int x=0;x<inventoryWidth; x++)
        {
            for(int y=0;y<inventoryHeight; y++)
            {
                if (inventorySlots[x, y] != null && itemClass.Equals(inventorySlots[x, y].itemClass))
                {
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary> cleans grid area of where the item used to be </summary>
    public void CleanGridReference(InventoryItem item)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                inventorySlots[item.mainPositionOnGrid.x + x, item.mainPositionOnGrid.y + y] = null;
            }
        }
    }

    private bool IsWithinBoundaries(int xPos, int yPos, Vector2Int itemSize)
    {
        //top left position
        if (!IsPosInsideGrid(xPos, yPos)) { return false; }

        //bottom right pos of item
        xPos += itemSize.x - 1;
        yPos += itemSize.y - 1;
        if (!IsPosInsideGrid(xPos, yPos)) { return false; }

        return true;
    }
    /// <summary> used to check if the position of a tile that an item occupies is inside the grid, used in boundary check </summary>
    private bool IsPosInsideGrid(int xPos, int yPos)
    {
        if (xPos < 0 || yPos < 0)
        {
            return false;
        }
        if (xPos >= inventoryWidth || yPos >= inventoryHeight)
        {
            return false;
        }
        return true;
    }

    private bool IsNotOverlapping(int xPos, int yPos, Vector2Int itemSize, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < itemSize.x; x++)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                if (inventorySlots[xPos + x, yPos + y] != null) //if there is an item in the checked position
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventorySlots[xPos + x, yPos + y]; //then we found an overlap, and we set it
                    }
                    else if (overlapItem != inventorySlots[xPos + x, yPos + y])
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    // Similar to IsNotOverlapping but doesn't have the overlap checks
    private Vector2Int? GetSpaceAvailable(Vector2Int itemSize)
    {
        for(int yPos = 0; yPos<inventoryHeight; yPos++)
        {
            for(int xPos = 0; xPos<inventoryWidth; xPos++)
            {
                //here it looks terrible, but essentially checks for each of the slots of that item,
                //if the required contiguous slots for its size are free

                bool areAllNeededSlotsFree = true; 

                for (int x = 0; x < itemSize.x; x++)
                {
                    for (int y = 0; y < itemSize.y; y++)
                    {
                        bool isPosInsideGrid = IsPosInsideGrid(xPos + x, yPos + y);
                        if ((isPosInsideGrid && inventorySlots[xPos + x, yPos + y] != null) || !isPosInsideGrid) //if there is an item in the checked position
                        {
                            areAllNeededSlotsFree = false;
                            break; //not available
                        }
                    }
                    //start again
                    if(!areAllNeededSlotsFree) break;
                }

                //if all the slots we need are free for this starting x and y pos, then return true, otherwise keep going
                if(areAllNeededSlotsFree) 
                {
                    return new Vector2Int(xPos, yPos);
                }
            }
        }
        return null;
    }

    #endregion

    #region Extra functionality

    /// <summary>
    /// Call to get the dictionary form of the current inventory
    /// </summary>
    /// <returns>A dictionary with each itemclass in the inventory, and the number of times it appears</returns>
    public Dictionary<FurnitureItem, int> GetDictionaryOfCurrentItems()
    {
        Dictionary<FurnitureItem, int> dictionary = new Dictionary<FurnitureItem, int>();

        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                InventoryItem item = inventorySlots[x, y];
                //if there is an item and that item's main position on grid is this one
                if (item != null && item.mainPositionOnGrid == new Vector2Int(x, y))
                {
                    //if it already contains it, just add 1 to the item
                    if(dictionary.ContainsKey(item.itemClass))
                    {
                        dictionary[item.itemClass] += 1;
                    }
                    else
                    {
                        //set it to 1
                        dictionary.Add(item.itemClass, 1);
                    }
                }
            }
        }

        return dictionary;
    }

    #endregion

    #region Test
    public List<FurnitureItem> itemsToTest = new List<FurnitureItem>();
    public GameObject itemPrefab;
    //
    public void PlaceTestItems()
    {
        Transform rootCanvas = SharedUIManager.instance.rootCanvas.transform;
        InventoryItem item = Instantiate(itemPrefab, rootCanvas).GetComponent<InventoryItem>();
        item.Set(itemsToTest[0]);
        PlaceItem(item, 0, 0);

        InventoryItem item2 = Instantiate(itemPrefab, rootCanvas).GetComponent<InventoryItem>();
        item2.Set(itemsToTest[1]);
        PlaceItem(item2, 0, 1);

        InventoryItem item3 = Instantiate(itemPrefab, rootCanvas).GetComponent<InventoryItem>();
        item3.Set(itemsToTest[0]);
        PlaceItem(item3, 2, 1);
    }
    #endregion
}
