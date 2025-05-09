using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PricingTableManager : MonoBehaviour
{
    public ProductRowUI rowPrefab;
    [SerializeField] private GameObject nothingToSeeYetText;
    public Transform contentTransform;
    /// <summary> Logical storage of the product data. Data might be sorted </summary>
    private List<ProductData> products = new List<ProductData>();
    /// <summary> ORIGINAL Logical storage of the product data </summary>
    [HideInInspector] public List<ProductData> originalProducts = new List<ProductData>();

    /// <summary> UI storage of data </summary>
    [HideInInspector] public List<ProductRowUI> productRows;

    #region Adding products
    /// <summary>
    /// Call to try add a new product to the table.
    /// Adds it if not already present, otherwise nothing happens.
    /// </summary>
    /// <param name="product">Item to add</param>
    /// <param name="populateTableImmediately">If true, PopulateTable() will be called immediately, otherwise it won't be done. Useful not to for performance if done in larger numbers</param>
    public void TryAddNewProduct(ProductData product, bool populateTableImmediately = true)
    {
        int todaysDay = ShopManager.instance.day;

        //if not already present in table
        if (originalProducts.Find(p => p.itemRef.Equals(product.itemRef)) == null)
        {
            product.lastDayChanged = todaysDay;
            originalProducts.Add(product);
            products.Add(product);

            if (populateTableImmediately)
            {
                PopulateTable();
            }
            print("Item " + product.itemRef.itemName + " added to pricing table");
        }
        else
        {
            print("item " + product.itemRef.itemName + " already exists in pricing table. Not added");
        }
    }

    /// <summary>
    /// Call to remove product if it exists
    /// </summary>
    /// <param name="product"></param>
    public void TryRemoveProduct(ProductData product)
    {
        if (originalProducts.Find(p => p.itemRef.Equals(product.itemRef)) != null)
        {
            originalProducts.Remove(product);
            products.Remove(product);

            PopulateTable();
        }
    }

    /// <summary>
    /// Call to try to add a list of new products to the table.
    /// Adds a given product if not already present, otherwise nothing happens.
    /// </summary>
    /// <param name="items">Items to add</param>
    public void TryAddNewProducts(List<ProductData> items)
    {
        foreach (ProductData product in items)
        {
            TryAddNewProduct(product, false);
        }
    }
    #endregion

    public void UpdateThisProductInfo(ProductData product)
    {
        ProductRowUI productRow = productRows.Find(p => p.productDataRef.Equals(product));
        if(productRow==null) { return; }

        productRow.Set(product);
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

        if(products.Count==0)
        {
            nothingToSeeYetText.SetActive(true);
        }
        else
        {
            nothingToSeeYetText.SetActive(false);
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
        margin,
        lastMarketPrice
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
