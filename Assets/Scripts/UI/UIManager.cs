using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;

    [Header("Testing")]
    [SerializeField] private InventoryGrid gridToStart;

    //SHOULD NOT BE HERE, ONLY FOR NOW
    public static int money = 30;
    //

    private void Start()
    {
        scaleFactor = rootCanvas.scaleFactor;
        gridToStart.StartInventory();
    }
}
