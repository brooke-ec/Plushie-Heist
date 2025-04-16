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
        shopTimer.SetupClock();
        shopTimer.StartCoroutine(shopTimer.StartClock());
    }

    #region Time

    [SerializeField] private Clock shopTimer;
    public int day;

    /// <summary>
    /// Called by clock when the time reaches the dayEndHour, will mean that clients stop coming
    /// </summary>
    public void EndShoppingDay()
    {
        //TO-DO MAKE CLIENTS STOP COMING
    }
    #endregion
}
