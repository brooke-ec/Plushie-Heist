using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyAndCustomerInfoWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI salesToday;
    [SerializeField] private TextMeshProUGUI tipsToday;
    [SerializeField] private TextMeshProUGUI customersServed;
    [SerializeField] private TextMeshProUGUI earnedToday;

    public void SetUp(Button button)
    {
        ShopManager.OnCustomerServed += UpdateWidgetInfo;
        UpdateWidgetInfo();
    }

    public void UpdateWidgetInfo()
    {
        ShopManager shopManager = ShopManager.instance;

        salesToday.text = "£"+shopManager.salesMadeToday.ToString("n2");
        tipsToday.text = "£" + shopManager.tipsReceivedToday.ToString("n2");
        customersServed.text = shopManager.numOfCustomersServed.ToString();
        earnedToday.text = "£" + shopManager.GetTotalMoneyEarnedToday().ToString("n2");
    }
}
