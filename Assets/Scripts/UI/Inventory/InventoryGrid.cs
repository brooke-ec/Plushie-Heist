using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> Controls a single inventory grid functionality </summary>
public class InventoryGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region References
    private InventoryController inventoryController;
    [SerializeField] private Canvas rootCanvas;
    private RectTransform rectTransform;
    #endregion

    /// <summary> Used both for width and height of tile </summary>
    public const int tileSize = 100;
    [SerializeField] private int inventoryWidth; //gridSizeWidth
    [SerializeField] private int inventoryHeight;
    float scaleFactor;

    InventoryItem[,] inventorySlots;

    private void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        rectTransform = GetComponent<RectTransform>();

        scaleFactor = rootCanvas.scaleFactor;

        CreateInventoryGrid(inventoryWidth, inventoryHeight);
        PlaceTestItems();
    }

    /// <summary> Create inventory grid of width and height, such as 3x3  /// </summary>
    public void CreateInventoryGrid(int width, int height)
    {
        inventorySlots = new InventoryItem[width, height];
        rectTransform.sizeDelta = new Vector2(width * tileSize, height * tileSize); //actual size of inventory
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

    /// <summary> Place item in inventory in tile units. Eg: [2, 5] </summary>
    public bool PlaceItem(InventoryItem item, int xPos, int yPos)
    {
        bool canBePlaced = CanBePlaced(xPos, yPos, new Vector2Int(item.itemClass.sizeWidth, item.itemClass.sizeHeight));
        if (canBePlaced)
        {
            RectTransform itemRectTransform = item.GetComponent<RectTransform>();
            itemRectTransform.SetParent(rectTransform);

            //telling the grid that this item is present on each of these tiles of the grid (if bigger than 1x1)
            for (int x = 0; x < item.itemClass.sizeWidth; x++)
            {
                for (int y = 0; y < item.itemClass.sizeHeight; y++)
                {
                    inventorySlots[xPos + x, yPos + y] = item;
                }
            }
            item.mainPositionOnGrid.x = xPos;
            item.mainPositionOnGrid.y = yPos;

            Vector2 itemPosition = new Vector2();
            itemPosition.x = (xPos * tileSize) + (tileSize * item.itemClass.sizeWidth) / 2; //+ is the offset so it's in the centre
            itemPosition.y = -((yPos * tileSize) + (tileSize * item.itemClass.sizeHeight) / 2);

            itemRectTransform.localPosition = itemPosition;
        }
        return canBePlaced;
    }

    public InventoryItem PickUpItem(int xPos, int yPos)
    {
        InventoryItem item = inventorySlots[xPos, yPos];

        if(item == null) { return null; }

        for(int x = 0; x < item.itemClass.sizeWidth; x++)
        {
            for(int y=0;  y < item.itemClass.sizeHeight;y++)
            {
                inventorySlots[item.mainPositionOnGrid.x + x, item.mainPositionOnGrid.y + y] = null;
            }
        }
        return item;
    }

    private bool IsPosInsideGrid(int xPos, int yPos)
    {
        if(xPos < 0 || yPos < 0)
        {
            return false;
        }
        if (xPos >= inventoryWidth || yPos >= inventoryHeight)
        {
            return false;
        }
        return true;
    }

    private bool CanBePlaced(int xPos, int yPos, Vector2Int itemSize)
    {
        //top left position
        if(!IsPosInsideGrid(xPos, yPos)) { return false; }

        //bottom right pos of item
        xPos += itemSize.x - 1;
        yPos += itemSize.y - 1;
        if (!IsPosInsideGrid(xPos, yPos)) { return false; }

        return true;
    }

    #endregion

    #region Test
    public List<ItemClass> itemsToTest = new List<ItemClass>();
    public GameObject itemPrefab;
    public void PlaceTestItems()
    {
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        item.Set(itemsToTest[0]);
        PlaceItem(item, 1, 3);

        InventoryItem item2 = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        item2.Set(itemsToTest[1]);
        PlaceItem(item2, 0, 2);
    }
    #endregion
}
