using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plushie : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press F to Free Plushie";
    public bool outline => true;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrimaryInteract(Interactor interactor) 
    { 
        //PlayerController.instance.PickupBean();
    }
}
