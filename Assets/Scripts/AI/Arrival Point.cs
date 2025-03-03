using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrivalPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered");
        if(other.CompareTag("Customer"))
        {
            other.SendMessage("ReachedPoint");
        }
    }
}
