using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    #region Private fields
    /// <summary>The NavMesh Agent</summary>
    private NavMeshAgent _navAgent;

    /// <summary>A refernece to the customer controller script</summary>
    private CustomerController _custController;

    /// <summary>The Position that the customer goes to be Destroyed at</summary>
    private Vector3 _deathPosition;

    /// <summary>The List that is the Customers Shopping List</summary>
    private List<FurnitureItem> _shoppingList = new List<FurnitureItem>();

    /// <summary>The List that refers to how many items the Customer has picked up</summary>
    private List<FurnitureItem> _shoppingListBought = new List<FurnitureItem>();

    /// <summary>A refernece to the Till object<summary>
    private TillQueue _tillQueue;

    /// <summary>Checks wether the Customer is ready to die</summary>
    private bool _readyToDie;

    /// <summary>A  timer to allow the customer to move away from there current position.</summary>
    private float _timeBeforeDeath = 1;

    /// <summary>The time that the customer is Searching for</summary>
    private float _maxSearchTime;

    /// <summary>A timer to allow the customer to move away from there current position. Handled Seperately as this one doesm't need to be reset.</summary>
    private float _distanceBuffer = 1;
    #endregion

    #region Serialized fields    
    /// <summary>A float for the Time the Customer Will spend at a shelf</summary>
    [SerializeField] private float _searchTime;
    #endregion
    #region Private Methods
    // Start is called before the first frame update
    void Start()
    {
        //Assigning Refernces To Objects
        _tillQueue = FindAnyObjectByType<TillQueue>();
        _navAgent = GetComponent<NavMeshAgent>();
        _custController = FindAnyObjectByType<CustomerController>();

        //Assinging other values using the references
        _deathPosition = _custController.GetDeathPoint();

        //Assinging other values that at affected by other objects
        _navAgent.avoidancePriority = Random.Range(0, 50);
        _maxSearchTime = _searchTime;

        //Gives the customer the first point to go to
        _shoppingList = _custController.ShoppingList();
        if (_shoppingList.Count == 0) Kill();

        Debug.Log(_shoppingList.Count);
        Debug.Log("SpaceShip");
        _shoppingListBought = _shoppingList;
        //Debug.Log(_shoppingList[0]);
        //Debug.Log(_shoppingList[0].transform.position);
        //Debug.Log(_shoppingList[0].gameObject.transform.position);
        UpdateDestination(_shoppingList[0].transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(_navAgent.remainingDistance <= 2 && _distanceBuffer <= 0)
        {
            if(_shoppingListBought.Count != 0)
            {
                _navAgent.isStopped = true;
                SearchingShelf();
            }
        }
        if(_navAgent.isStopped && _navAgent.remainingDistance > 1)
        {
            _navAgent.isStopped = false;
        }
        
        if(_distanceBuffer > 0)
        {
            _distanceBuffer -= Time.deltaTime;
        }
        
        //Handles Killing of Customers once they have finished everything
        if(_readyToDie)
        {
            if(_navAgent.remainingDistance <= 2 && _timeBeforeDeath <= 0)
            {
                Kill();
            }
            _timeBeforeDeath -= Time.deltaTime;
        }

        // Rotate to face the till
        if (_shoppingListBought.Count == 0 && _navAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Vector3 direction = _tillQueue.transform.position - transform.position;
            direction.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 3);
        }
    }

    /// <summary>
    /// Puts the Customer into the Queue for the Till
    /// </summary>
    private void AddToTill()
    {
        _tillQueue.AddToQueue(this.gameObject);
    }

    /// <summary>
    /// Handles the customer Searching the Shelf by giving it a timer to be searching for.
    /// Then it assigns the next destination for the player to move to
    /// </summary>
    private void SearchingShelf()
    {
        if(_searchTime <= 0)
        {
            _navAgent.isStopped = false;
            _searchTime = _maxSearchTime;
            _shoppingListBought.RemoveAt(0);

            if(_shoppingListBought.Count != 0)
            {
                UpdateDestination(_shoppingListBought[0].transform.position);
            }
            else
            {
                AddToTill();
            }

            _distanceBuffer = 1;
            return;
        }
        _searchTime -= Time.deltaTime;
    }
    
    /// <summary>
    /// The Customer has been served and will leave the shop
    /// </summary>
    public void LeftTill()
    {
        //_shopTill.GetComponent<TillQueue>().TillActivation();
        _navAgent.destination = _deathPosition;
        _readyToDie = true;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the current Destination of the Navmesh Agent
    /// </summary>
    /// <param name="NewDestin">The location that the customer need to go to</param>
    public void UpdateDestination(Vector3 NewDestin)
    {
        _navAgent.destination = NewDestin;
    }

    public void Kill()
    {
        Destroy(this.gameObject);
        _custController.CustomerLeft();
    }
    #endregion
}