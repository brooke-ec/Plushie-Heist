using cakeslice;
using Unity.VisualScripting;
using UnityEngine;

public class PlushieEnable : MonoBehaviour, IInteractable
{
    [SerializeField] PlushieInfo plushie;
    [SerializeField] bool inverted = false;

    string IInteractable.interactionPrompt => "Press F to Squish " + plushie.name;

    void Start()
    {
        GetComponentsInChildren<Renderer>().ForEach(r => r.AddComponent<Outline>().enabled = false);
        GetComponentsInChildren<Collider>().ForEach(c => c.gameObject.layer = LayerMask.NameToLayer("Interactable"));
        gameObject.SetActive(plushie.unlocked == !inverted);
    }

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.squeak);
    }
}
