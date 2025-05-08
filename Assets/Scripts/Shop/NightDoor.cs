using cakeslice;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class NightDoor : MonoBehaviour, IInteractable
{
    string IInteractable.interactionPrompt => "Press F to collect supplies";

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
        SceneManager.LoadScene(0);
    }
}
