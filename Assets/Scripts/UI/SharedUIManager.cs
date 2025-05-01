using System;
using UnityEngine;
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
    
    private PlayerInput playerInput;

    [Header("Testing")]
    [SerializeField] private InventoryGrid gridToStart;

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
        gridToStart.StartInventory();

        GetComponent<InventoryController>().PlaceTestItems();
    }

    public void OpenMenu(IUIMenu menu)
    {
        if (currentMenu != null) currentMenu.SetOpenState(false);
        
        currentMenu = menu;
        currentMenu.SetOpenState(true);

        playerInput.SwitchCurrentActionMap("InventoryActions");
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void CloseMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetOpenState(false);
            currentMenu = null;
        }

        playerInput.SwitchCurrentActionMap("PlayerMovement");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleMenu(IUIMenu menu)
    {
        if (currentMenu != menu) OpenMenu(menu);
        else CloseMenu();
    }
}
