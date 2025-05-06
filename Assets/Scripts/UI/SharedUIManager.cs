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
    
    private PlayerInput playerInput;

    public static SharedUIManager instance { get; private set; }

    private void Awake()
    {
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
        scaleFactor = rootCanvas.scaleFactor;

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
