using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteraction : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Pick Up";
    [SerializeField] private ItemClass Item;

    public bool interact(Interactor interactor)
    {
        Debug.Log("Picked Up"+gameObject.name);
        InventoryController inventoryController = FindAnyObjectByType<InventoryController>();
        inventoryController.InsertItem(Item);
        return true;
    }

    
}
