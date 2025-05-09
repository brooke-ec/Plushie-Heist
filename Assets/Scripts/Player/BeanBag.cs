using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanBag : MonoBehaviour
{
    private Rigidbody _rb;

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.layer)
        {
            case 6:
                //Env
                Destroy(this.gameObject);
                break;
            case 12:
                //boss
                collision.gameObject.GetComponent<Boss>().HitByBean();
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }

    public void Throw(float throwStrength)
    {
        this.gameObject.SetActive(true);

        _rb = GetComponent<Rigidbody>();

        _rb.AddForce(this.transform.forward * throwStrength);

        _rb.AddTorque(new Vector3(Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1)));
    }
}
