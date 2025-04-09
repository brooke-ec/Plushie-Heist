using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductData
{
    public Sprite icon;
    public string productName;
    public float marketPrice;
    public int lastDayChanged;
    public float price;

    public ProductData(Sprite icon, string productName, float marketPrice, int todaysDay)
    {
        this.icon = icon;
        this.productName = productName;
        this.marketPrice = marketPrice;
        this.lastDayChanged = todaysDay;
        this.price = marketPrice;
    }

    public string GetLastDayChangedText(int todaysDay)
    {
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

    public float GetProfit()
    {
        return price - marketPrice;
    }

    /// <summary>
    /// Gets profit as a 2 decimal number as a string
    /// </summary>
    /// <returns>Profit as a 2 decimal number as a string</returns>
    public string GetProfitAsString()
    {
        return GetProfit().ToString("n2");
    }
}
