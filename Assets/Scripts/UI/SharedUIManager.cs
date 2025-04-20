using System;
using UnityEngine;

/// <summary>
/// Used ONLY for shared UI between shop and night (inventory)
/// </summary>
public class SharedUIManager : MonoBehaviour
{
    public Canvas rootCanvas;
    [HideInInspector] public float scaleFactor;

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
        scaleFactor = rootCanvas.scaleFactor;
        gridToStart.StartInventory();

        GetComponent<InventoryController>().PlaceTestItems();
    }
}
