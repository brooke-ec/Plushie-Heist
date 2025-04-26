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
        shopTimer.SetupClock();
        StartNewDay();
    }

    #region Time

    [SerializeField] private Clock shopTimerPrefab;
    private Clock shopTimer;
    private void StartNewDay()
    {
        day++;
        shopTimer.StartCoroutine(shopTimer.StartClock());

        stocksController.NewDay(day);
    }

    /// <summary>
    /// Called by clock when the time reaches the dayEndHour, will mean that clients stop coming
    /// </summary>
    public void EndShoppingDay()
    {
        //TO-DO MAKE CLIENTS STOP COMING

        //Won't be here, as this will actually be triggered once the LAST customer is done
        //AND THEN the night is over, that's when it will be called
        StartNewDay();
    }
    #endregion

    #region Interaction
    /// <summary>
    /// Call when you want to select a given item and change its price.
    /// It's assumed that the item is already in pricing table
    /// </summary>
    public void SetPriceOfItem(FurnitureItem item)
    {
        stocksController.CreateSetPricingUI(item);
    }
    #endregion

    #region Money
    private int money = 30;
    public static event Action OnMoneyChanged;

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
