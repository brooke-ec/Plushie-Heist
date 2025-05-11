using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class GuardAI : MonoBehaviour
{
    #region public & Serialised fields
    /// <summary>Time The guard will search chase you after it stops detecting you until it stops</summary>
    [SerializeField] private float DetectionTime;
    /// <summary>The points the guard patrols between</summary>
    public Transform[] patrolPoints;

    public bool GuardActive;

    #endregion
    #region private fields
    /// <summary>NavMeshAgent component </summary>
    private NavMeshAgent agent;
    /// <summary>The animator component</summary>
    protected Animator anim;
    /// <summary>NavMeshAgent component </summary>
    protected GameObject chasee;
    /// <summary>Time since last detected</summary>
    private float detectionTimer;
    /// <summary>The current destination to patrol to</summary>
    private int curPatrolIndex;
    protected new AudioSource audio;


    #endregion
    #region core methods
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = patrolPoints[0].position;
        anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (GuardActive)
        {
            if (chasee != null && detectionTimer <= DetectionTime)
            {
                agent.destination = chasee.transform.position;

                detectionTimer += Time.deltaTime;
                agent.autoBraking = true;
                anim.SetBool("Caught", false);
                if (chasee != null && PlayerController.instance.arrested)
                {
                    agent.speed = 0;
                }
                else if (agent.remainingDistance <= 1.2 && !agent.pathPending && Vector3.Distance(transform.position, PlayerController.instance.transform.position) <= 4)
                {
                    Arrest();
                    anim.SetBool("Arrest", true);
                    anim.SetBool("Caught", true);
                }
            }
            else
            {
                loseIntrest();
            }
        }
        else { agent.speed = 0;}
    }
    #endregion

    #region Player Interaction
    public void Detect(GameObject detectee)
    {
        Ray LOSRay = new Ray(transform.position, detectee.transform.position - (transform.position+new Vector3(0,-1,0)));
        Debug.DrawRay(transform.position, detectee.transform.position - (transform.position + new Vector3(0, -1, 0)));
        RaycastHit hitinfo = new RaycastHit();
        if (Physics.Raycast(LOSRay, out hitinfo) && hitinfo.collider.gameObject.tag == "Player"){
            if (chasee == null) audio.Play();
            chasee = detectee;
            //Debug.Log("Detected");
            detectionTimer = 0;
            agent.speed = 10;
            anim.SetBool("Chasing", true);
            PlayerController.instance.addGuard(this);
        }
    }

    private void Patrol()
    {
        agent.autoBraking = false;
        if(agent.remainingDistance<0.5f&&!agent.pathPending)
        {
            curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[curPatrolIndex].position;
            agent.speed = 3.5f;
            chasee = null;
            anim.SetBool("Chasing", false);
            anim.SetBool("Arrest", false);
            //Debug.Log("Patrol");
        }
    }

    private void Arrest()
    {
        PlayerController.instance.arrested = true;
        //Debug.Log("arrest"+transform.position);
        //Debug.Log(agent.remainingDistance+transform.position.ToString());
        //Debug.Break();
        
    }

    public void loseIntrest()
    {
        
        if (chasee != null)
        {
            PlayerController.instance.removeGuard(this);
            chasee = null;
        }

        Patrol();
    }
    #endregion
}
