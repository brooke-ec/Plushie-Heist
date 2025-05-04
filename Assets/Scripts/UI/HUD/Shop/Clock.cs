using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    /// <summary> Secs from dayStartHour </summary>
    [SerializeField] private Image coloredTop;
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    public Gradient gradient;

    [Header("Settings")]
    /// <summary> The duration of the "activity" day (dayEndHour - dayStartHour), in real-life minutes
    /// If dayStartHour is 9, and dayEndHour is 17, and this value is 2, then it will take 2 real minutes to go from 9am until 5pm
    /// </summary>
    [SerializeField] private float lengthOfDayInRealMins;
    /// <summary> at what time to start the day, 9 means 9am </summary>
    private int dayStartHour;
    /// <summary> at what time to end the day (close the shop), 17 means 5pm </summary>
    int dayEndHour;
    /// <summary> If true, behaves as the shop clock, calling the equivalent end function. Otherwise, behaves as the night clock </summary>
    private bool isShopClock = true;

    private float totalDayTimeSeconds; // in seconds
    private float timeMultiplier;

    private float GetTimeMultiplier()
    {
        //essentially, for every real second, how many in-game minutes should pass
        float realDayDurationSecs = lengthOfDayInRealMins * 60f;

        float totalInGameMinutes = GetTimeDifference() * 60f;
        float timeMultiplier = totalInGameMinutes / realDayDurationSecs;

        return timeMultiplier;
    }

    public void SetupClock(bool isShopClock)
    {
        print("clock setup");

        this.isShopClock = isShopClock;
        if (isShopClock)
        {
            GetComponent<Button>().onClick.AddListener(() => TryCloseEarly());
            dayStartHour = 9;
            dayEndHour = 17;
        }
        else
        {
            dayStartHour = 21;
            dayEndHour = 5;
        }

        timeMultiplier = GetTimeMultiplier();
        totalDayTimeSeconds = lengthOfDayInRealMins * 60f;

        SetClockUI(0);
    }

    public void UpdateClockTime(float extraTimeInMins)
    {
        if (!clockCurrentlyRunning)
        {
            lengthOfDayInRealMins += extraTimeInMins;

            timeMultiplier = GetTimeMultiplier();
            totalDayTimeSeconds = lengthOfDayInRealMins * 60f;
        }
    }

    float elapsedTime = 0;

    public IEnumerator StartClock()
    {
        clockCurrentlyRunning = true;
        elapsedTime = 0;
        while(elapsedTime < totalDayTimeSeconds)
        {
            elapsedTime += Time.deltaTime;

            float inGameTime = elapsedTime * timeMultiplier;

            SetClockUI(inGameTime);
            yield return null;
        }

        OnTimeEnded();
    }

    /// <summary>
    /// to stop on time ended potentially running twice when the coroutine is stopped
    /// </summary>
    private bool clockCurrentlyRunning = false;

    public void OnTimeEnded()
    {
        if(!clockCurrentlyRunning) { return; }

        //TO-DO play a sound
        StopCoroutine(StartClock());
        clockCurrentlyRunning = false;

        elapsedTime = totalDayTimeSeconds;

        SetClockUI(elapsedTime * timeMultiplier);

        if(isShopClock)
        {
            ShopManager.instance.CloseShopToCustomers();
            ShopManager.instance.openOrCloseShopButton.SetUpOpenOrCloseButton();
            Debug.LogWarning("End of day");
        }
        else
        {
            Debug.LogWarning("End of night");
            //because not successful if it's because of timer ending
            NightManager.instance.OnEndNight(false);
        }
    }

    private void SetClockUI(float inGameMinutes)
    {
        float totalInGameMinutes = GetTimeDifference() * 60f;
        float val = 1 - (inGameMinutes / totalInGameMinutes);

        Color colour = gradient.Evaluate(val);
        coloredTop.color = colour;
        timerText.color = colour;
        timerFill.color = colour;
        timerFill.fillAmount = val;

        //Display time
        (int hour, int minute) = GetCurrentInGameTime();
        if(minute%5==0)
        {
            timerText.text = string.Format("{0:00}:{1:00}", hour, minute);
        }
    }

    public (int hour, int minute) GetCurrentInGameTime()
    {
        float inGameMinutes = elapsedTime * timeMultiplier;
        int totalMinutes = Mathf.FloorToInt(inGameMinutes);
        int hour = (int)(dayStartHour + (totalMinutes / 60));

        if(hour>=24)
        {
            hour = hour - 24;
        }

        int minute = totalMinutes % 60;
        return (hour, minute);
    }

    private int GetTimeDifference()
    {
        //it means it goes from like 22 until like 3
        if(dayEndHour - dayStartHour < 0)
        {
            return dayEndHour - dayStartHour + 24;
        }
        return dayEndHour - dayStartHour;
    }

    /// <summary>
    /// Called by the onclick method in the clock (as a button) to try to close early
    /// </summary>
    public void TryCloseEarly()
    {
        if(GetCurrentInGameTime().hour == dayEndHour) { return; }

        OnTimeEnded();
    }
}
