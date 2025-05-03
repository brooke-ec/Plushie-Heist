using cakeslice;
using System.Linq;
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
    [SerializeField] public LayerMask interactorLayerMask;
    /// <summary>The UI element to show the Interaction Text </summary>
    [SerializeField] private TextMeshProUGUI interactionText;

    /// <summary>number of colliders currently in range</summary>
    private int count;
    /// <summary>array to hold colliders that are currently in range</summary>
    private readonly Collider[] colliders = new Collider[8];
    /// <summary> The closest interactable in range </summary>
    private IInteractable interactable;
    /// <summary> The closest collider in range </summary>
    private new Collider collider;
    /// <summary> The closest collider in range last frame </summary>
    private Collider previous;
    /// <summary> The outline of the interactable collider in range </summary>
    private Outline outline;


    private void Start()
    {
        if (interactionText == null) enabled = false;
    }

    private void Update()
    {
        // Get closest interactable in range
        count = Physics.OverlapSphereNonAlloc(interactorPoint.position, interactorRadius, colliders, interactorLayerMask);
        collider = colliders.Take(count).OrderBy(c => Vector3.Distance(interactorPoint.position, c.transform.position)).FirstOrDefault();

        if (!ReferenceEquals(previous, collider))
        {
            previous = collider;

            // Clean up the previous outline
            if (outline != null) outline.enabled = false;

            // Get new interactable
            if (collider == null)
            {
                interactable = null;
                outline = null;
            }
            else
            {
                interactable = collider.GetComponent<IInteractable>();
                
                // Activate Outline
                outline = collider.GetComponentInChildren<Outline>();
                if (outline != null) outline.enabled = true;
            }
        }

        if (interactable == null) interactionText.text = "";
        else
        {
            interactionText.text = interactable.interactionPrompt;
            if (outline != null) outline.color = interactable.interactable ? 0 : 1;
        }
    }

#if UNITY_EDITOR // For Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (interactorPoint == null) Debug.LogError("Interactor.interactorPoint not set");
        else Gizmos.DrawWireSphere(interactorPoint.position, interactorRadius);
    }
#endif

    /// <summary>
    /// If Interactor is close enough to interact call the interactables interact Method when interact button pressed
    /// </summary>
    public void pressPrimaryInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && interactable != null && interactable.interactable) interactable.PrimaryInteract(this);
    }

    /// <summary>
    /// If Interactor is close enough to interact call the interactables secondary interact method when secondary interact button pressed
    /// </summary>
    public void pressSecondaryInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && interactable != null && interactable.interactable) interactable.SecondaryInteract(this);
    }
}
