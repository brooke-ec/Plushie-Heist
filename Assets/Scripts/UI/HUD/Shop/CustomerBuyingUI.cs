using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomerBuyingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalMoneyText;
    [SerializeField] private TextMeshProUGUI tipsText;
    [SerializeField] private Transform itemsBuyingContainer;
    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject itemPrefab;

    public void SetUp(List<FurnitureItem> basket, UnityAction actionForButton)
    {
        float totalMoney = 0;

        foreach(FurnitureItem item in basket)
        {
            float price = ShopManager.instance.stocksController.GetSellingPriceOfItem(item);
            totalMoney += price;

            GameObject itemContainer = Instantiate(itemPrefab, itemsBuyingContainer);
            itemContainer.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = item.inventoryIcon;
            itemContainer.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "£" + price.ToString("n2");
        }

        float tips = GenerateTip(totalMoney);
        totalMoneyText.text = "Total: £"+totalMoney.ToString("n2");
        tipsText.text = "Tips: £" + tips.ToString("n2");

        continueButton.onClick.AddListener(() => AudioManager.instance.PlaySound(AudioManager.SoundEnum.selling));
        continueButton.onClick.AddListener(actionForButton);
        continueButton.onClick.AddListener(() => ShopManager.instance.OnCustomerBuying(totalMoney, tips));
        continueButton.onClick.AddListener(() => Destroy(gameObject));
    }

    private float GenerateTip(float totalMoney)
    {
        //ShopManager.instance.tipPercentage
        return 0;
    }
}
