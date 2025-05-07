using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EscapingUI : MonoBehaviour
{
    [SerializeField] private GameObject successfulEscapingUIPrefab;
    [SerializeField] private GameObject caughtEscapingUIPrefab;

    [SerializeField] private Color32 pinkColour;
    [SerializeField] private Color32 greenColour;

    public GameObject itemSlotPrefab;
    private bool successful = false;

    public void CreateEscapingUI(bool successful, Transform canvasTransform)
    {
        this.successful = successful;
        GameObject escapingUI;

        //create needed UI element
        if (successful)
        {
            escapingUI = Instantiate(successfulEscapingUIPrefab, canvasTransform);
        }
        else
        {
            escapingUI = Instantiate(caughtEscapingUIPrefab, canvasTransform);
        }

        InventoryController inventoryController = FindAnyObjectByType<InventoryController>();

        //Get the dictionary of items successfully stolen, and those lost
        (Dictionary<FurnitureItem, int> successfullyStolenItems, Dictionary<FurnitureItem, int> lostItems) = RandomiseItemsLost(inventoryController.backpackGrid);

        //Do the first container, which is the same for both
        Transform mainContainer = escapingUI.transform.GetChild(1);
        Transform content = mainContainer.GetChild(0).GetChild(1).GetChild(0).GetChild(0);

        if (successfullyStolenItems.Count == 0)
        {
            //Activate text that says "nothing"
            mainContainer.GetChild(0).GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            InstantiateGivenItemsInContainer(successfullyStolenItems, content);
        }

        if(!successful)
        {
            //then modify the extra container that the unsuccessful has
            if (lostItems.Count == 0)
            {
                //Activate text that says "nothing"
                mainContainer.GetChild(1).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                InstantiateGivenItemsInContainer(lostItems, mainContainer.GetChild(1).GetChild(1).GetChild(0).GetChild(0));
            }
        }

        escapingUI.transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(() => Destroy(escapingUI));
        //TO-DO ADD ON-CLICK OF PASSING TO THE DAY SCENE
    }

    /// <summary>
    /// Given an inventory grid, randomise which items are lost based on the losing items percentage
    /// </summary>
    /// <param name="inventoryGrid"></param>
    /// <returns></returns>
    private (Dictionary<FurnitureItem, int>, Dictionary<FurnitureItem, int>) RandomiseItemsLost(InventoryGrid inventoryGrid)
    {
        Dictionary<FurnitureItem, int> allItems = inventoryGrid.GetDictionaryOfCurrentItems();
        //create the dictionaries
        Dictionary<FurnitureItem, int> successfullyStolenItems = new Dictionary<FurnitureItem, int>(allItems);
        //the difference in items from items originally in storage, and the ones in successfully stolen items
        Dictionary<FurnitureItem, int> lostItems = new Dictionary<FurnitureItem, int>();

        if (!successful)
        {
            //Now shuffle list of items
            //create a list of items, adding every single item in dictionary the number of times the value appears in the dictionary
            List<FurnitureItem> listOfItems = new List<FurnitureItem>();
            foreach (KeyValuePair<FurnitureItem, int> itemPair in allItems)
            {
                for (int i = 0; i < itemPair.Value; i++)
                {
                    listOfItems.Add(itemPair.Key);
                }
            }


            //after this you can shuffle the list
            int numOfItemsToLose = Mathf.CeilToInt((NightManager.instance.itemLosePercentage / 100f) * allItems.Count);
            listOfItems = listOfItems.OrderBy(x => UnityEngine.Random.value).ToList();
            //then add the first numOfItemsToLose into the dictionary format for lostItems
            List<FurnitureItem> itemsToLose = listOfItems.Take(numOfItemsToLose).ToList();

            //for each item to lose, if it still exists in the successful dictionary,
            //take it away and add it to the lostItems dictionary
            foreach (FurnitureItem itemToLose in itemsToLose)
            {
                //if it already contains it, just do -1 to the item
                if (successfullyStolenItems.ContainsKey(itemToLose))
                {
                    successfullyStolenItems[itemToLose] -= 1;
                    if (successfullyStolenItems[itemToLose] == 0) { successfullyStolenItems.Remove(itemToLose); }
                }

                //add it to lost items
                if (lostItems.ContainsKey(itemToLose))
                {
                    lostItems[itemToLose] += 1;
                }
                else
                {
                    //set it to 1
                    lostItems.Add(itemToLose, 1);
                }

                //Now remove from inventory
                InventoryController controller = FindAnyObjectByType<InventoryController>();
                controller.RemoveAnItemTypeFromInventory(itemToLose, true);
            }
        }

        return (successfullyStolenItems, lostItems);
    }

    private void InstantiateGivenItemsInContainer(Dictionary<FurnitureItem, int> items, Transform container)
    {
        Color32 colorToUseForQuantity = GetRightColour();

        foreach (KeyValuePair<FurnitureItem, int> itemPair in items)
        {
            GameObject itemContainer = Instantiate(itemSlotPrefab, container);
            itemContainer.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = itemPair.Key.inventoryIcon;
            itemContainer.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = itemPair.Value.ToString();
            itemContainer.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().color = colorToUseForQuantity;
        }
    }

    private Color32 GetRightColour()
    {
        if (successful)
        {
            return greenColour;
        }
        return pinkColour;
    }
}
