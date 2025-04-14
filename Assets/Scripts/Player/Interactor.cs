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
    /// <summary>Layer to interact with should be Interactable add others if needed(shouldnt be) </summary>
    [SerializeField] private LayerMask interactorLayerMask;
    /// <summary>The UI element to show the Interaction Text </summary>
    [SerializeField] private TextMeshProUGUI interactionText;

    /// <summary>number of colliders currently interactable with</summary>
    private int numColliders;
    /// <summary>array to hold colliders that are currently in range</summary>
    private readonly Collider[] colliders = new Collider[1];
    /// <summary>is Ui Text displayed </summary>
    private bool textDisplayed = false;

    private void Update()
    {
        //get interactables in range
        numColliders = Physics.OverlapSphereNonAlloc(interactorPoint.position, interactorRadius, colliders, interactorLayerMask);

        if (numColliders > 0 && !textDisplayed) // show relevant text 
        {
            if (colliders[0] != null) 
            {
                interactionText.text = colliders[0].GetComponent<IInteractable>() != null ? colliders[0].GetComponent<IInteractable>().interactionPrompt : "Not interactable";
            }
            interactionText.gameObject.SetActive(true);
            textDisplayed = true;
        }
        else if(numColliders ==0 && textDisplayed) // hide text
        {
            interactionText.gameObject.SetActive(false);
            textDisplayed = false;
        }
    }

    // For Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactorPoint.position, interactorRadius);
    }
    /// <summary>
    /// If Interactor is close enough to interact call the interactables interact Method when interact button pressed
    /// </summary>
    /// <param name="ctx"></param>
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
