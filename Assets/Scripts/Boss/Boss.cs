using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Boss : MonoBehaviour
{
    #region Private Fields
    /// <summary>The RigidBody of the Boss</summary>
    private Rigidbody _rb;

    /// <summary>The Direction that the Boss is flying in</summary>
    private Vector3 _flyDirection;

    private float _flySpeed;

    private float _maxFlySpeed;

    /// <summary>The Max Speed that the boss will move at</summary>
    private float _maxSpeed;

    private float _ramtimer = 0;

    private BossBehaviour previousBossBehaviour;
    #endregion
    
    #region Serialized Fields
    [Header("Boss State Flags")]

    /// <summary>An Enum that references the bosses current state</summary>
    [SerializeField] private BossBehaviour _bossBehaviour;

    /// <summary>An Enum that references the bosses current stage</summary>
    [SerializeField] private int _bossHp;

    [SerializeField] private Material[] _shatterMaterials;
    [SerializeField] private MeshRenderer _shatterOverlay;
    [SerializeField] private GameObject[] _deathPersistent;
    [SerializeField] private AudioClip[] _hitSounds;
    private AudioSource _audioSource;

    #region Boss Stage Base Stats
    [Header("Big Stage Stats")]

    /// <summary>The Scale of the Boss in the big stage</summary>
    [SerializeField] private float _bigScale;
    /// <summary>The Fly speed of the Boss in the Big Stage</summary>
    [SerializeField] private float _bigFlySpeed;
    /// <summary>The Max fly speed of the Boss in the Big Stage</summary>
    [SerializeField] private float _bigMaxFly;

    //[Header("Medium Stage Stats")]

    /// <summary>The Scale of the Boss in the medium stage</summary>
    //[SerializeField] private float _medScale;
    /// <summary>The Fly speed of the Boss in the medium Stage</summary>
    //[SerializeField] private float _medFlySpeed;
    /// <summary>The Max fly speed of the Boss in the medium Stage</summary>
    //[SerializeField] private float _medMaxFly;

    //[Header("Small Stage Stats")]

    /// <summary>The Scale of the Boss in the small stage</summary>
    //[SerializeField] private float _smallScale;
    /// <summary>The Fly speed of the Boss in the small Stage</summary>
    //[SerializeField] private float _smallFlySpeed;
    /// <summary>The Max fly speed of the Boss in the small Stage</summary>
    //[SerializeField] private float _smaMaxFly;

    #endregion

    [Header("Base Stats")]

    [SerializeField] private float minRamTime;
    [SerializeField] private float maxRamTime;

    #region Circle -> Not Working
    //[Header("Circle")]

    /// <summary>The distance at which the boss should circle the player</summary>
    private float _circleCloseDistance;

    /// <summary>The Size of the Band that the boss will circle around the player within</summary>
    private float _circleBandWidth;

    /// <summary>The Speed at which the Boss circles the player at</summary>
    private float _circleSpeed;

    /// <summary>The Max Speed at which the Boss circles the player at</summary>
    private float _circleMaxSpeed;
    #endregion

    #region Idle
    [Header("Idle")]

    [SerializeField] private float intensity;
    [SerializeField] private float frequency;
    #endregion

    #region Ramming
    [Header("Ramming")]

    [SerializeField] private float _ramSpeed;

    /// <summary>The delay before the boss rams and it deciding to ram</summary>
    [SerializeField] private float _ramDelay;
    #endregion
    #endregion

    public bool outline => true;

    public float bounceStrength;
    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _flyDirection =RandomVector3(null, false);
        _flySpeed = _bigFlySpeed;
        _maxFlySpeed = _bigMaxFly;
        _audioSource = GetComponent<AudioSource>();
        _bossHp = _shatterMaterials.Length + 1;
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
                if(_ramtimer > 0)
                {
                    _ramtimer -= Time.deltaTime;
                }
                else
                {
                    _ramtimer = Random.Range(minRamTime, maxRamTime);
                    _bossBehaviour = BossBehaviour.Ramming;
                }

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

        
        if(_ramtimer > 0)
        {
            _ramtimer -= Time.deltaTime;    
        }
        else
        {

        }
    }

    #region Main State functions
    /// <summary>
    /// Animates the boss to be in the Idle state
    /// </summary>
    private void Idle()
    {
        // _rb.AddForce(Vector3.down * 5);
        // this.transform.position = Vector3.up * Mathf.Cos(Time.time * frequency) * intensity;
        // Vector3 floatfloat = Vector3.up * Mathf.Cos(Time.time * frequency) * intensity;
        // _rb.AddForce(floatfloat);

        _rb.velocity = Vector3.up * Mathf.Cos(Time.time * frequency) * intensity;
        transform.Rotate(0, 0.5f, 0);
    }   

    /// <summary>
    /// Get the boss to fly around the room
    /// </summary>
    private void Fly()
    {
        _rb.AddForce(_flyDirection * _flySpeed);
        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxFlySpeed);
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
    #endregion

    #region Useful Functions
    /// <summary>
    /// Returns the Distance to the player
    /// </summary>
    /// <returns>a float value that is the remaining distnace to the player</returns>
    private Vector3 GetPlayerDirection()
    {
        return PlayerController.instance.transform.position - this.transform.position;
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
                _flyDirection = RandomVector3(collision, true);
                break;
            case 7:
                PlayerController.instance.HitHazard("Boss", this.gameObject);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns a random vector where all values are between -1 and 1 inclusive
    /// If given collision data and true then it will randomise it in a way that moves it away from the collision
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomVector3(Collision collision, bool UseCollisionPosition)
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        if(UseCollisionPosition)
        {
            Vector3 normal = collision.GetContact(0).normal;
            if(normal.x != 0)
            {
                x = normal.x;
            }
            else if(normal.y != 0)
            {
                y = normal.y;
            }
            else if(normal.z != 0)
            {
                z = normal.z;
            }
        }

        Vector3 returnVector = new Vector3(x,y,z);
        returnVector.Normalize();
        return returnVector;
    }

    /// <summary>
    /// Denotes how the boss acts when it gets hit with a bean bag
    /// </summary>
    public void HitByBean()
    {
        Debug.Log("Boss hit by beanbag");
        _audioSource.clip = _hitSounds[Random.Range(0, _hitSounds.Length)];
        _audioSource.Play();
        _bossHp--;

        if (_bossHp == 0)
        {
            this.gameObject.layer = 8;
            Debug.Log("Boss has been beaten");

            AudioManager.instance.PlaySound(AudioManager.SoundEnum.defeatBoss);
            AudioManager.instance.PlayMusic(AudioManager.MusicEnum.defeatBoss);
            FindObjectsOfType<GuardAI>().ForEach(g => Destroy(g.gameObject));

            foreach (var o in _deathPersistent)
            {
                o.transform.parent = null;
                o.SetActive(true);
            }

            Destroy(this.gameObject);
        }
        else _shatterOverlay.material = _shatterMaterials[_bossHp - 1];
    }
    
    public void StartFight()
    {
        _bossBehaviour = BossBehaviour.Flying;
    }
    #endregion
}

enum BossBehaviour
{
    Idle,
    Flying,
    Circling,
    Ramming,
    Dead
}