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
        shopTimer.SetupClock(GetTimeMultiplier(), lengthOfDayInRealMins, dayStartHour, dayEndHour);
        shopTimer.StartCoroutine(shopTimer.StartClock());
    }

    #region Time

    [SerializeField] private Clock shopTimer;
    public int day;

    [Header("Settings")]
    /// <summary> The duration of the "shop" day (dayEndHour - dayStartHour), in real-life minutes
    /// If dayStartHour is 9, and dayEndHour is 17, and this value is 2, then it will take 2 real minutes to go from 9am until 5pm
    /// </summary>
    [SerializeField] private float lengthOfDayInRealMins;
    /// <summary> at what time to start the day, 9 means 9am </summary>
    [SerializeField] private int dayStartHour = 9;
    /// <summary> at what time to end the day (close the shop), 17 means 5pm </summary>
    [SerializeField] int dayEndHour = 17;

    /// <summary>
    /// Called by clock when the time reaches the dayEndHour, will mean that clients stop coming
    /// </summary>
    public void EndShoppingDay()
    {
        //TO-DO MAKE CLIENTS STOP COMING
    }

    private float GetTimeMultiplier()
    {
        //essentially, for every real second, how many in-game minutes should pass
        float realDayDurationSecs = lengthOfDayInRealMins * 60f;

        float totalInGameMinutes = (dayEndHour - dayStartHour) * 60f;
        float timeMultiplier = totalInGameMinutes / realDayDurationSecs;

        return timeMultiplier;
    }
    #endregion
}
