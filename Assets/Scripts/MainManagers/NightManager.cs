using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NightManager : MonoBehaviour
{
    //might want to make a static instance
    public static NightManager instance { get; private set; }
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(transform.parent.gameObject);
            print("Night manager already in scene, starting guards");
            FindAnyObjectByType<GaurdSpawer>().startGuards();
        }
        else
        {
            instance = this;
        }

        playerInput = FindAnyObjectByType<PlayerInput>();

        nightTimer = Instantiate(nightTimerPrefab, nightUICanvas.transform);
        nightTimer.transform.SetAsFirstSibling(); //so it's not in front of any UI
        nightTimer.enabled = false;
    }

    private void Start()
    {
        LoadNight();
    }

    /// <summary>
    /// The percentage of items to lose. Will need to read from saved file to get the itemLosePercentage from skills obtained.
    /// In % form (E.g: 20 for 20%)
    /// </summary>
    public int itemLosePercentage;

    [SerializeField] private GameObject nightIntroUIPrefab;
    [SerializeField] private ChooseAnAbilityUI chooseAbilityUIPrefab;

    public int levelProgress = 0;

    private PlayerInput playerInput;

    public void LoadNight()
    {
        //load UI saying what the night is about, and the continue button calls StartNight()
        playerInput.SwitchCurrentActionMap("MenuActions");
        SharedUIManager.instance.menusDisabled = true;
        Cursor.lockState = CursorLockMode.None;
        GameObject nightIntroUI = Instantiate(nightIntroUIPrefab, nightUICanvas.transform);
        nightIntroUI.transform.GetChild(3).GetComponentInChildren<Button>().onClick.AddListener(() => {
                Instantiate(chooseAbilityUIPrefab, nightUICanvas.transform);
                Destroy(nightIntroUI);
            }
        );

        LoadPlushieIndicator(SharedUIManager.instance.nextPlushie);
    }

    /// <summary>
    /// Called after choosing an ability to be active
    /// </summary>
    public void StartNight()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInput.SwitchCurrentActionMap("PlayerMovement");
        FindAnyObjectByType<GaurdSpawer>().startGuards();
        print("night started");

        nightTimer.enabled = true;
        nightTimer.SetupClock(false);

        //start clock
        nightTimer.StartCoroutine(nightTimer.StartClock());
    }

    /// <summary>
    /// Either called by the clock ending or the player going through the exit door
    /// </summary>
    /// <param name="successful">Pass true if the player leaves through the door</param>
    public void OnEndNight(bool successful)
    {
        print("night ENDED");

        nightTimer.StopAllCoroutines();

        //even if you rescue it, if caught then you lose it
        if (!successful) { hasRescuedPlushie = false; }
        SharedUIManager.instance.menusDisabled = true;
        playerInput.SwitchCurrentActionMap("MenuActions");
        Cursor.lockState= CursorLockMode.None;
        AudioManager.instance.PlayMusic(AudioManager.MusicEnum.endOfNight);
        FindAnyObjectByType<GaurdSpawer>()?.stopGuards();

        //Call end stuff
        if(!hasRescuedPlushie)
        {
            escapingUI.CreateEscapingUI(successful, nightUICanvas.transform, null);
        }
        else
        {
            escapingUI.CreateEscapingUI(successful, nightUICanvas.transform, SharedUIManager.instance.nextPlushie);
        }
    }

    public void UpdateClockTime(float extraTimeInMins)
    {
        nightTimer.UpdateClockTime(extraTimeInMins);
    }

    #region UI
    [SerializeField] private Clock nightTimerPrefab;
    public Clock nightTimer;
    public Canvas nightUICanvas;

    [SerializeField] private EscapingUI escapingUI;

    [SerializeField] private Image plushieIcon;
    [HideInInspector] public bool hasRescuedPlushie = false;

    private void LoadPlushieIndicator(PlushieInfo plushieInfo=null)
    {
        if(plushieInfo != null) { plushieIcon.sprite = plushieInfo.icon; }
        if (hasRescuedPlushie)
        {
            plushieIcon.color = Color.white;
        }
        else {
            plushieIcon.color = new Color(1, 1, 1, 0.5f);
        }
    }

    /// <summary>
    /// Call when plushie is rescued
    /// </summary>
    public void OnRescuePlushie()
    {
        hasRescuedPlushie = true;
        LoadPlushieIndicator();
    }
    #endregion
}
