using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomerController : MonoBehaviour
{
    /// <summary>The customer spawn positions</summary>
    [SerializeField] private Transform[] customerSpawns = new Transform[0];

    /// <summary>The First Customer Death position</summary>
    [SerializeField] private Transform[] customerDeaths = new Transform[0];

    /// <summary>The current number of Customers the shop</summary>
    private int customerCount;

    /// <summary>THe Timer before the Next Customer is spawned in</summary>
    private float spawnTimer;

    /// <summary>The Max number of Customers allowed to be spawned</summary>
    [SerializeField] private int maxCustomers;
    /// <summary>The Min Spawn Time for the Customers</summary>
    [SerializeField] private float minSpawnTime;
    /// <summary>The Max spawn Time for the Customers</summary>
    [SerializeField] private float maxSpawnTime;
    
    /// <summary>The Prefab that makes the Customer</summary>
    [SerializeField] private GameObject customerPrefab;

    // Update is called once per frame
    void Update()
    {
        if (ShopManager.instance.isShopOpen)
        {
            if(customerCount < maxCustomers && spawnTimer <= 0)
            {
                Vector3 customerSpawnPoint = customerSpawns[Random.Range(0, customerSpawns.Length)].position;
                GameObject customer = Instantiate(customerPrefab, customerSpawnPoint, Quaternion.identity);

                spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
                customerCount++;
            }

            spawnTimer -= Time.deltaTime;
        }
    }

    #region Public Methods
    /// <summary>
    /// Randomises the Shopping List Based off of the contents of the shop
    /// If None of the objects are chosen randomly then the list defaults to giving the first item in the contents list
    /// </summary>
    /// <returns>A list that is the Shopping List of the customer</returns>
    public List<FurnitureController> ShoppingList()
    {
        FurnitureController[] forSale = FindObjectsOfType<FurnitureController>()
            .Where(i => i.selling).OrderBy(_ => Random.value).ToArray();
        return forSale.Take(Random.Range(1, 3)).ToList();
    }

    /// <summary>
    /// Lets the customer Controller know that a customer has left the shop
    /// </summary>
    public void CustomerLeft()
    {
        customerCount--;
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    /// <summary>
    /// returns the Position for the Customer to be Destroyed at
    /// </summary>
    /// <returns>a Vector3 position that the player customer gets destroyed at</returns>
    public Vector3 PickDeathPoint()
    {
        return customerDeaths[Random.Range(0, customerDeaths.Length)].position;
    }

    public void IncreaseCustomerSpawnRate(float addedMultiplier)
    {
        maxCustomers *= Mathf.CeilToInt(1 + addedMultiplier);
        minSpawnTime /= Mathf.CeilToInt(1 + addedMultiplier);
        maxCustomers *= Mathf.CeilToInt(1 + addedMultiplier);
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Transform spawn in customerSpawns)
        {
            Gizmos.DrawIcon(spawn.position, "enter");
        }

        foreach (Transform death in customerDeaths)
        {
            Gizmos.DrawIcon(death.position, "exit");
        }
    }
#endif
}
