using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductRowUI : MonoBehaviour
{
    private ProductData productDataRef;

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private TextMeshProUGUI lastMarketPrice;
    [SerializeField] private TextMeshProUGUI lastDayChanged;
    [SerializeField] private TextMeshProUGUI marketPrice;
    [SerializeField] private TMP_InputField price;
    [SerializeField] private TextMeshProUGUI profit;

    public void Set(ProductData productData)
    {
        productDataRef = productData;
        productName.text = productDataRef.itemRef.itemName;
        marketPrice.text = "£"+productDataRef.marketPrice.ToString("F2");
        lastMarketPrice.text = "£" +productDataRef.lastMarketPrice.ToString("F2");
        lastDayChanged.text = productDataRef.GetLastDayChangedText(); //need to change eventually to pass the actual day
        price.text = productDataRef.price.ToString("F2");
        profit.text = "£"+productDataRef.GetMarginAsString();
        icon.sprite = productData.itemRef.itemIcon;
    }

    /// <summary>
    /// Called when price input field is modified
    /// </summary>
    public void OnChangePrice()
    {
        float newPrice;
        float.TryParse(price.text, out newPrice);
        productDataRef.price = newPrice;

        profit.text = productDataRef.GetMarginAsString();
    }
}
