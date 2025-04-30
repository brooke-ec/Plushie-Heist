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
    private Queue<FurnitureItem> shoppingList;

    /// <summary> List of items this customer has in their basket </summary>
    private List<FurnitureItem> basket = new List<FurnitureItem>();

    /// <summary>A refernece to the Till object<summary>
    private TillQueue till;

    /// <summary>The animator Component of the model</summary>
    private Animator anim;

    /// <summary> The current item the customer is looking for </summary>
    private FurnitureItem currentItem = null;

    /// <summary> True if the customer has brought everything on their shopping list </summary>
    private bool finishedShopping => shoppingList.Count == 0 && currentItem == null;

    /// <summary> True if the customer is leaving the store </summary>
    private bool leaving = false;
    #endregion

    #region Serialized fields    
    /// <summary>A float for the Time the Customer Will spend at a shelf</summary>
    [SerializeField] private float pickupTime = 2;
    [SerializeField] private Animator[] models;
    [SerializeField] private RuntimeAnimatorController controller;
    #endregion

    #region Private Methods

    void Start()
    {
        //Assigning References To Objects
        navAgent = GetComponent<NavMeshAgent>();
        till = FindAnyObjectByType<TillQueue>();
        customerController = FindAnyObjectByType<CustomerController>();

        // Pick model
        anim = Instantiate(models[Random.Range(0, models.Length)], transform);
        anim.runtimeAnimatorController = controller;

        //Assinging other values using the references
        shoppingList = new Queue<FurnitureItem>(customerController.ShoppingList());
    }

    void Update()
    {
        if (finishedWalking)
        {
            // Rotate customer
            Vector3 target;
            if (currentItem != null) target = currentItem.bounds.center;
            else target = till.transform.position;

            if (target != null)
            {
                target.y = transform.position.y;
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(target - transform.position),
                    Time.deltaTime * 3
                );
            }

            if (!navAgent.isStopped)
            {
                navAgent.isStopped = true; // Run once after reaching the destination

                if (leaving) Destroy(gameObject);
                else if (currentItem != null) this.RunAfter(pickupTime, PickedUp);
                else Next();
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
        currentItem = null;
        Next();
    }

    /// <summary>
    /// Start the next task for this customer
    /// </summary>
    private void Next()
    {
        if (!finishedShopping) NextItem();
        else if (basket.Count > 0) till.AddToQueue(this); // Hand control to the till
        else LeaveShop();
    }

    /// <summary>
    /// Tells the <see cref="navAgent"/> to path to the next item on the shopping list
    /// </summary>
    private void NextItem()
    {
        NavMeshHit hit;

        // Repeat until we find an item to path to
        while (!NavMesh.SamplePosition( // Get closest position to item
            (currentItem = shoppingList.Dequeue()).transform.position,
            out hit, float.PositiveInfinity, NavMesh.AllAreas)
        );

        SetDestination(hit.position);
    }

    /// <summary>
    /// The Customer has been served and will leave the shop
    /// </summary>
    public void LeaveShop()
    {
        SetDestination(customerController.PickDeathPoint());
        leaving = true;
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
}