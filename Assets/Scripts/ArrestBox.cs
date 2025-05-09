using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrestBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().arrested = true;
        }
    }
}
