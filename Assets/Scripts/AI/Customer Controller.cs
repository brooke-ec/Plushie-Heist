using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    private GameObject _Cust_POI1;

    private GameObject _Cust_Spawn1;
    private GameObject _Cust_Spawn2;

    private GameObject _Cust_Death1;
    private GameObject _Cust_Death2;

    private int _numCustomers;
    
    private float _respawnTimer;

    [SerializeField]private List<GameObject> _shopContents;

    [SerializeField] private int _maxCustomers;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;
    

    [SerializeField] private GameObject _customerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _Cust_POI1 = GameObject.Find("Customer Controller/Cust POI 1");
        _Cust_Spawn1 = GameObject.Find("Customer Controller/Cust Spawn 1");
        _Cust_Spawn2 = GameObject.Find("Customer Controller/Cust Spawn 2");
        _Cust_Death1 = GameObject.Find("Customer Controller/Cust Death 1");
        _Cust_Death2 = GameObject.Find("Customer Controller/Cust Death 2");
        _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
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
            customer.GetComponent<CustomerAI>().SetShoppingList(ShoppingList());
            _numCustomers++;

            _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
        }
        


        _respawnTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Randomises the Shopping List Based off of the contents of the shop
    /// If None of the objects are chosen randomly then the list defaults to giving the first item in the contents list
    /// </summary>
    /// <returns>A list that is the Shopping List of the customer</returns>
    private List<GameObject> ShoppingList()
    {
        List<GameObject> ShoppingList = new List<GameObject>();
        foreach(GameObject shopContent in _shopContents)
        {
            if(Random.Range(0,4) == 1)
            {
                ShoppingList.Add(shopContent);
            }
        }
        if(ShoppingList.Count == 0)
        {
            ShoppingList.Add(_shopContents[Random.Range(0, _shopContents.Count + 1)]);
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
    #endregion
}
