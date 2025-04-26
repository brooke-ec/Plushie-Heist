using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls the prices of ALL available furniture pieces in the game
/// </summary>
public class StocksController : MonoBehaviour
{
    private PricingTableManager pricingTableManager;

    [SerializeField] private List<ItemClass> allItemsInGame = new List<ItemClass>();
    [HideInInspector] public List<ProductData> allStocksInGame;

    public SetPricingUIFunctionality setPricingUIPrefab;

    #region Setup
    private void Awake()
    {
        pricingTableManager = FindAnyObjectByType<PricingTableManager>(FindObjectsInactive.Include);
        CreateAllProductData();
    }

    private void CreateAllProductData()
    {
        int todaysDate = ShopManager.instance.day;

        allStocksInGame = new List<ProductData>(allItemsInGame.Count);
        foreach(ItemClass item in allItemsInGame)
        {
            ProductData product = new ProductData(item, todaysDate);
            allStocksInGame.Add(product);
        }
    }

    #endregion

    #region Actions
    /// <summary>
    /// TO-DO Call when furniture is placed OR new item added to inventory
    /// Essentially, we check if the passed furniture is already part
    /// of the pricing table. If it isn't, it's added. Otherwise nothing happens
    /// </summary>
    public void TryAddFurnitureToPricingTable(ItemClass item)
    {
        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            pricingTableManager.TryAddNewProduct(product);
        }
    }

    public void TryRemoveFurnitureFromPricingTable(ItemClass item)
    {
        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            pricingTableManager.TryRemoveProduct(product);
        }
    }

    /// <summary>
    /// Goes through all stocks and randomise a bit some items' market price
    /// </summary>
    /// <param name="day">The number of today's day</param>
    public void NewDay(int day)
    {
        // go through all stocks and randomise a bit some items market price
        //and update last modified and last market price

        /*int minNumOfChanges = 1;
        int maxNumOfChanges = 3;*/

        int minNumOfChanges = allStocksInGame.Count / 10;
        int maxNumOfChanges = allStocksInGame.Count / 4;
        int numOfChanges = UnityEngine.Random.Range(minNumOfChanges, maxNumOfChanges + 1);

        //shuffle list of items
        List<ProductData> shuffled = allStocksInGame.OrderBy(x => UnityEngine.Random.value).ToList();
        List<ProductData> productsToChange = shuffled.Take(numOfChanges).ToList();

        foreach (ProductData product in productsToChange)
        {
            float lastMarketPrice = product.marketPrice;

            float fluctuation = UnityEngine.Random.Range(-0.2f, 0.2f);
            float dwd = product.marketPrice * (1 + fluctuation);

            float newPrice = Mathf.Max(0f, product.marketPrice * (1 + fluctuation)); //to make sure it doesn't go below 0
            newPrice = (float)System.Math.Round(newPrice, 2);
            //then get to the nearest allowed endings (make it nicer)

            product.marketPrice = RoundToRetailPrice(newPrice);
            if(product.marketPrice != lastMarketPrice)
            {
                //basically if the rounding ended up making the same price, then don't modify last market price
                product.lastMarketPrice = lastMarketPrice;
                product.lastDayChanged = day;
            }
        }

        //Essentially to update ALL of them, because even those not modified need to say like "yesterday"
        pricingTableManager.ResetTableToOriginalOrder();
    }

    public void CreateSetPricingUI(ItemClass item)
    {
        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            SetPricingUIFunctionality pricingUI = Instantiate(setPricingUIPrefab, ShopManager.instance.mainCanvas.transform);
            pricingUI.SetUI(product);
        }
    }

    public void UpdateProduct(ProductData product)
    {
        pricingTableManager.UpdateThisProductInfo(product);
    }

    #endregion

    #region Extras
    private float RoundToRetailPrice(float price)
    {
        float[] allowedEndings = { 0.00f, 0.50f, 0.75f, 0.95f };

        float basePart = Mathf.Floor(price);
        float decimalPart = price - basePart;

        float closestEnding = allowedEndings.OrderBy(ending => Mathf.Abs(decimalPart - ending)).First();

        return basePart + closestEnding;
    }
    #endregion
}
