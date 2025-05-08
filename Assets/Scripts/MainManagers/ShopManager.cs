using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the flow of things in the daytime.
/// For example, activating or deactivating the flow of customers when the shop is open/closed.
/// Or Setting things on sale
/// Or calling scripts at the beginning of a day (like with prices for stocks)
/// AND More TO-DO
/// </summary>
public class ShopManager : MonoBehaviour
{
    [JsonProperty("storage")] public InventoryGrid storage => InventoryController.instance.storageGrid;
    [JsonProperty("hasShopBeenOpenToday")] private bool hasShopBeenOpenToday = false;
    [JsonProperty("stock")] public StocksController stocksController;
    [JsonProperty("layout")] private ShopLayout layout;
    [JsonProperty("money")] private float money = 0;
    [JsonProperty("day")] public int day;

    [SerializeField] private ShopLayout[] layouts;

    public Canvas mainCanvas;
    public bool isShopOpen = false;
    public static ShopManager instance { get; private set; }

    public ShopLayout SetLayout(int level)
    {
        if (layout != null) Destroy(layout);

        layout = Instantiate(layouts[level]);
        layout.level = level;
        return layout;
    }

    private void Awake()
    {
        SaveManager.onLoaded.AddListener(() => { if (layout == null) SetLayout(0); });
        if (instance != null)
        {
            Destroy(this);
            Debug.LogError("Shop manager instance already in scene");
        }
        else
        {
            instance = this;
        }

        // Not good practice
        shopTimer = Instantiate(shopTimerPrefab, mainCanvas.transform);
        shopTimer.transform.SetAsFirstSibling(); //so it's not in front of any UI
        shopTimer.SetupClock(true);
    }

    private void Start()
    {
        StartNewDay();
    }

    #region Time

    [SerializeField] private Clock shopTimerPrefab;
    private Clock shopTimer;
    [HideInInspector] public OpenOrCloseShopButton openOrCloseShopButton;
    private void StartNewDay()
    {
        day++;
        hasShopBeenOpenToday = false;
        stocksController.NewDay(day);
    }

    /// <summary>
    /// Call this to start the timer and get customers to come in
    /// </summary>
    public void OpenShopToCustomers()
    {
        if(isShopOpen || hasShopBeenOpenToday) { print("cannot open"); return; }

        AudioManager.instance.PlaySound(AudioManager.SoundEnum.bell);

        print("open ");
        isShopOpen = true;
        hasShopBeenOpenToday = true;
        shopTimer.StartCoroutine(shopTimer.StartClock());
    }

    /// <summary>
    /// Called by clock when the time reaches the dayEndHour, will mean that clients stop coming
    /// </summary>
    public void CloseShopToCustomers()
    {
        if (!isShopOpen) { print("shop is not open, cannot close"); return; }

        AudioManager.instance.PlaySound(AudioManager.SoundEnum.lowPitchBell);
        isShopOpen = false;
        shopTimer.OnTimeEnded(); //make sure it's ended
    }
    #endregion

    #region Interaction
    /// <summary>
    /// Call when you want to select a given item and change its price.
    /// It's assumed that the item is already in pricing table
    /// </summary>
    public void SetPriceOfItem(FurnitureController item)
    {
        stocksController.CreateSetPricingUI(item);
    }

    public void UpdateClockTime(float extraTimeInMins)
    {
        if(!isShopOpen)
        {
            shopTimer.UpdateClockTime(extraTimeInMins);
        }
    }
    #endregion

    #region Money
    public static event Action OnMoneyChanged;

    public float GetMoney()
    {
        return money;
    }


    public void ModifyMoney(float modification)
    {
        money += modification;
        OnMoneyChanged?.Invoke();
    }
    #endregion

    #region Customers
    [HideInInspector] public float tipPercentage = 0;
    [HideInInspector] public float itemBuyingMultiplier = 1;

    [HideInInspector] public int numOfCustomersServed = 0;
    [HideInInspector] public float tipsReceivedToday = 0;
    [HideInInspector] public float salesMadeToday = 0;

    [SerializeField] private CustomerBuyingUI customerBuyingUIPrefab;

    public static event Action OnCustomerServed;
    public void CreateCustomerBuyingUI(List<FurnitureItem> basket, UnityAction actionForButton)
    {
        CustomerBuyingUI customerBuyingUI = Instantiate(customerBuyingUIPrefab, mainCanvas.transform);
        customerBuyingUI.SetUp(basket, actionForButton);
    }

    public float GetTotalMoneyEarnedToday()
    {
        return tipsReceivedToday + salesMadeToday;
    }

    public void OnCustomerBuying(float salesMade, float tipsReceived)
    {
        salesMadeToday += salesMade;
        tipsReceivedToday += tipsReceived;
        numOfCustomersServed++;

        ModifyMoney(salesMade + tipsReceived);

        OnCustomerServed?.Invoke();
    }

    #endregion
}
