using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Apple;

public class GaurdAI : MonoBehaviour
{
    #region public & Serialised fields
    /// <summary>Time The guard will search chase you after it stops detecting you until it stops</summary>
    [SerializeField] private float DetectionTime;
    /// <summary>The points the guard patrols between</summary>
    public Transform[] patrolPoints;
    #endregion
    #region private fields
    /// <summary>NavMeshAgent component </summary>
    private NavMeshAgent agent;
    /// <summary>NavMeshAgent component </summary>
    private GameObject chasee;
    /// <summary>Time since last detected</summary>
    private float detectionTimer;
    /// <summary>The current destination to patrol to</summary>
    private int curPatrolIndex;
    #endregion
    #region core methods
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = patrolPoints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasee != null && detectionTimer<=DetectionTime)
        {
            agent.destination = chasee.transform.position;
            detectionTimer += Time.deltaTime;
            agent.autoBraking = true;
            if(agent.remainingDistance <= 1.2)
            {
                Arrest();
            }
        }
        else
        {
            Patrol();
        }
    }
    #endregion
    
    public void detect(GameObject detectee)
    {
        Ray LOSRay = new Ray(transform.position, detectee.transform.position - (transform.position+new Vector3(0,-1,0)));
        Debug.DrawRay(transform.position, detectee.transform.position - (transform.position + new Vector3(0, -1, 0)));
        RaycastHit hitinfo = new RaycastHit();
        if (Physics.Raycast(LOSRay, out hitinfo) && hitinfo.collider.gameObject.tag == "Player"){
            chasee = detectee;
            Debug.Log("hit");
            detectionTimer = 0;
        }
    }

    private void Patrol()
    {
        agent.autoBraking = false;
        if(agent.remainingDistance<0.5f&&!agent.pathPending)
        {
            curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Length;
            agent.destination = patrolPoints[curPatrolIndex].position;
        }
    }

    private void Arrest()
    {
        chasee.GetComponent<PlayerController>().arrested = true;
    }
   
}
