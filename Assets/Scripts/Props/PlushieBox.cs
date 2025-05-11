using UnityEngine;

public class PlushieBox : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string interactionPrompt { get; private set; } = "Press E to Pick Up";

    public bool outline => true;
    public void PrimaryInteract(Interactor interactor) 
    { 
        PlayerController.instance.PickupBean();
    }
}
