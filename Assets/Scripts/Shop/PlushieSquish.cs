using cakeslice;
using Unity.VisualScripting;
using UnityEngine;

public class PlushieSquish: PlushieEnable, IInteractable
{
    string IInteractable.interactionPrompt => "Press F to Squish " + plushie.name;

    private void Start()
    {
        GetComponentsInChildren<Renderer>().ForEach(r => r.AddComponent<Outline>().enabled = false);
        GetComponentsInChildren<Collider>().ForEach(c => c.gameObject.layer = LayerMask.NameToLayer("Interactable"));
    }

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.squeak);
    }
}
