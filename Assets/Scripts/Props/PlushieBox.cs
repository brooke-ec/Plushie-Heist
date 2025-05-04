using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlushieBox : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Pick Up";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool interact(Interactor interactor)
    {
        
        // //InventoryController inventoryController = FindAnyObjectByType<InventoryController>();
        // if (inventoryController.InsertItem(Item))
        // {
        //     Destroy(gameObject);
        //     Debug.Log("Picked Up" + gameObject.name);
        //     return true;
        // }
        // else { 
        //     Debug.Log("Can't Pick up"+gameObject.name);
        //     return false;
        // }
        
        return false;
    }
}
