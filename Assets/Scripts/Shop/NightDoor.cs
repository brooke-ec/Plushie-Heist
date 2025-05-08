using cakeslice;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class NightDoor : MonoBehaviour, IInteractable
{
    string IInteractable.interactionPrompt => ShopManager.instance.isShopOpen ? "Close the Shop First" : "Press F to Collect Supplies";
    bool IInteractable.outline => !ShopManager.instance.isShopOpen;

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
        if (ShopManager.instance.isShopOpen) return;

        ShopManager.instance.StartNewDay();
        SaveManager.instance.Save();
        LoadingSceneController.instance.LoadSceneAsync(0);
    }
}
