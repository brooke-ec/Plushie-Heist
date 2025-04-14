using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteraction : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Pick Up";
    [SerializeField] private ItemClass ItemClass;

    public bool interact(Interactor interactor)
    {
        Debug.Log("Picked Up"+gameObject.name);
        return true;
    }

    
}
