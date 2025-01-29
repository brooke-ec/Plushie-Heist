using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemClass itemClass;

    private void Set(ItemClass itemClass)
    {
        this.itemClass = itemClass;
        GetComponent<Image>().sprite = itemClass.itemIcon;
    }
}
