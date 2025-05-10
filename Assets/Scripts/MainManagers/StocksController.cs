using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls the prices of ALL available furniture pieces in the game
/// </summary>
public class StocksController : MonoBehaviour
{
    private PricingTableManager pricingTableManager;

    [HideInInspector] [JsonProperty("pricing")] public List<ProductData> allStocksInGame;

    public SetPricingUIFunctionality setPricingUIPrefab;

    public Vector2 purchaseRange = new Vector2(0.65f, 0.75f);

    [Range(0, 1)]
    public float maxPercentOfItemsToChange = 0.5f;
    [Range(0, 1)]
    public float minPercentOfItemsToChange = 0.2f;

    #region Setup
    private void Awake()
    {
        pricingTableManager = FindAnyObjectByType<PricingTableManager>(FindObjectsInactive.Include);
    }

    private void Start()
    {
        if (allStocksInGame == null || allStocksInGame.Count == 0) CreateAllProductData();
        UpdatePricingTable();
    }

    private void CreateAllProductData()
    {
        int todaysDate = ShopManager.instance.day;

        FurnitureItem[] allItemsInGame = Resources.LoadAll<FurnitureItem>(FurnitureItem.PATH);
        allStocksInGame = new List<ProductData>(allItemsInGame.Length);
        foreach (FurnitureItem item in allItemsInGame)
        {
            ProductData product = new ProductData(item, todaysDate);
            allStocksInGame.Add(product);
        }
    }

    #endregion

    #region Actions
    public void UpdatePricingTable()
    {
        FurnitureItem[] all = FindObjectsOfType<FurnitureController>().Select(c => c.item)
            .Concat(FindObjectsOfType<InventoryItem>(true).Select(i => i.itemClass))
            .Distinct().ToArray();

        foreach (ProductData stock in allStocksInGame)
        {
            if (all.Contains(stock.itemRef)) pricingTableManager.TryAddNewProduct(stock);
            else pricingTableManager.TryRemoveProduct(stock);
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

        int minNumOfChanges = (int) (allStocksInGame.Count * maxPercentOfItemsToChange);
        int maxNumOfChanges = (int) (allStocksInGame.Count * minPercentOfItemsToChange);
        int numOfChanges = UnityEngine.Random.Range(minNumOfChanges, maxNumOfChanges + 1);

        //shuffle list of items
        List<ProductData> shuffled = allStocksInGame.OrderBy(x => UnityEngine.Random.value).ToList();
        List<ProductData> productsToChange = shuffled.Take(numOfChanges).ToList();

        foreach (ProductData product in productsToChange)
        {
            float lastMarketPrice = product.marketPrice;

            float fluctuation = UnityEngine.Random.Range(-0.2f, 0.2f);

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

    public void CreateSetPricingUI(FurnitureItem item)
    {
        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            SetPricingUIFunctionality pricingUI = Instantiate(setPricingUIPrefab, ShopManager.instance.mainCanvas.transform);
            pricingUI.SetUI(product);
            SharedUIManager.instance.OpenMenu(pricingUI);
        }
    }

    public void UpdateProduct(ProductData product)
    {
        pricingTableManager.UpdateThisProductInfo(product);
    }

    /// <summary>
    /// Call to get the selling price of a given item
    /// </summary>
    public float GetSellingPriceOfItem(FurnitureItem item)
    {

        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            return product.price;
        }
        else
        {
            Debug.LogError("Selling price of item is wrong. Giving max value");
            return int.MaxValue;
        }
    }

    public float GetMarketPriceOfItem(FurnitureItem item)
    {

        ProductData product = allStocksInGame.Find(s => s.itemRef.Equals(item));
        if (product != null)
        {
            return product.marketPrice;
        }
        else
        {
            Debug.LogError("Market price of item is wrong. Giving max value");
            return int.MaxValue;
        }
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
