using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GaurdSpawer : MonoBehaviour
{
    /// <summary>Minimum number of guards to spawn</summary>
    public int spawnMin;
    /// <summary>Maximum number of guards to spawn</summary>
    public int spawnMax;
    /// <summary>Ratio of Gun Guards to Guards 1 is all GunGuards 0 is all regular guards</summary>
    public float GunGaurdRatio;

    //Object refereces for the guards
    [SerializeField] private GuardAI Guard;
    [SerializeField] private GunGuard GunGuard;

    /// <summary>actual number of gaurds to spawn</summary>
    private int spawnNumber;
    /// <summary>list of all patrol points</summary>
    private List<PatrolPoint> points;

    private bool guardsActive = false;

    private GuardAI[] guards = new GuardAI[0];

    private void Start()
    {
        //SpawnGaurds();
    }

    /// <summary>
    /// Spawns the Guards, call when level is generated
    /// </summary>
    public void SpawnGaurds()
    {
        //find all patrol point objects
        points = FindObjectsOfType<PatrolPoint>().ToList();
        //if the number of points in the level divded by 4(4 so that no gaurd shares the same patrol point) is less than the spawn max set it to be the spawn max
        spawnMax = points.Count / 4 < spawnMax ? points.Count : spawnMax + 1;
        //get number of gaurds to spawn
        spawnNumber = Random.Range(spawnMin, spawnMax);

        guards = new GuardAI[spawnNumber];
        // for each gaurd to spawn
        for (int i = 0; i < spawnNumber; i++)
        {
            if (points.Count <= 0)
            {
                System.Array.Resize(ref guards, i + 1);
                break;
            }

            // get its spawn point 
            PatrolPoint p = points[Random.Range(0, points.Count)];
            points.Remove(p);
            GuardAI guard;
            // decide which type of guard it is
            if (Random.Range(0f, 1f) <= GunGaurdRatio)
            {
                guard = Instantiate(GunGuard, p.transform.position, p.transform.rotation, transform);
            }
            else 
            {
                guard = Instantiate(Guard, p.transform.position, p.transform.rotation, transform);
            }

            // get its patrol points
            List<Transform> guardPoints = new List<Transform>();
            guardPoints.Add(p.transform);
            for(int j = 0;j<3;j++)
            {
                int idx = Random.Range(0, points.Count);

                int k = 0;
                bool pointFound= false;
                while(k<points.Count && !pointFound)
                { 
                    p = points[idx];
                    if ((p.transform.position - guardPoints[0].position).magnitude < 20)
                    {
                        points.Remove(p);
                        guardPoints.Add(p.transform);
                        pointFound = true;
                    }
                    else
                    {
                        k++;
                        idx += k;
                        if (idx >= points.Count)
                        {
                            idx = 0;
                        }
                    }
                }
                
            }

            guard.patrolPoints = guardPoints.ToArray();
            guard.GuardActive = guardsActive;
            guards[i] = guard;
        }
    }

    public void startGuards()
    {
        guardsActive = true;
        foreach(GuardAI g in guards)
        {
                if (g == null) continue;
                g.GuardActive = true;
                g.GetComponent<NavMeshAgent>().speed = 3.5f;
        }
    }

    public void stopGuards()
    {
        foreach (GuardAI g in guards)
        {
            g.GuardActive = false;
            g.GetComponent<NavMeshAgent>().speed = 0;
        }
    }
}
