using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class GenerateNavmesh : MonoBehaviour
{
    void Start()
    {
        GetComponent<NavMeshSurface>()?.BuildNavMesh();
    }
}
