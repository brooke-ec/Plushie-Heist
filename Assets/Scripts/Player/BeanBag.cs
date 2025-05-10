using UnityEngine;

public class BeanBag : MonoBehaviour
{
    private Rigidbody _rb;
    private AudioSource _audio;


    private void OnCollisionEnter(Collision collision)
    {
        _audio.Play();

        if (collision.gameObject.tag == "Boss")
        {
            collision.gameObject.GetComponent<Boss>().HitByBean();
            Destroy(gameObject);
        } else Destroy(gameObject, 1f);
    }

    public void Throw(Vector3 direction)
    {
        _rb = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();

        _rb.AddForce(direction);

        _rb.AddTorque(new Vector3(Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1)));
    }
}
