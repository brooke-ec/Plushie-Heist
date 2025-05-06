using cakeslice;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public class BrowserOpener : MonoBehaviour, IInteractable
{
    public string interactionPrompt => "Press F for Shop Management";

    private BrowserManager browser;

    private void Awake()
    {
        browser = FindObjectOfType<BrowserManager>(true);
    }

    public void PrimaryInteract(Interactor interactor)
    {
        SharedUIManager.instance.OpenMenu(browser);
    }
}
