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
    public int day;
    public static ShopManager instance { get; private set; }

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
        StartNewDay();
    }

    private void StartNewDay()
    {
        day++;

        Clock shopTimer = Instantiate(shopTimerPrefab, UIManager.instance.rootCanvas.transform);
        shopTimer.SetupClock();
        shopTimer.StartCoroutine(shopTimer.StartClock());

        StocksController.instance.NewDay(day);
    }

    #region Time

    [SerializeField] private Clock shopTimerPrefab;

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
}
