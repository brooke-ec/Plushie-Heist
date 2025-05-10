using cakeslice;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlushiePickup : MonoBehaviour, IInteractable
{
    string IInteractable.interactionPrompt => "Press F to Pickup " + SharedUIManager.instance.nextPlushie.name;

    private void Start()
    {
        GetComponentsInChildren<Renderer>().ForEach(r => r.AddComponent<Outline>().enabled = false);
        GetComponentsInChildren<Collider>().ForEach(c => c.gameObject.layer = LayerMask.NameToLayer("Interactable"));

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, float.MaxValue, NavMesh.AllAreas))
        {
            transform.DORotate(Vector3.zero, 0.5f);
            transform.DOMove(hit.position, 0.5f);
        }
    }

    public void PrimaryInteract(Interactor interactor) 
    {
        AudioManager.instance.PlaySound(AudioManager.SoundEnum.squeak);
        NightManager.instance.OnRescuePlushie();
        Destroy(gameObject);
    }
}
