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
    private List<string> _shoppingList;

    /// <summary>The List that refers to how many items the Customer has picked up</summary>
    private List<string> _shoppingListBought;

    /// <summary>A refernece to the Till object<summary>
    private TillQueue _tillQueue;

    /// <summary>Checks wether the Customer is ready to die</summary>
    private bool _readyToDie;
    #endregion

    #region Serialized fields
    /// <summary>A bool that determines wether the Csutomer has been served at the Till</summary>
    [SerializeField] private bool _hasPayed= false;
    

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _shopTill = GameObject.Find("Till");
        _navAgent = GetComponent<NavMeshAgent>();
        _custController = GameObject.Find("Customer Controller").GetComponent<CustomerController>();
        _deathPosition = _custController.GetDeathPoint();
        _shoppingList = new List<string>();
        _shoppingListBought = _shoppingList;
        _tillQueue = GameObject.Find("Till").GetComponent<TillQueue>();
    }

    // Update is called once per frame
    void Update()
    {
        /***
        if(_navAgent.remainingDistance == 0 && !_finishedFindingItems)
        {
            _shopTill.SendMessage("AddToQueue", this.gameObject);
            _finishedFindingItems = true;
        }
        if(_hasPayed&& !_leavingShop)
        {
            _shopTill.SendMessage("TillActivation");
            _leavingShop = true;
        }
        ***/

        // if the shopping list is empty
        if(_shoppingListBought.Count == 0)
        {
            _shoppingListBought.Add("PlaceHolder");
            AddToTill();
        }
        if(_hasPayed)
        {
            _hasPayed = false;
            LeftQueue();
        }
        if(_readyToDie && _navAgent.remainingDistance == 0)
        {   
            Debug.Log(_navAgent.destination);
            GameObject.Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Updates the current Destination of the Navmesh Agent
    /// </summary>
    /// <param name="NewDestin">The location that the customer need to go to</param>
    public void UpdateDestination(Vector3 NewDestin)
    {
        _navAgent.destination = NewDestin;
    }

    /// <summary>
    /// Puts the Customer into the Queue for the Till
    /// </summary>
    private void AddToTill()
    {
        //_shopTill.SendMessage("AddToQueue", this.gameObject);
        _shopTill.GetComponent<TillQueue>().AddToQueue(this.gameObject);
    }

    /// <summary>
    /// The Customer has been served and will leave the shop
    /// </summary>
    public void LeftQueue()
    {
        _custController.CustomerLeft();
        _shopTill.GetComponent<TillQueue>().TillActivation();
        _navAgent.destination = _deathPosition;
        _hasPayed = false;
        _readyToDie = true;
    }
}

//Do we want it to choose its shopping list off of the contents of the shop at the start of the day
//or do we want the shopping list to be chosen from all potential things that can be put in the shop
//meaning they could come in and find that there is nothing for them in the shop so they leave
//
//Do the customers want multiple items or just one.