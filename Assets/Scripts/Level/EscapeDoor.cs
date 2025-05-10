using UnityEngine;

public class EscapeDoor : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string interactionPrompt { get; private set; }


    public void PrimaryInteract(Interactor interactor)
    {
        NightManager.instance.OnEndNight(true);
    }
}
