using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    private GameObject _Cust_POI1;

    private GameObject _Cust_Spawn1;

    private GameObject _Cust_Death1;

    private int _numCustomers;
    
    private float _respawnTimer;

    [SerializeField] private int _maxCustomers;
    [SerializeField] private float _minSpawnTime;
    [SerializeField] private float _maxSpawnTime;
    

    [SerializeField] private GameObject _customerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _Cust_POI1 = GameObject.Find("Customer Controller/Cust POI 1");
        _Cust_Spawn1 = GameObject.Find("Customer Controller/Cust Spawn 1");
        _Cust_Death1 = GameObject.Find("Customer Controller/Cust Death 1");
        _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(_numCustomers < _maxCustomers && _respawnTimer <= 0)
        {
            GameObject customer = Instantiate(_customerPrefab, _Cust_Spawn1.transform.position, this.transform.rotation);
            _numCustomers++;

            _respawnTimer = Random.Range(_minSpawnTime, _maxSpawnTime);
        }
        


        _respawnTimer -= Time.deltaTime;
    }

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
        return _Cust_Death1.transform.position;
    }
}
