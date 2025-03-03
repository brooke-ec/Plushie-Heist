using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas rootCanvas; //TO-DO should pass this to a UI manager eventually
    [HideInInspector] public float scaleFactor;

    [Header("Testing")]
    [SerializeField] private InventoryGrid gridToStart;

    private void Start()
    {
        scaleFactor = rootCanvas.scaleFactor;
        gridToStart.StartInventory();
    }
}
