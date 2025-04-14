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
    [SerializeField] private TextMeshProUGUI marketPrice;
    [SerializeField] private TextMeshProUGUI lastDayChanged;
    [SerializeField] private TMP_InputField price;
    [SerializeField] private TextMeshProUGUI profit;

    public void Set(ProductData productData)
    {
        productDataRef = productData;
        productName.text = productDataRef.productName;
        marketPrice.text = productDataRef.marketPrice.ToString();
        lastDayChanged.text = productDataRef.GetLastDayChangedText(0); //need to change eventually to pass the actual day
        price.text = productDataRef.price.ToString();
        profit.text = productDataRef.GetProfitAsString();
    }

    /// <summary>
    /// Called when price input field is modified
    /// </summary>
    public void OnChangePrice()
    {
        float newPrice;
        float.TryParse(price.text, out newPrice);
        productDataRef.price = newPrice;

        profit.text = productDataRef.GetProfitAsString();
    }
}
