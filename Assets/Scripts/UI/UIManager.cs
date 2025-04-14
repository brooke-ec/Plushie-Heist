using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;

    [Header("Testing")]
    [SerializeField] private InventoryGrid gridToStart;

    //SHOULD NOT BE HERE, ONLY FOR NOW
    private int money = 30;
    public static event Action OnMoneyChanged;
    //

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
