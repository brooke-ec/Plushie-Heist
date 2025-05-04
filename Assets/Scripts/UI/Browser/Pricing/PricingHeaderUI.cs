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
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIclick3);
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
            case PricingTableManager.PricingHeaders.margin:
                products = SortByMargin(products);
                break;
            case PricingTableManager.PricingHeaders.lastMarketPrice:
                products = SortByLastMarketPrice(products);
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
            products.Sort((p, p2) => p.itemRef.itemName.CompareTo(p2.itemRef.itemName));
        }
        else
        {
            products.Sort((p, p2) => p2.itemRef.itemName.CompareTo(p.itemRef.itemName));
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

    private List<ProductData> SortByLastMarketPrice(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.lastMarketPrice.CompareTo(p2.marketPrice));
        }
        else
        {
            products.Sort((p, p2) => p2.lastMarketPrice.CompareTo(p.marketPrice));
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

    private List<ProductData> SortByMargin(List<ProductData> products)
    {
        ascending = !ascending;

        if (ascending)
        {
            products.Sort((p, p2) => p.GetMargin().CompareTo(p2.GetMargin()));
        }
        else
        {
            products.Sort((p, p2) => p2.GetMargin().CompareTo(p.GetMargin()));
        }
        return products;
    }
    #endregion
}
