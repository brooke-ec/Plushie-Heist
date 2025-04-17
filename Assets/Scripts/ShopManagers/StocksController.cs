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
    public static StocksController instance { get; private set; }
    private PricingTableManager pricingTableManager;

    [SerializeField] private List<ItemClass> allItemsInGame = new List<ItemClass>();
    [HideInInspector] public List<ProductData> allStocksInGame;

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
            //TO-DO THIS pricingTableManager.TryAddNewProduct(product);
            pricingTableManager.TryAddNewProduct(product);
        }
    }

    /// <summary>
    /// Goes through all stocks and randomise a bit some items' market price
    /// </summary>
    /// <param name="day">The number of today's day</param>
    public void NewDay(int day)
    {
        //TO-DO go through all stocks and randomise a bit some items market price
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
        
        if(productsToChange.Count>=1)
        {
            pricingTableManager.ResetTableToOriginalOrder();
        }
    }

    private float RoundToRetailPrice(float price)
    {
        float[] allowedEndings = { 0.00f, 0.50f, 0.75f, 0.95f };

        float basePart = Mathf.Floor(price);
        float decimalPart = price - basePart;

        float closestEnding = allowedEndings.OrderBy(ending => Mathf.Abs(decimalPart - ending)).First();

        return basePart + closestEnding;
    }
}
