using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunGuard : GaurdAI
{
    [SerializeField] private GameObject proj;
    [SerializeField] private float firerate;

    private float fireTimer;
    // Start is called before the first frame update
    public override void  Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (chasee!=null &&!chasee.GetComponent<PlayerController>().arrested)
        {
            Shoot();
        }
        fireTimer += Time.deltaTime;
    }


    private void Shoot()
    {
        if(chasee!=null && fireTimer>firerate)
        {
            Vector3 direction = chasee.transform.position+chasee.GetComponentInChildren<CharacterController>().center - (transform.position+new Vector3(0.5f, 0, 0.5f));
            Instantiate(proj, transform.position+new Vector3(0.5f,0,0.5f), Quaternion.LookRotation(direction));
            fireTimer = 0;
        }
    }
}
