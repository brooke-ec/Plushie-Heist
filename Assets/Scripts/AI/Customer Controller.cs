using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomerController : MonoBehaviour
{
    /// <summary>The First Customer Spawn position</summary>
    private GameObject _Cust_Spawn1;
    /// <summary>The Second Customer Spawn position</summary>
    private GameObject _Cust_Spawn2;

    /// <summary>The First Customer Death position</summary>
    private GameObject _Cust_Death1;
    /// <summary>The Second Customer Death position</summary>
    private GameObject _Cust_Death2;

    /// <summary>The Current number of Customers allowed in a shop</summary>
    private int _numCustomers;

    /// <summary>THe Timer before the Next Customer is allowed to spawn in</summary>
    private float _respawnTimer;

    /// <summary>The Contents of the Shop</summary>
    private List<FurnitureItem> _shopContents;

    /// <summary>Represents wether the shop is open</summary>
    private bool _shopOpen;

    /// <summary>The Max number of Customers allowed to be spawned</summary>
    [SerializeField] private int _maxCustomers;
    /// <summary>The Min Spawn Time for the Customers</summary>
    [SerializeField] private float _minSpawnTime;
    /// <summary>The Max spawn Time for the Customers</summary>
    [SerializeField] private float _maxSpawnTime;
    
    /// <summary>The Prefab that makes the Customer</summary>
    [SerializeField] private GameObject _customerPrefab;

    /// <summary>The Grid that the furntiture is placed on</summary>
    [SerializeField] private FurnitureGrid _furnitureGrid;

    // Start is called before the first frame update
    void Start()
    {
        _Cust_Spawn1 = GameObject.Find("Customer Controller/Cust Spawn 1");
        _Cust_Spawn2 = GameObject.Find("Customer Controller/Cust Spawn 2");
        _Cust_Death1 = GameObject.Find("Customer Controller/Cust Death 1");
        _Cust_Death2 = GameObject.Find("Customer Controller/Cust Death 2");
        _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
        _shopContents = new List<FurnitureItem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_shopOpen && _shopContents.Count != 0)
        {
            if(_numCustomers < _maxCustomers && _respawnTimer <= 0)
            {
                GameObject customerSpawnPoint;
                if(Random.Range(0,2) == 0)
                {
                    customerSpawnPoint = _Cust_Spawn1;
                }
                else
                {
                    customerSpawnPoint = _Cust_Spawn2;
                }
                GameObject customer = Instantiate(_customerPrefab, customerSpawnPoint.transform.position, this.transform.rotation);
                //customer.GetComponent<CustomerAI>().SetShoppingList(ShoppingList());
                _numCustomers++;

                _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
            }
            _respawnTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Randomises the Shopping List Based off of the contents of the shop
    /// If None of the objects are chosen randomly then the list defaults to giving the first item in the contents list
    /// </summary>
    /// <returns>A list that is the Shopping List of the customer</returns>
    public List<FurnitureItem> ShoppingList()
    {
        List<FurnitureItem> ShoppingList = new List<FurnitureItem>();
        foreach(FurnitureItem shopContent in _shopContents)
        {
            if(Random.Range(0,4) == 1)
            {
                ShoppingList.Add(shopContent);
            }
        }
        if(ShoppingList.Count == 0)
        {
            if(_shopContents.Count == 1)
            {
                ShoppingList.Add(_shopContents[0]);
            }
            else
            {
                ShoppingList.Add(_shopContents[Random.Range(0, _shopContents.Count + 1)]);
            }
        }
        return ShoppingList;
    }

    #region Public Methods
    /// <summary>
    /// Lets the customer Controller know that a customer has left the shop
    /// </summary>
    public void CustomerLeft()
    {
        _numCustomers--;
        _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    /// <summary>
    /// returns the Position for the Customer to be Destroyed at
    /// </summary>
    /// <returns>a Vector3 position that the player customer gets destroyed at</returns>
    public Vector3 GetDeathPoint()
    {
        if(Random.Range(0,2) == 0)
        {
            return _Cust_Death1.transform.position;
        }
        else
        {
            return _Cust_Death2.transform.position;
        }
    }

    /// <summary>
    /// Opens the Shop allowing Customers to enter
    /// </summary>
    public void OpenShop()
    {
        _shopOpen = true;
        _shopContents = _furnitureGrid.GetContents();
    }

    /// <summary>
    /// Closes the Shop meaning Customers can't enter
    /// </summary>
    public void CloseShop()
    {
        _shopOpen = false;
    }
    #endregion

    [InspectorButton("OpenShop")]
    public bool Open_Shop;

    [InspectorButton("CloseShop")]
    public bool Close_Shop;
}
