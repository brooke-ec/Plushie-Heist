using TMPro;
using UnityEngine;

public class MoneyContainer : MonoBehaviour
{
    private void Start()
    {
        ShopManager.onMoneyChanged.AddListener(UpdateCoins);
        UpdateCoins();
    }
    private void UpdateCoins()
    {
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = ShopManager.instance.GetMoney().ToString("n2");
    }
}
