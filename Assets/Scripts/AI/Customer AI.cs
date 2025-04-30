using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    #region Private fields
    /// <summary>The NavMesh Agent</summary>
    private NavMeshAgent _navAgent;

    /// <summary>A reference to the Till GameObject</summary>
    private GameObject _shopTill;

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

    /// <summary>The animator Component of the model</summary>
    private Animator _anim;
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
        _shopTill = GameObject.Find("Till");
        _navAgent = GetComponent<NavMeshAgent>();
        _custController = GameObject.Find("Customer Controller").GetComponent<CustomerController>();

        //Assinging other values using the references
        _deathPosition = _custController.GetDeathPoint();
        _tillQueue = _shopTill.GetComponent<TillQueue>();
        _anim = this.GetComponentInChildren<Animator>();

        //Assinging other values that at affected by other objects
        _navAgent.avoidancePriority = Random.Range(0, 50);
        _maxSearchTime = _searchTime;

        //Gives the customer the first point to go to
        _shoppingList = _custController.ShoppingList();
        _shoppingListBought = _shoppingList;
        UpdateDestination(ItemPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if(_navAgent.remainingDistance <= 2 && _distanceBuffer <= 0)
        {
            _navAgent.isStopped = true;
            _anim.SetBool("Walking", false);
            if(_shoppingListBought.Count != 0)
            {
                SearchingShelf();
            }
        }
        if(_navAgent.isStopped && _navAgent.remainingDistance > 1)
        {
            _navAgent.isStopped = false;
            _anim.SetBool("Walking", true);
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
    }

    /// <summary>
    /// Puts the Customer into the Queue for the Till
    /// </summary>
    private void AddToTill()
    {
        _shopTill.GetComponent<TillQueue>().AddToQueue(this.gameObject);
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
            _anim.SetBool("Walking", true);
            _searchTime = _maxSearchTime;
            _shoppingListBought.RemoveAt(0);

            _anim.SetBool("Searching", true);
            _anim.SetBool("Searching", false);
            
            if(_shoppingListBought.Count != 0)
            {
                UpdateDestination(ItemPosition());
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
    /// returns a vector3 position that is adjusted for the items of the shop
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 ItemPosition()
    {
        Vector3 vectorToReturn = _shoppingListBought[0].transform.position + (_shoppingListBought[0].transform.forward * 2);

        return vectorToReturn;
    }

    /// <summary>
    /// The Customer has been served and will leave the shop
    /// </summary>
    public void LeftTill()
    {
        _anim.SetBool("Paying", true);
        _custController.CustomerLeft();
        //_shopTill.GetComponent<TillQueue>().TillActivation();
        _navAgent.destination = _deathPosition;
        _readyToDie = true;
        _anim.SetBool("Paying", false);
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
        GameObject.Destroy(this.gameObject);
    }
    #endregion
}