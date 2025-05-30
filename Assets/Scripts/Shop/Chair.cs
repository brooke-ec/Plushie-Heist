using cakeslice;
using UnityEngine;

public class Chair : MonoBehaviour, IInteractable
{
    [SerializeField] Transform anchor;

    string IInteractable.interactionPrompt => "Press E to Sit";

    void IInteractable.PrimaryInteract(Interactor interactor)
    {
       PlayerController.instance.seat = anchor;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (anchor == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(anchor.position + anchor.right * 0.25f, anchor.position + -anchor.right * 0.25f);
        Gizmos.DrawLine(anchor.position + anchor.up * 0.25f, anchor.position + -anchor.up * 0.25f);
        Gizmos.DrawLine(anchor.position + anchor.forward * 0.25f, anchor.position + -anchor.forward* 0.25f);
    }
#endif
}
