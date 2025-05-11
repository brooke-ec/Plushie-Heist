using UnityEngine;

[System.Serializable]
public class ProductData
{
    public FurnitureItem itemRef;
    public int lastDayChanged;
    [HideInInspector] public float marketPrice;
    [HideInInspector] public float lastMarketPrice;
    public float price;

    public ProductData() { }

    public ProductData(FurnitureItem item, int todaysDay)
    {
        itemRef = item;
        lastMarketPrice = item.marketPrice;
        marketPrice = item.marketPrice;
        lastDayChanged = todaysDay;
        price = marketPrice;
    }

    public string GetLastDayChangedText()
    {
        int todaysDay = ShopManager.instance.day;

        if (lastDayChanged == todaysDay)
        {
            return "Today";
        }
        else if (todaysDay - 1 == lastDayChanged)
        {
            return "Yesterday";
        }
        else
        {
            return Mathf.Abs(todaysDay - lastDayChanged).ToString() + " days ago";
        }
    }

    public float GetMargin()
    {
        return price - marketPrice;
    }

    /// <summary>
    /// Gets margin as a 2 decimal number as a string
    /// </summary>
    /// <returns>Profit as a 2 decimal number as a string</returns>
    public string GetMarginAsString()
    {
        return (-GetMargin()).ToString("n2");
    }
}
