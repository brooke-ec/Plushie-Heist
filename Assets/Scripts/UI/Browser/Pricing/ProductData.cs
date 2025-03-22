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
    public float profit;

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
            return (todaysDay - lastDayChanged).ToString() + " days ago";
        }
    }
}
