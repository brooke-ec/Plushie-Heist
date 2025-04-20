using UnityEngine;

/// <summary>
/// Interaction for Picking items up, implements the IInteractable Interface</br>
/// Object this is on needs a mesh collider and to be on the interactable layer
/// </summary>
public class PickUpInteraction : MonoBehaviour, IInteractable
{
    /// <summary>Prompt Shown by the UI to let the player know they can interact with it</br>
    /// Default: "Press F to Pick Up"</summary>
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Pick Up";
    /// <summary>
    /// The item class the Object is attachted to 
    /// </summary>
    [SerializeField] private ItemClass Item;

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    /// <returns>True if picked up item<returns>
    public bool interact(Interactor interactor)
    {
        
        InventoryController inventoryController = FindAnyObjectByType<InventoryController>();
        if (inventoryController.InsertItem(Item))
        {
            Destroy(gameObject);
            Debug.Log("Picked Up" + gameObject.name);
            return true;
        }
        else { 
            Debug.Log("Can't Pick up"+gameObject.name);
            return false;
        }
        
    }

    
}
