using UnityEngine;

public class EscapeDoor : MonoBehaviour, IInteractable
{
    public string interactionPrompt => "Press E to Escape";


    public void PrimaryInteract(Interactor interactor)
    {
        NightManager.instance.OnEndNight(true);
    }
}
