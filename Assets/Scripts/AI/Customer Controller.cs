using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomerController : MonoBehaviour
{
    /// <summary>The First Customer Spawn position</summary>
    private Transform _Cust_Spawn1;
    /// <summary>The Second Customer Spawn position</summary>
    private Transform _Cust_Spawn2;

    /// <summary>The First Customer Death position</summary>
    private Transform _Cust_Death1;
    /// <summary>The Second Customer Death position</summary>
    private Transform _Cust_Death2;

    /// <summary>The Current number of Customers allowed in a shop</summary>
    private int _numCustomers;

    /// <summary>THe Timer before the Next Customer is allowed to spawn in</summary>
    private float _respawnTimer;

    /// <summary>Represents wether the shop is open</summary>
    [SerializeField] private bool _shopOpen;

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
        _Cust_Spawn1 = transform.Find("Cust Spawn 1"); // Only searches children (parent is likely to get accidentally renamed)
        _Cust_Spawn2 = transform.Find("Cust Spawn 2");
        _Cust_Death1 = transform.Find("Cust Death 1");
        _Cust_Death2 = transform.Find("Cust Death 2");
        _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(_shopOpen)
        {
            if(_numCustomers < _maxCustomers && _respawnTimer <= 0)
            {
                Transform customerSpawnPoint;
                if(Random.Range(0,2) == 0)
                {
                    customerSpawnPoint = _Cust_Spawn1;
                }
                else
                {
                    customerSpawnPoint = _Cust_Spawn2;
                }
                GameObject customer = Instantiate(_customerPrefab, transform.position, transform.rotation);
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
        FurnitureItem[] forSale = FindObjectsOfType<FurnitureItem>()
            .Where(i => i.selling).OrderBy(_ => Random.value).ToArray();
        return forSale.Take(Random.Range(1, 3)).ToList();
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
        Debug.Log("Doing Something");
        Debug.Log("Doing Something Else");
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
