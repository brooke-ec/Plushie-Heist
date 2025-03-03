using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent navAgent;

    [SerializeField] private GameObject travelPoint1;
    [SerializeField] private GameObject travelPoint2;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.SetDestination(travelPoint1.transform.position);
        Debug.Log(navAgent.destination);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ReachedPoint()
    {
        Debug.Log("The Customer has reached its destination");
        Debug.Log(navAgent.destination);
        Debug.Log(travelPoint1.transform.position == navAgent.destination);
        if(navAgent.destination == travelPoint1.transform.position)
        {
            navAgent.SetDestination(travelPoint2.transform.position);
        }
        else
        {
            navAgent.SetDestination(travelPoint1.transform.position);
        }
    }
}
