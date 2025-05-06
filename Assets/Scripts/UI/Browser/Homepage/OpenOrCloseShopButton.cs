using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOrCloseShopButton : MonoBehaviour
{
    [SerializeField] private Sprite greenBackground;
    [SerializeField] private Sprite redBackground;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Color32 green;
    [SerializeField] private Color32 red;

    [SerializeField] private Image icon;
    [SerializeField] private Image background;

    public void SetUpOpenOrCloseButton()
    {
        ShopManager.instance.openOrCloseShopButton = this;

        Button button = GetComponent<HomepageWidget>().button;
        bool isShopOpen = ShopManager.instance.isShopOpen;
        UpdateColours();

        button.onClick.RemoveAllListeners();

        if (isShopOpen)
        {
            button.onClick.AddListener(() => ShopManager.instance.CloseShopToCustomers());
        }
        else
        {
            button.onClick.AddListener(() => ShopManager.instance.OpenShopToCustomers());
        }

        button.onClick.AddListener(() => SetUpOpenOrCloseButton());
    }

    private void UpdateColours()
    {

        if(ShopManager.instance.isShopOpen)
        {
            background.sprite = greenBackground;
            icon.color = green;
            icon.sprite = openSprite;
        }
        else
        {
            background.sprite = redBackground;
            icon.color = red;
            icon.sprite = closedSprite;
        }
    }
}
