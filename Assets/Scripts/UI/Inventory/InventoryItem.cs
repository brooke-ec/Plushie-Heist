using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemClass itemClass;

    /// <summary> Mostly used to pick up an item </summary>
    public Vector2Int mainPositionOnGrid = new Vector2Int();

    public void Set(ItemClass itemClass)
    {
        this.itemClass = itemClass;
        GetComponent<Image>().sprite = itemClass.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemClass.sizeWidth * InventoryGrid.tileSize;
        size.y = itemClass.sizeHeight * InventoryGrid.tileSize;
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
