using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Projectilescript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float life;
    [field:SerializeField] public float SlowAmount { get; private set; }


    private float lifeTimer;

    private RaycastHit hit;
    private Vector3 originalPos;

    private void Awake()
    {
        LayerMask mask = LayerMask.GetMask("Env");
        originalPos = transform.position;
        Physics.Raycast(transform.position,transform.forward,out hit,speed*life,mask);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTimer += Time.deltaTime;
        if (Vector3.Distance(originalPos,transform.position)>hit.distance)
        {
            Destroy(this.gameObject);
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.gameObject.GetHashCode()+"hit"+other.name);
        if (!other.CompareTag("Player") && !other.CompareTag("GunGuard"))
        {
            Debug.Log("Hit");
            Destroy(gameObject);
        }
    }*/

}
