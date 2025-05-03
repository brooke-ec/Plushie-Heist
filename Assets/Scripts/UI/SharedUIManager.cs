using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used ONLY for shared UI between shop and night (inventory)
/// </summary>
public class SharedUIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;

    public static SharedUIManager instance { get; private set; }

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
        scaleFactor = rootCanvas.scaleFactor;
        InventoryGrid[] allGrids = FindObjectsByType<InventoryGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(InventoryGrid grid in allGrids)
        {
            grid.StartInventory();
        }

        if(NightManager.instance != null)
        {
            //if nighttime
            //then turn off button of add items to storage
            //canvas, backpack, last child (button)
            transform.GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(false);
        }

        GetComponent<InventoryController>().PlaceTestItems();
    }
}
