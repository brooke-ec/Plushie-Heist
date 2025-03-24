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
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI profit;

    public void Set(ProductData productData)
    {
        productDataRef = productData;
        productName.text = productDataRef.productName;
        marketPrice.text = productDataRef.marketPrice.ToString();
        lastDayChanged.text = productDataRef.GetLastDayChangedText(0); //need to change eventually to pass the actual day
        price.text = productDataRef.price.ToString();
        profit.text = productDataRef.profit.ToString();
    }
}
