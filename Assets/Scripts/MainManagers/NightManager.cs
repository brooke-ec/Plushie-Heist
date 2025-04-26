using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightManager : MonoBehaviour
{
    //might want to make a static instance
    public static NightManager instance { get; private set; }
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this);
            print("Night manager already in scene");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //StartNight();
    }

    /// <summary>
    /// The percentage of items to lose. Will need to read from saved file to get the itemLosePercentage from skills obtained.
    /// In % form (E.g: 20 for 20%)
    /// </summary>
    public int itemLosePercentage;

    public void LoadNight()
    {
        //TO-DO probably load ikea procedural stuff etc

        //TO-DO load UI saying what the night is about, and the continue button calls StartNight()
    }
    public void StartNight()
    {
        nightTimer = Instantiate(nightTimerPrefab, nightUICanvas.transform);
        nightTimer.transform.SetAsFirstSibling(); //so it's not in front of any UI
        nightTimer.SetupClock(false);

        //TO-DO start clock
        nightTimer.StartCoroutine(nightTimer.StartClock());

        //Start movement of guards?
    }

    /// <summary>
    /// Either called by the clock ending or the player going through the exit door
    /// </summary>
    /// <param name="successful">Pass true if the player leaves through the door</param>
    public void OnEndNight(bool successful)
    {
        //Call end stuff
        escapingUI.CreateEscapingUI(successful, nightUICanvas.transform);

    }

    #region UI
    [SerializeField] private Clock nightTimerPrefab;
    private Clock nightTimer;
    public Canvas nightUICanvas;

    [SerializeField] private EscapingUI escapingUI;
    #endregion
}
