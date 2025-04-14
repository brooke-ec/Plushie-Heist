using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    /// <summary>
    ///The place the interactor is placed on the Player</br>
    /// Should be a empty child of the player
    /// </summary>
    [SerializeField] private Transform interactorPoint;
    /// <summary>The radius of the interactor from the point</summary>
    [SerializeField] private float interactorRadius;
    [SerializeField] private LayerMask interactorLayerMask;

    private readonly Collider[] colliders = new Collider[1];
    [SerializeField] private int numColliders;

    private void Update()
    {
        numColliders = Physics.OverlapSphereNonAlloc(interactorPoint.position, interactorRadius, colliders, interactorLayerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactorPoint.position, interactorRadius);
    }

    public void pressInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (numColliders > 0)
            {
                IInteractable interactable = colliders[0].GetComponent<IInteractable>();
                if (interactable != null) { interactable.interact(this); }
            }
        }
    }
}
