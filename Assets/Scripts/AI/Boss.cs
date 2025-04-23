using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    #region the Boss state flags
    [Header("Boss State Flags")]
    /// <summary>An Enum that references the bosses current state</summary>
    [SerializeField] private BossBehaviour _bossBehaviour;
    #endregion
    
    #region Private Fields
    /// <summary>The RigidBody of the Boss</summary>
    private Rigidbody _rb;

    private Vector3 _flyDirection;
    #endregion
    
    #region Other Serialized Fields
    [Header("Other Serialized Fields")]
    /// <summary>A reference to the Player</summary>
    [SerializeField] private GameObject _player;
    

    [Header("Base Stats")]
    [SerializeField] private float _moveSpeed;
    /// <summary>The Max Speed that the boss will move at</summary>
    [SerializeField] private float _maxSpeed;
    

    [Header("Circle")]
    /// <summary>The distance at which the boss should circle the player</summary>
    [SerializeField] private float _circleDistance;


    [Header("Idle")]
    [SerializeField] private float intensity;
    [SerializeField] private float frequency;

    [Header("Ramming")]
    [SerializeField] private float _ramSpeed;
    [SerializeField] private float _ramDelay;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _flyDirection =RandomVector3();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_bossBehaviour)
        {
            case BossBehaviour.Idle:
                Idle();
                break;
            case BossBehaviour.Flying:
                Fly();
                break;
            case BossBehaviour.Circling:
                Circle();
                break;
            case BossBehaviour.Ramming:
                StartCoroutine(Ram());
                _bossBehaviour = BossBehaviour.Flying;
                break;
            default:
                break;
        }



    }

    /// <summary>
    /// Returns the Distance to the player
    /// </summary>
    /// <returns>a float value that is the remaining distnace to the player</returns>
    private Vector3 GetPlayerDirection()
    {
        return _player.transform.position - this.transform.position;
    }

    /// <summary>
    /// Animates the boss to be in the Idle state
    /// </summary>
    private void Idle()
    {
        //_rb.AddForce(Vector3.down * 5);
        //this.transform.position = Vector3.up * Mathf.Cos(Time.time * frequency) * intensity;
        //Vector3 floatfloat = Vector3.up * Mathf.Cos(Time.time * frequency) * intensity;
        //_rb.AddForce(floatfloat);

        
    }

    /// <summary>
    /// 
    /// </summary>
    private void Circle()
    {
        if(GetPlayerDirection().magnitude >= _circleDistance)
        {
            _rb.AddForce(GetPlayerDirection());
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
        }
        else if(GetPlayerDirection().magnitude <= _circleDistance)
        {
            _rb.AddForce(-GetPlayerDirection());
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
        }
        else
        {
            _rb.velocity = Vector3.zero;

        }
    }

    /// <summary>
    /// The Coroutine that is the bosses Ram
    /// </summary>
    IEnumerator Ram()
    {
        yield return new WaitForSeconds(_ramDelay);
        _rb.AddForce(GetPlayerDirection() * _ramSpeed);
    }

    /// <summary>
    /// Detects and handles all collisions that the boss has
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.layer)
        {
            case 6:
                _rb.velocity = Vector3.zero;
                _flyDirection = RandomVector3();
                //_bossBehaviour = BossBehaviour.Idle;
                break;
            default:
                break;
        }
    }

    private void Fly()
    {
        _rb.AddForce(_flyDirection * _moveSpeed);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
    }

    /// <summary>
    /// Returns a random vector where all values are between -1 and 1 inclusive
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomVector3()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        Vector3 returnVector = new Vector3(x,y,z);
        returnVector.Normalize();
        return returnVector;
    }
}

enum BossBehaviour
{
    Idle,
    Flying,
    Circling,
    Ramming
}