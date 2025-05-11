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
    public List<FurnitureItem> basket = new List<FurnitureItem>();

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
                        PickUp();
                        break;
                    case State.Leaving:
                        Destroy(gameObject);
                        break;
                }
            }
        }
        else navAgent.updateRotation = true;
    }

    private void PickUp()
    {
        if (currentItem != null && currentItem.selling)
        {
            float marketPrice = ShopManager.instance.stocksController.GetMarketPriceOfItem(currentItem.item);
            float price = ShopManager.instance.stocksController.GetSellingPriceOfItem(currentItem.item);

            Vector2 acceptablePurchaseRange = ShopManager.instance.stocksController.purchaseRange;
            float currentPriceRatio = price / marketPrice; //So 1 is full market price, 0.5 half price for example
            float chance;

            //prices below purchaseRange.x are guaranteed to sell
            //prices between purchaseRange.x and purchaseRange.y high probability of sale
            //above purchaseRange.y very unlikely to sell

            if (currentPriceRatio <= acceptablePurchaseRange.x)
            {
                //Price is a bargain so always buy
                chance = 1f;
            }
            else if (currentPriceRatio <= acceptablePurchaseRange.y)
            {
                //Within sweet spot so very high probability
                chance = Mathf.Lerp(1f, 0.9f, (currentPriceRatio - acceptablePurchaseRange.x) / (acceptablePurchaseRange.y - acceptablePurchaseRange.x));
            }
            else
            {
                //Above preferred range so sharply decreasing chance
                //The further above maxRange, the worse the chance
                float overRatio = (currentPriceRatio - acceptablePurchaseRange.y) / ((acceptablePurchaseRange.y*2f) - acceptablePurchaseRange.y); // Normalise up to twice the upper acceptable purchase range price
                chance = Mathf.Clamp01(Mathf.Lerp(0.2f, 0.005f, Mathf.Pow(overRatio, 2f))); //Quadratic falloff
            }

            if(Random.value < chance)
            {
                //buy
                animator.SetTrigger("pickup");
                this.RunAfter(pickupTime, PickedUp);
            } else
            {
                animator.SetTrigger("shake");
                this.RunAfter(pickupTime, NextItem);
            }
        } else NextItem();
    }

    /// <summary>
    /// Handles the customer Searching the Shelf by giving it a timer to be searching for.
    /// Then it assigns the next destination for the player to move to
    /// </summary>
    private void PickedUp()
    {
        if (currentItem != null)
        {
            basket.Add(currentItem.item);

            if (
                !ShopManager.instance.autoRestocking ||
                !InventoryController.instance.RemoveAnItemTypeFromInventory(currentItem.item, false)
            ) currentItem.Remove();
        }

        NextItem();
    }

    private void NextItem()
    {
        shoppingList.Dequeue();
        NextAction();
    }

    /// <summary>
    /// Start the next task for this customer
    /// </summary>
    private void NextAction()
    {
        if (shoppingList.Count > 0) PathItem();
        else if (basket.Count > 0) StartQueuing();
        else if (shoppingList.Count == 0 && state == State.Uninitialized) CheckShop(5);
        else LeaveShop();
    }

    /// <summary>
    /// Tells the <see cref="navAgent"/> to path to the next item on the shopping list
    /// </summary>
    private void PathItem()
    {
        if (currentItem != null && NavMesh.SamplePosition( // Get closest position to item
            currentItem.transform.position,
            out NavMeshHit hit, float.PositiveInfinity, NavMesh.AllAreas)
        ) {
            SetDestination(hit.position);
            state = State.Shopping;
        } else {
            shoppingList.Dequeue();
            PathItem();
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

    public void CheckShop(float radius)
    {
        if (Random.Range(0, 3) == 0) LeaveShop();
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += till.transform.position;
            Vector3 finalPosition = till.transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
                finalPosition = hit.position;

            SetDestination(finalPosition);
        }
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