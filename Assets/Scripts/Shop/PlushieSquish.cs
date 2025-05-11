using cakeslice;
using Unity.VisualScripting;
using UnityEngine;

public class PlushieSquish: PlushieEnable, IInteractable
{
    string IInteractable.interactionPrompt => "Press E to Squish " + plushie.name;

    protected override void Start()
    {
        GetComponentsInChildren<Renderer>().ForEach(r => r.AddComponent<Outline>().enabled = false);
        GetComponentsInChildren<Collider>().ForEach(c => c.gameObject.layer = LayerMask.NameToLayer("Interactable"));
        base.Start();
    }

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.squeak);
    }
}
