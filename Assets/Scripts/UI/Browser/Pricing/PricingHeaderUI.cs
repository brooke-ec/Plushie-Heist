using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Deals with sorting of headers, and the appropriate setting of the arrow indicating if sorted before
/// </summary>
public class PricingHeaderUI : MonoBehaviour
{
    [HideInInspector] public Image arrow;
    [SerializeField] private Sprite upArrow;
    [SerializeField] private Sprite downArrow;

    /// <summary> true means sorted ascending, false sorted descending </summary>
    [HideInInspector] public bool ascending = false;
    public PricingTableManager.PricingHeaders header;

    /// <summary>
    /// Called when clicking on the pricing header. Calls the sorting method in PricingTableManager
    /// </summary>
    public void StartSorting()
    {
        FindAnyObjectByType<PricingTableManager>().SortByHeader(header);
    }

    /// <summary>
    /// Decides what Sorting method to call depending on the header
    /// </summary>
    /// <param name="products">The list of products to sort</param>
    /// <returns>Products sorted by the header</returns>
    public List<ProductData> Sort(List<ProductData> products)
    {
        switch(header)
        {
            case PricingTableManager.PricingHeaders.productName:
                products = SortByProductName(products);
                break;
            case PricingTableManager.PricingHeaders.marketPrice:
                products = SortByMarketPrice(products);
                break;
            case PricingTableManager.PricingHeaders.lastChange:
                products = SortByLastChange(products);
                break;
            case PricingTableManager.PricingHeaders.price:
                products = SortByPrice(products);
                break;
            case PricingTableManager.PricingHeaders.profit:
                products = SortByProfit(products);
                break;
            default:
                Debug.LogWarning("Problem with header being none");
                return null;
        }

        SetArrow();

        return products;
    }

    public void SetArrow()
    {
        //if not active, it means it hasn't been sorted before, therefore set it active first
        if (!arrow.gameObject.activeSelf)
        {
            arrow.gameObject.SetActive(true);
        }

        if (ascending)
        {
            arrow.sprite = upArrow;
        }
        else
        {
            arrow.sprite = downArrow;
        }

    }

    #region All sort methods
    private List<ProductData> SortByProductName(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.productName.CompareTo(p2.productName));
        }
        else
        {
            products.Sort((p, p2) => p2.productName.CompareTo(p.productName));
        }
        return products;
    }

    private List<ProductData> SortByMarketPrice(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.marketPrice.CompareTo(p2.marketPrice));
        }
        else
        {
            products.Sort((p, p2) => p2.marketPrice.CompareTo(p.marketPrice));
        }
        return products;
    }

    private List<ProductData> SortByLastChange(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.lastDayChanged.CompareTo(p2.lastDayChanged));
        }
        else
        {
            products.Sort((p, p2) => p2.lastDayChanged.CompareTo(p.lastDayChanged));
        }
        return products;
    }

    private List<ProductData> SortByPrice(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.price.CompareTo(p2.price));
        }
        else
        {
            products.Sort((p, p2) => p2.price.CompareTo(p.price));
        }
        return products;
    }

    private List<ProductData> SortByProfit(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.GetProfit().CompareTo(p2.GetProfit()));
        }
        else
        {
            products.Sort((p, p2) => p2.GetProfit().CompareTo(p.GetProfit()));
        }
        return products;
    }
    #endregion
}
