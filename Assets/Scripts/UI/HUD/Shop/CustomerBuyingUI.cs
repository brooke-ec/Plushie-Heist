using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomerBuyingUI : MonoBehaviour, IUIMenu
{
    [SerializeField] private TextMeshProUGUI totalMoneyText;
    [SerializeField] private TextMeshProUGUI tipsText;
    [SerializeField] private Transform itemsBuyingContainer;
    [SerializeField] private Button continueButton;

    [SerializeField] private GameObject itemPrefab;

    UnityAction closeAction;

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

        SharedUIManager.instance.OpenMenu(this);
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.selling);
        
        continueButton.onClick.AddListener(SharedUIManager.instance.CloseMenu);
        closeAction = () =>
        {
            ShopManager.instance.OnCustomerBuying(totalMoney, tips);
            AudioManager.instance.PlaySound(AudioManager.SoundEnum.coins);
            actionForButton();
        };
    }

    void IUIMenu.SetOpenState(bool open)
    {
        if (!open)
        {
            closeAction.Invoke();
            Destroy(gameObject);
        }
    }

    private float GenerateTip(float totalMoney)
    {
        return totalMoney * ShopManager.instance.tipPercentage;
    }
}
