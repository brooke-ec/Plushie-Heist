using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;

    [Header("Testing")]
    [SerializeField] private InventoryGrid gridToStart;

    public static UIManager instance { get; private set; }

    //SHOULD NOT BE HERE, ONLY FOR NOW
    private int money = 30;
    public static event Action OnMoneyChanged;
    //

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
        scaleFactor = rootCanvas.scaleFactor;
        gridToStart.StartInventory();
    }

    public int GetMoney()
    {
        return money;
    }

    public void ModifyMoney(int modification)
    {
        money += modification;
        OnMoneyChanged?.Invoke();
    }
}
