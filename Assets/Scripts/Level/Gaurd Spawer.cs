using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GaurdSpawer : MonoBehaviour
{
    public int spawnMin;
    public int spawnMax;
    public float GunGaurdRatio;

    [SerializeField] private GameObject Guard;
    [SerializeField] private GameObject GunGuard;

    private int spawnNumber;

    private List<PatrolPoint> points;  
    private void Start()
    {
        //SpawnGaurds();
    }

    public void SpawnGaurds()
    {
        points = FindObjectsOfType<PatrolPoint>().ToList<PatrolPoint>();
        spawnMax = points.Count / 4 < spawnMax ? points.Count : spawnMax + 1;
        spawnNumber = Random.Range(spawnMin, spawnMax);

        for (int i = 0; i < spawnNumber; i++)
        { 
            PatrolPoint p = points[Random.Range(0, points.Count)];
            points.Remove(p);
            GameObject guard;
            if (Random.Range(0f, 1f) <= GunGaurdRatio)
            {
                guard = Instantiate(GunGuard, p.transform.position, p.transform.rotation, transform);
            }
            else 
            {
                guard = Instantiate(Guard, p.transform.position, p.transform.rotation, transform);
            }

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

            guard.GetComponent<GaurdAI>().patrolPoints = guardPoints.ToArray();
        }
    }
}
