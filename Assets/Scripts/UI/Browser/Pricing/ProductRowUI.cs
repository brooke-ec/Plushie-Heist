using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProductRowUI : MonoBehaviour
{
    public ProductData productDataRef;

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private TextMeshProUGUI lastMarketPrice;
    [SerializeField] private TextMeshProUGUI lastDayChanged;
    [SerializeField] private TextMeshProUGUI marketPrice;
    [SerializeField] private TMP_InputField price;
    [SerializeField] private TextMeshProUGUI margin;

    [Header("Colours")]
    [SerializeField] private Color32 greenColour;
    [SerializeField] private Color32 pinkColour;

    public void Set(ProductData productData)
    {
        productDataRef = productData;
        productName.text = productDataRef.itemRef.itemName;
        marketPrice.text = "£"+productDataRef.marketPrice.ToString("F2");
        lastMarketPrice.text = "£" +productDataRef.lastMarketPrice.ToString("F2");
        lastDayChanged.text = productDataRef.GetLastDayChangedText();
        price.text = productDataRef.price.ToString("F2");
        margin.text = "£"+productDataRef.GetMarginAsString();
        icon.sprite = productData.itemRef.inventoryIcon;

        ChangeMarginsColour();
    }

    /// <summary>
    /// Called when price input field is modified
    /// </summary>
    public void OnChangePrice()
    {
        float newPrice;
        float.TryParse(price.text, out newPrice);
        if(newPrice<0) { newPrice = 0; }

        productDataRef.price = newPrice;
        price.text = productDataRef.price.ToString();

        margin.text = "£"+productDataRef.GetMarginAsString();
        ChangeMarginsColour();
    }

    private void ChangeMarginsColour()
    {
        if (productDataRef.GetMargin() >= 0) { margin.color = greenColour; }
        else { margin.color = pinkColour; }
    }
}
