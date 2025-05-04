using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    #region Private Fields
    /// <summary>The RigidBody of the Boss</summary>
    private Rigidbody _rb;

    private Vector3 _flyDirection;
    #endregion
    
    #region Serialized Fields
    [Header("Boss State Flags")]
    /// <summary>An Enum that references the bosses current state</summary>
    [SerializeField] private BossBehaviour _bossBehaviour;

    /// <summary>An Enum that references the bosses current stage</summary>
    [SerializeField] private BossStage _bossStage;

    [Header("Base Stats")]
    [SerializeField] private float _moveSpeed;
    /// <summary>The Max Speed that the boss will move at</summary>
    [SerializeField] private float _maxSpeed;
    

    [Header("Circle")]
    /// <summary>The distance at which the boss should circle the player</summary>
    [SerializeField] private float _circleCloseDistance;

    /// <summary>The Size of the Band that the boss will circle around the player within</summary>
    [SerializeField] private float _circleBandWidth;

    /// <summary>The Speed at which the Boss circles the player at</summary>
    [SerializeField] private float _circleSpeed;

    /// <summary>The Max Speed at which the Boss circles the player at</summary>
    [SerializeField] private float _circleMaxSpeed;


    [Header("Idle")]
    [SerializeField] private float intensity;
    [SerializeField] private float frequency;

    [Header("Ramming")]

    [SerializeField] private float _ramSpeed;

    /// <summary>The delay before the boss rams and it deciding to ram</summary>
    [SerializeField] private float _ramDelay;

    [Header("Other Serialized Fields")]
    /// <summary>A reference to the Player</summary>
    [SerializeField] private GameObject _player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _flyDirection =RandomVector3(Vector3.zero, false);
    }

    // Update is called once per frame
    void FixedUpdate()
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
    /// The stay between a band of values around the player and circles them
    /// </summary>
    private void Circle()
    {
        if(GetPlayerDirection().magnitude >= (_circleCloseDistance + _circleBandWidth))
        {
            _rb.AddForce(GetPlayerDirection());
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
        }
        else if(GetPlayerDirection().magnitude <= _circleCloseDistance)
        {
            _rb.AddForce(-GetPlayerDirection());
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
        }
        else
        {

            Vector3 Player3dVec = GetPlayerDirection();
            Vector2 Player2dVec = new Vector2(Player3dVec.x, Player3dVec.y);
            //float perpenX = Random.Range(-1, 1);
            float perpenX = 1;
            float perpenY = (-(Player2dVec.x * perpenX))/Player2dVec.y;
            Vector3 Perpen3D = new Vector3(perpenX, 0f, perpenY);
            Perpen3D.Normalize();
            
            _rb.AddForce(Perpen3D * _circleSpeed);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _circleMaxSpeed);
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
                //_rb.velocity = Vector3.zero;
                Vector3 CollisionPosition = collision.gameObject.transform.position;

                Vector3 CollisionNormal = collision.GetContact(0).normal;
                //Debug.Log(CollisionNormal);

                _flyDirection = RandomVector3(CollisionPosition, false);
                //_bossBehaviour = BossBehaviour.Idle;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Get the boss to fly around the room
    /// </summary>
    private void Fly()
    {
        _rb.AddForce(_flyDirection * _moveSpeed);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
    }

    /// <summary>
    /// Returns a random vector where all values are between -1 and 1 inclusive
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomVector3(Vector3 CollisionPostion, bool UseCollisionPosition)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        if(!UseCollisionPosition)
        {
            x = Random.Range(-1f, 1f);
            y = Random.Range(-1f, 1f);
            z = Random.Range(-1f, 1f);
        }
        else
        {
            
        }

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

enum BossStage
{
    Big,
    Medium,
    Small
}