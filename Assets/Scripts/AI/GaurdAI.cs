using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GaurdAI : MonoBehaviour
{
    #region Serialised fields
    [SerializeField] private float DetectionTime;
    #endregion
    #region private fields
    /// <summary>NavMeshAgent component </summary>
    private NavMeshAgent agent;
    /// <summary>NavMeshAgent component </summary>
    private GameObject chasee;
    /// <summary>Time since last detected</summary>
    private float detectionTimer;
    #endregion
    #region core methods
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chasee != null && detectionTimer<=DetectionTime)
        {
            agent.destination = chasee.transform.position;
            detectionTimer += Time.deltaTime;
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
   
}
