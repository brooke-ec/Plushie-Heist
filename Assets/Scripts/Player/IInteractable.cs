using UnityEngine;

public interface IInteractable 
{
    /// <summary> Prompt Shown by the interactor's UI element </summary>
    public string interactionPrompt { get; }

    /// <summary>
    /// Denotes whether this object is interactible- used for setting outline colour.
    /// </summary>
    public bool interactable => true;

    /// <summary>
    /// Called when the primary interact key is pressed on this <see cref="IInteractable"/> instance.
    /// </summary>
    /// <param name="interactor"> Interactor object that called the interaction </param>
    public void PrimaryInteract(Interactor interactor) { }

    /// <summary>
    /// Called when the secondary interact key is pressed on this <see cref="IInteractable"/> instance.
    /// </summary>
    /// <param name="interactor"> Interactor object that called the interaction </param>
    public void SecondaryInteract(Interactor interactor) { }
}
