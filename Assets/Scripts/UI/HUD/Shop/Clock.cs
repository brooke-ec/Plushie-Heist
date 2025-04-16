using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    /// <summary>
    /// Secs from dayStartHour
    /// </summary>
    private float totalDayTimeSeconds; // in seconds
    private float timeMultiplier;
    private float dayStartHour;
    private float dayEndHour;

    [SerializeField] private Image coloredTop;
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    public Gradient gradient;


    public void SetupClock(float timeMultiplier, float lengthOfDayInRealMins, int dayStartHour, int dayEndHour)
    {
        this.timeMultiplier = timeMultiplier;
        this.dayStartHour = dayStartHour;
        this.dayEndHour = dayEndHour;

        totalDayTimeSeconds = lengthOfDayInRealMins * 60f;

        SetClockUI(dayStartHour);
    }

    float elapsedTime = 0;

    public IEnumerator StartClock()
    {
        while(elapsedTime < totalDayTimeSeconds)
        {
            elapsedTime += Time.deltaTime;

            float inGameTime = elapsedTime * timeMultiplier;

            SetClockUI(inGameTime);
            yield return null;
        }

        OnTimeEnded();
    }

    private void OnTimeEnded()
    {
        //TO-DO play a sound
        elapsedTime = totalDayTimeSeconds;

        SetClockUI(elapsedTime * timeMultiplier);

        Debug.LogWarning("End of day");
    }

    private void SetClockUI(float inGameMinutes)
    {
        float totalInGameMinutes = (dayEndHour - dayStartHour) * 60f;
        float val = 1 - (inGameMinutes / totalInGameMinutes);

        Color colour = gradient.Evaluate(val);
        coloredTop.color = colour;
        timerText.color = colour;
        timerFill.color = colour;
        timerFill.fillAmount = val;

        //Display time
        (int hour, int minute) = GetCurrentInGameTime();

        timerText.text = string.Format("{0:00}:{1:00}", hour, minute);
    }

    public (int hour, int minute) GetCurrentInGameTime()
    {
        float inGameMinutes = elapsedTime * timeMultiplier;
        int totalMinutes = Mathf.FloorToInt(inGameMinutes);
        int hour = (int)(dayStartHour + (totalMinutes / 60));
        int minute = totalMinutes % 60;
        return (hour, minute);
    }

    /// <summary>
    /// Called by the onclick method in the clock (as a button) to try to close early
    /// </summary>
    public void TryCloseEarly()
    {
        if(GetCurrentInGameTime().hour == dayEndHour) { return; }

        StopCoroutine(StartClock());
        OnTimeEnded();
    }
}
