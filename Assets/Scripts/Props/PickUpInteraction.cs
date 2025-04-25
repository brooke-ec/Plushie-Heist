using cakeslice;
using UnityEngine;

/// <summary>
/// Interaction for Picking items up, implements the IInteractable Interface</br>
/// Object this is on needs a mesh collider and to be on the interactable layer
/// </summary>
public class PickUpInteraction : MonoBehaviour
{
    /// <summary>Prompt Shown by the UI to let the player know they can interact with it</summary>
    public virtual string interactionPrompt => hasSpace ? "Press F to Pick Up" : "Inventory Full";
    /// <summary> Whether this item can be picked up or not </summary>
    public virtual bool canPickup => hasSpace;
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

    protected virtual void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        outline = GetComponentInChildren<Outline>();

        inventoryController.onChanged.AddListener(() =>
        {
            hasSpace = inventoryController.CanInsert(Item);
        });
    }

    protected void Update()
    {
        outline.color = canPickup ? 0 : 1;
    }

    /// <summary>
    /// Called when interacted with </br>
    /// Adds the item to inventory if theres enough space and then destroys it</br>
    /// otherwise it doesnt
    /// </summary>
    /// <param name="interactor">Interactor this was called from</param>
    /// <returns>True if picked up item<returns>
    public virtual bool Interact(Interactor interactor)
    {
        if (!canPickup) return false;

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
