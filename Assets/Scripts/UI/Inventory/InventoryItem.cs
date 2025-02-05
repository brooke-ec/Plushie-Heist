using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemClass itemClass;

    public int Height
    {
        get
        {
            if(!rotated)
            {
                return itemClass.sizeHeight;
            }
            return itemClass.sizeWidth;
        }
    }

    public int Width
    {
        get
        {
            if(!rotated)
            {
                return itemClass.sizeWidth;
            }
            return itemClass.sizeHeight;
        }
    }

    /// <summary> Mostly used to pick up an item. Gets the INDEXES of the main position on grid, x and y </summary>
    public Vector2Int mainPositionOnGrid = new Vector2Int();

    public bool rotated = false;

    public void Set(ItemClass itemClass)
    {
        this.itemClass = itemClass;
        GetComponent<Image>().sprite = itemClass.itemIcon;

        Vector2 size = new Vector2();
        size.x = Width * InventoryGrid.usableTileSize;
        size.y = Height * InventoryGrid.usableTileSize;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotate()
    {
        //MISSING ADAPTATION HERE
        if (itemClass.sizeWidth == itemClass.sizeHeight) { return; }

        rotated = !rotated;
        RectTransform itemRectTransform = GetComponent<RectTransform>();
        //if rotated is true, rotate it to 90 degrees, otherwise rotate it to 0
        itemRectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f);
    }
}
