using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private GameObject shopTill;

    //[SerializeField] private GameObject travelPoint1;
    [SerializeField] private bool Shopped = false;

    private bool leavingShop = false;

    private GameObject currentDestination;
    private bool finishedShopping = false;
    // Start is called before the first frame update
    void Start()
    {
        shopTill = GameObject.Find("Till");
        navAgent = GetComponent<NavMeshAgent>();
        //UpdateDestination(travelPoint1.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(navAgent.remainingDistance == 0 && !finishedShopping)
        {
            shopTill.SendMessage("AddToQueue", this.gameObject);
            finishedShopping = true;
        }
        if(Shopped && !leavingShop)
        {
            shopTill.SendMessage("TillActivation");
            leavingShop = true;
        }
    }

    private void UpdateDestination(Vector3 NewDestin)
    {
        navAgent.destination = NewDestin;
    }

    public void LeftQueue()
    {
        //navAgent.destination = travelPoint1.transform.position;
        GameObject.Find("Customer Controller").SendMessage("CustomerLeft");
        GameObject.Destroy(this.gameObject);
    }
}

//Do we want it to choose its shopping list off of the contents of the shop at the start of the day
//or do we want the shopping list to be chosen from all potential things that can be put in the shop
//meaning they could come in and find that there is nothing for them in the shop so they leave
//
//Do the customers want multiple items or just one.