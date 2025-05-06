using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerAI : MonoBehaviour
{
    #region Public fields
    /// <summary> True if the customer is not currently walking </summary>
    public bool finishedWalking => navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending;
    #endregion

    #region Private fields
    /// <summary>The NavMesh Agent</summary>
    private NavMeshAgent navAgent;

    /// <summary>A refernece to the customer controller script</summary>
    private CustomerController customerController;

    /// <summary> A shopping list of items the customer wants to buy </summary>
    private Queue<FurnitureController> shoppingList;

    /// <summary> List of items this customer has in their basket </summary>
    private List<FurnitureController> basket = new List<FurnitureController>();

    /// <summary>A refernece to the Till object<summary>
    private TillQueue till;

    /// <summary>The animator Component of the model</summary>
    private Animator animator;

    /// <summary> The current item the customer is looking for </summary>
    private FurnitureController currentItem => shoppingList.TryPeek(out FurnitureController item) ? item : null;

    private State state = State.Uninitialized;
    #endregion

    #region Serialized fields    
    /// <summary>A float for the Time the Customer Will spend at a shelf</summary>
    [SerializeField] private float pickupTime = 2;
    #endregion

    #region Private Methods

    void Start()
    {
        //Assigning References To Objects
        navAgent = GetComponent<NavMeshAgent>();
        till = FindAnyObjectByType<TillQueue>();
        customerController = FindAnyObjectByType<CustomerController>();

        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("pickup multiplier", 0.375f / pickupTime);

        //Assinging other values using the references
        shoppingList = new Queue<FurnitureController>(customerController.ShoppingList());
    }

    void Update()
    {
        animator.SetBool("shopping", state == State.Shopping);
        animator.SetBool("walking", !finishedWalking);

        if (finishedWalking)
        {
            // Rotate customer
            Vector3 target;
            if (currentItem != null) target = currentItem.bounds.center;
            else target = till.transform.position;

            target.y = transform.position.y;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(target - transform.position),
                Time.deltaTime * 3
            );

            // Handle destination
            if (!navAgent.isStopped)
            {
                navAgent.isStopped = true; // Run once after reaching the destination
                switch (state)
                {
                    case State.Uninitialized:
                        NextAction();
                        break;
                    case State.Shopping:
                        this.RunAfter(pickupTime, PickedUp);
                        break;
                    case State.Leaving:
                        Destroy(gameObject);
                        break;
                }
            }
        }
        else navAgent.updateRotation = true;
    }

    /// <summary>
    /// Handles the customer Searching the Shelf by giving it a timer to be searching for.
    /// Then it assigns the next destination for the player to move to
    /// </summary>
    private void PickedUp()
    {
        basket.Add(currentItem);

        shoppingList.Dequeue();
        NextAction();
    }

    /// <summary>
    /// Start the next task for this customer
    /// </summary>
    private void NextAction()
    {
        if (shoppingList.Count > 0) NextItem();
        else if (basket.Count > 0) StartQueuing();
        else LeaveShop();
    }

    /// <summary>
    /// Tells the <see cref="navAgent"/> to path to the next item on the shopping list
    /// </summary>
    private void NextItem()
    {
        if (NavMesh.SamplePosition( // Get closest position to item
            currentItem.transform.position,
            out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas)
        ) {
            SetDestination(hit.position);
            state = State.Shopping;
        } else {
            shoppingList.Dequeue();
            NextItem();
        }
    }

    /// <summary>
    /// Passes control to <see cref="till"/>
    /// </summary>
    public void StartQueuing()
    {
        state = State.Queueing;
        till.AddToQueue(this);
    }

    /// <summary>
    /// The Customer has been served and will leave the shop
    /// </summary>
    public void LeaveShop()
    {
        SetDestination(customerController.PickDeathPoint());
        state = State.Leaving;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the current Destination of the Navmesh Agent
    /// </summary>
    /// <param name="position">The location that the customer need to go to</param>
    public void SetDestination(Vector3 position)
    {
        navAgent.ResetPath();
        navAgent.SetDestination(position);
    }

    public void OnDestroy()
    {
        customerController.CustomerLeft();
    }
    #endregion

    private enum State
    {
        Uninitialized,
        Shopping,
        Queueing,
        Leaving
    }
}