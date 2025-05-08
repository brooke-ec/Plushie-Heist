using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Used ONLY for shared UI between shop and night (inventory)
/// </summary>
public class SharedUIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;
    [HideInInspector] public IUIMenu currentMenu = null;
    [HideInInspector] public bool isMenuOpen => currentMenu != null;
    [HideInInspector] public UnityEvent onMenuClose = new UnityEvent();
    [JsonProperty("plushie")] public PlushieInfo plushie = null;
    [JsonProperty("backpack")] public InventoryGrid backpack => InventoryController.instance.backpackGrid;
    [JsonProperty("skills")] public List<Skill> unlockedSkills = new List<Skill>();
    public int plushieIndex => plushie == null ? 0 : plushie.order;
    
    private PlayerInput playerInput;

    public static SharedUIManager instance { get; private set; }

    private void Awake()
    {
        scaleFactor = rootCanvas.scaleFactor;
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerInput = FindAnyObjectByType<PlayerInput>();

        if (NightManager.instance != null)
        {
            //if nighttime
            //then turn off button of add items to storage
            //canvas, backpack, last child (button)
            rootCanvas.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        }
    }

    public void OpenMenu(IUIMenu menu)
    {
        print($"Opening Menu: {menu}");
        if (currentMenu != null) currentMenu.SetOpenState(false);
        
        currentMenu = menu;
        currentMenu.SetOpenState(true);

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("MenuActions");
        }
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void CloseMenu()
    {
        print("Closing all menus");
        onMenuClose.Invoke();

        if (currentMenu != null)
        {
            currentMenu.SetOpenState(false);
            currentMenu = null;
        }

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("PlayerMovement");
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleMenu(IUIMenu menu)
    {
        if (currentMenu != menu) OpenMenu(menu);
        else CloseMenu();
    }
}
