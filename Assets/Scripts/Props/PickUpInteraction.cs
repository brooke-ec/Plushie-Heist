using cakeslice;
using UnityEngine;

/// <summary>
/// Interaction for Picking items up, implements the IInteractable Interface</br>
/// Object this is on needs a mesh collider and to be on the interactable layer
/// </summary>
[RequireComponent(typeof(Outline))]
public class PickUpInteraction : MonoBehaviour, IInteractable
{
    /// <summary>Prompt Shown by the UI to let the player know they can interact with it</summary>
    public string interactionPrompt => hasSpace ? "Press F to Pick Up" : "Inventory Full!";
    /// <summary>
    /// The item class the Object is attachted to 
    /// </summary>
    [SerializeField] private ItemClass Item;
    /// <summary> The current <see cref="InventoryController"/> instance </summary>
    private InventoryController inventoryController;
    /// <summary> The outline script attached to this object </summary>
    private Outline outline;
    /// <summary> If there is space in the players inventory </summary>
    private bool hasSpace = true;

    private void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        outline = GetComponent<Outline>();

        inventoryController.onChanged.AddListener(() =>
        {
            hasSpace = inventoryController.CanInsert(Item);
            outline.color = hasSpace ? 0 : 1;
        });
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    /// <returns>True if picked up item<returns>
    public bool interact(Interactor interactor)
    {
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
