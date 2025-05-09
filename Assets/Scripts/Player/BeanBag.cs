using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanBag : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audio;


    private void OnCollisionEnter(Collision collision)
    {
        _audio.Play();
        switch(collision.gameObject.layer)
        {
            case 6:
                //Env
                Destroy(this.gameObject, 1f);
                break;
            case 12:
                //boss
                collision.gameObject.GetComponent<Boss>().HitByBean();
                Destroy(this.gameObject, 0.2f);
                break;
            default:
                break;
        }
    }

    public void Throw(float throwStrength)
    {
        _rb = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();

        _rb.AddForce(this.transform.forward * throwStrength);

        _rb.AddTorque(new Vector3(Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1)));
    }
}
