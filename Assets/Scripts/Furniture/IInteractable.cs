using UnityEngine;

public interface IInteractable 
{
    /// <summary> Prompt Shown by the interactor's UI element </summary>
    public string interactionPrompt { get; }

    /// <summary>
    /// Called when the primary interact key is pressed on this <see cref="IInteractable"/> instance.
    /// </summary>
    /// <param name="interactor"> Interactor object that called the interaction </param>
    /// <returns> <see cref="true"/> if the intraction was succesful </returns>
    public bool PrimaryInteract(Interactor interactor) { return true; }

    /// <summary>
    /// Called when the secondary interact key is pressed on this <see cref="IInteractable"/> instance.
    /// </summary>
    /// <param name="interactor"> Interactor object that called the interaction </param>
    /// <returns> <see cref="true"/> if the intraction was succesful </returns>
    public bool SecondaryInteract(Interactor interactor) { return true; }
}
