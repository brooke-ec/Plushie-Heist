using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetPricingUIFunctionality : MonoBehaviour, IUIMenu
{
    [SerializeField] private Color32 green;
    [SerializeField] private Color32 pink;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemMarketPrice;
    [SerializeField] private TMP_InputField priceInputField;
    [SerializeField] private TextMeshProUGUI margin;
    [SerializeField] private TextMeshProUGUI marginPoundText;

    private ProductData product;
    public void SetUI(ProductData product)
    {
        this.product = product;
        itemIcon.sprite = product.itemRef.inventoryIcon;
        itemName.text = product.itemRef.itemName;
        itemMarketPrice.text = product.marketPrice.ToString();
        priceInputField.text = product.price.ToString();
        margin.text = product.GetMarginAsString();
        SetMarginColour();
    }

    /// <summary>
    /// Called when price input field is modified
    /// </summary>
    public void OnChangePrice()
    {
        float newPrice;
        float.TryParse(priceInputField.text, out newPrice);

        if (newPrice < 0) {
            newPrice = 0;
        }
        product.price = newPrice;
        priceInputField.text = product.price.ToString();

        margin.text = product.GetMarginAsString();
        SetMarginColour();
    }

    public void OnSetPricing()
    {
        //not updating price because that is done when text is edited
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.UIhover);
        ShopManager.instance.stocksController.UpdateProduct(product);
        SharedUIManager.instance.CloseMenu();
    }

    private void SetMarginColour()
    {
        if (ShopManager.instance != null && ShopManager.instance.stocksController != null)
        {
            Vector2 purchaseRange = ShopManager.instance.stocksController.purchaseRange;
            print("purchase range " + purchaseRange.y);
            if (product.price <= purchaseRange.y)
            {
                margin.color = green;
                marginPoundText.color = green;
            }
            else
            {
                float priceRatio = product.price / product.marketPrice;

                float overRatio = Mathf.InverseLerp(purchaseRange.y, purchaseRange.y * 1.1f, priceRatio);
                margin.color = Color.Lerp(green, pink, overRatio);
                marginPoundText.color = Color.Lerp(green, pink, overRatio);
            }
        }
        else
        {
            //if it hasn't loaded or something then just do our usual
            if (product.price < product.marketPrice) {
                margin.color = green;
                marginPoundText.color = green;
            }
            else {
                margin.color = pink;
                marginPoundText.color = pink;
            }
        }
    }

    void IUIMenu.SetOpenState(bool open)
    {
        if (!open) Destroy(this.gameObject);
    }
}
