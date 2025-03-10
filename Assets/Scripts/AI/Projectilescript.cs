using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectilescript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float life;

    private float lifeTimer;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTimer += Time.deltaTime;
        if (lifeTimer > life)
        {
            Destroy(this.gameObject);
        }
    }

    
}
