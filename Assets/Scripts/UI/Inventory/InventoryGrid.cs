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
    const int tileSize = 100;
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
    public void PlaceItem(InventoryItem item, int xPos, int yPos)
    {
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);
        inventorySlots[xPos, yPos] = item;

        Vector2 itemPosition = new Vector2();
        itemPosition.x = (xPos * tileSize) + tileSize / 2; //+ is the offset so it's in the centre
        itemPosition.y = -((yPos * tileSize) + tileSize / 2);

        itemRectTransform.localPosition = itemPosition;
    }

    public InventoryItem PickUpItem(int xPos, int yPos)
    {
        InventoryItem item = inventorySlots[xPos, yPos];
        inventorySlots[xPos, yPos] = null;
        return item;
    }

    #endregion

    #region Test

    #endregion
}
