using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class PricingTableManager : MonoBehaviour
{
    public ProductRowUI rowPrefab;
    public Transform contentTransform;
    /// <summary> Logical storage of the product data. Data might be sorted </summary>
    private List<ProductData> products;

    /// <summary> UI storage of data </summary>
    [HideInInspector] public List<ProductRowUI> productRows;

    /// <summary> ORIGINAL Logical storage of the product data </summary>
    public List<ProductData> originalProducts;

    private void Start()
    {
        products = new List<ProductData>(originalProducts.Count);
        //Assign reference to each product data in originalProducts to products. This is so that if a ProductData is changed, like price, then it's still updated on the originalProducts
        //but they keep their own ordering
        for(int i=0;i<originalProducts.Count; i++)
        {
            products.Add(originalProducts[i]);
        }
        PopulateTable();
    }

    /// <summary>
    /// Call to add a new product to the table
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="populateTableImmediately">If true, PopulateTable() will be called immediately, otherwise it won't be done. Useful not to for performance if done in larger numbers</param>
    public void AddNewProduct(ItemClass item, bool populateTableImmediately=true)
    {
        //TO-DO MISSING GETTING TODAY'S ACTUAL DAY
        int todaysDay = 0;
        ProductData newProduct = new ProductData(item.itemIcon, item.itemName, item.marketPrice, todaysDay);
        originalProducts.Add(newProduct);
        products.Add(newProduct);

        if (populateTableImmediately)
        {
            PopulateTable();
        }
    }

    /// <summary>
    /// Call to add a list of new products to the table
    /// </summary>
    /// <param name="items">Items to add</param>
    public void AddNewProducts(List<ItemClass> items)
    {
        foreach(ItemClass item in items)
        {
            AddNewProduct(item, false);
        }
    }


    private void PopulateTable()
    {
        //Clear previous rows
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i).gameObject);
        }

        foreach (ProductData product in products)
        {
            ProductRowUI row = Instantiate(rowPrefab, contentTransform);
            row.Set(product);
            productRows.Add(row);
        }
    }



    #region Area for sorting with buttons
    [SerializeField] private Transform pricingHeaderUIsTransform;
    public enum PricingHeaders
    {
        productName,
        marketPrice,
        lastChange,
        price,
        profit
    }

    public void SortByHeader(PricingHeaders header)
    {
        //to ignore the icon one
        for (int i = 1; i < pricingHeaderUIsTransform.childCount; i++)
        {
            PricingHeaderUI pricingHeaderUI = pricingHeaderUIsTransform.GetChild(i).GetComponent<PricingHeaderUI>();
            if (pricingHeaderUI == null) { Debug.LogWarning("pricing header null"); return; }

            //once header is found, sort by that

            if (pricingHeaderUI.header.Equals(header)) {
                products = pricingHeaderUI.Sort(products);
                break;
            }
        }
        PopulateTable();
    }

    public void ResetTableToOriginalOrder()
    {
        products = new List<ProductData>(originalProducts);

        //Now set all arrows deactivated

        //starts from 1 to ignore the icon one
        for (int i = 1; i < pricingHeaderUIsTransform.childCount; i++)
        {
            PricingHeaderUI pricingHeaderUI = pricingHeaderUIsTransform.GetChild(i).GetComponent<PricingHeaderUI>();
            if (pricingHeaderUI == null) { Debug.LogWarning("pricing header null"); return; }

            //once header is found, sort by that

            pricingHeaderUI.ascending = false;
            pricingHeaderUI.SetArrow();
            pricingHeaderUI.arrow.gameObject.SetActive(false);
        }

        PopulateTable();
    }
    #endregion
}
