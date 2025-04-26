using UnityEngine;

public interface IInteractable 
{
    /// <summary>Prompt Shown by the interactor's UI element</summary>
    public string interactionPrompt { get; }
    /// <summary>
    /// Called when wanting to interact button pressed when in range of interacting
    /// </summary>
    /// <param name="interactor">Interactor object that called the interaction</param>
    /// <returns>True if intraction succesful</returns>
    public bool Interact(Interactor interactor);
}
