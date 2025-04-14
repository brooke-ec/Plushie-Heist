using System.Collections.Generic;
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

        scaleFactor = FindAnyObjectByType<UIManager>().scaleFactor;

        CreateInventoryGrid(inventoryWidth, inventoryHeight);
        PlaceTestItems();
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
        inventoryController.selectedInventoryGrid = null;
    }
    #endregion

    #region Storing items on grid

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int width = tileSize - itemToInsert.Width + 1;
        int height = tileSize - itemToInsert.Height + 1;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsSpaceAvailable(x, y, new Vector2Int(itemToInsert.Width, itemToInsert.Height)))
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        //no space on grid
        return null;
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

    /// <summary> cleans grid area of where the item used to be </summary>
    private void CleanGridReference(InventoryItem item)
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
    /// <summary> used to check if the position of all tiles that an item occupies are inside the grid, used in boundary check </summary>
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

    //similar to overlapping check but doesn't have the overlap checks
    private bool IsSpaceAvailable(int xPos, int yPos, Vector2Int itemSize)
    {
        for (int x = 0; x < itemSize.x; x++)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                if (inventorySlots[xPos + x, yPos + y] != null) //if there is an item in the checked position
                {
                    return false; //not available
                }
            }
        }
        return true;
    }

    #endregion

    #region Test
    public List<ItemClass> itemsToTest = new List<ItemClass>();
    public GameObject itemPrefab;
    //
    public void PlaceTestItems()
    {
        Transform rootCanvas = FindAnyObjectByType<UIManager>().rootCanvas.transform;
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
