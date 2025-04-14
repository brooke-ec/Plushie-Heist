using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private int numColliders;
    [SerializeField] private TextMeshProUGUI interactionText;

    private readonly Collider[] colliders = new Collider[1];
    private bool textDisplayed = false;

    private void Update()
    {
        numColliders = Physics.OverlapSphereNonAlloc(interactorPoint.position, interactorRadius, colliders, interactorLayerMask);

        if (numColliders > 0 && !textDisplayed)
        {
            if (colliders[0] != null) 
            {
                interactionText.text = colliders[0].GetComponent<IInteractable>() != null ? colliders[0].GetComponent<IInteractable>().interactionPrompt : "Not interactable";
            }
            interactionText.gameObject.SetActive(true);
            textDisplayed = true;
        }
        else if(numColliders ==0 && textDisplayed) 
        {
            interactionText.gameObject.SetActive(false);
            textDisplayed = false;
        }
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
