using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePads : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _strength;

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
        Debug.Log("Entered Trig");
        if(other.CompareTag("Player"))
        {
            //Debug.Log("Calling function on Player");
            PlayerController.instance.HitHazard("Bounce Pad", this.gameObject);
        }
    }

    public Vector3 getDirection()
    {
        return _direction;
    }

    public float getStrength()
    {
        return _strength;
    }
}
