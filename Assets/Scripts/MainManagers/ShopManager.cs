using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the flow of things in the daytime.
/// For example, activating or deactivating the flow of customers when the shop is open/closed.
/// Or Setting things on sale
/// Or calling scripts at the beginning of a day (like with prices for stocks)
/// AND More TO-DO
/// </summary>
public class ShopManager : MonoBehaviour
{
    public Canvas mainCanvas;
    public int day;
    
    public bool isShopOpen = false;
    public static ShopManager instance { get; private set; }
    public StocksController stocksController;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.LogError("Shop manager instance already in scene");
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        shopTimer = Instantiate(shopTimerPrefab, mainCanvas.transform);
        shopTimer.transform.SetAsFirstSibling(); //so it's not in front of any UI
        shopTimer.SetupClock(true);
        StartNewDay();
    }



    #region Time

    [SerializeField] private Clock shopTimerPrefab;
    private Clock shopTimer;
    [HideInInspector] public OpenOrCloseShopButton openOrCloseShopButton;
    private void StartNewDay()
    {
        //moneyAtTheBeginningOfToday = money;
        //moneyEarnedEveryDay[day] = (money - moneyAtTheBeginningOfToday);

        day++;
        stocksController.NewDay(day);
    }

    /// <summary>
    /// Call this to start the timer and get customers to come in
    /// </summary>
    public void OpenShopToCustomers()
    {
        if(isShopOpen) { print("shop is open, cannot open"); return; }

        print("open ");
        isShopOpen = true;
        shopTimer.StartCoroutine(shopTimer.StartClock());
        //TO-DO Make clients come
    }

    /// <summary>
    /// Called by clock when the time reaches the dayEndHour, will mean that clients stop coming
    /// </summary>
    public void CloseShopToCustomers()
    {
        if (!isShopOpen) { print("shop is not open, cannot close"); return; }

        isShopOpen = false;
        shopTimer.OnTimeEnded(); //make sure it's ended

        //TO-DO MAKE CLIENTS STOP COMING

        //TO-DO moneyEarnedEveryDay[day] = (money - moneyAtTheBeginningOfToday);

        //Won't be here, as this will actually be triggered once the LAST customer is done
        //AND THEN the night is over, that's when it will be called
        //StartNewDay();
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
    #endregion

    #region Money
    private int money = 30;
    /// <summary> Set at the beginning of the day. Money - moneyAtTheBeginningOfToday is added to the end of moneyEarnedEveryDay </summary>
    //private int moneyAtTheBeginningOfToday = 0;
    public static event Action OnMoneyChanged;

    public List<int> moneyEarnedEveryDay = new List<int>();

    public int GetMoney()
    {
        return money;
    }


    public void ModifyMoney(int modification)
    {
        money += modification;
        OnMoneyChanged?.Invoke();
    }

    #endregion
}
