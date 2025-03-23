using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
public class FurnitureGrid : MonoBehaviour
{
    [SerializeField] private float cellSize = 2;
    [SerializeField] private float spacing = 0.25f;

    private Vector2Int size => new Vector2Int((int)(collider.size.x / cellSize), (int)(collider.size.z / cellSize));
    private float height => collider.center.y + collider.size.y / 2;

    new private BoxCollider collider;

#if UNITY_EDITOR
    private Vector3 oldSize = Vector3.zero;
    private Mesh mesh;

    private void OnDrawGizmos()
    {
        if (collider.size != oldSize || mesh == null) mesh = BuildMarker();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireMesh(mesh, transform.TransformPoint(collider.center));
    }

    private void OnValidate()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
        mesh = null;
    }
#endif

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        GetComponent<MeshFilter>().mesh = BuildMarker();
    }

    private Mesh BuildMarker()
    {
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();

        float far = cellSize - spacing;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                float xc = (x - size.x / 2f) * cellSize;
                float yc = (y - size.y / 2f) * cellSize;

                indicies.AddRange(new int[] {
                    0, 1, 2,
                    1, 3, 2
                }.Select(i => i + verticies.Count));

                verticies.Add(new Vector3(xc + spacing, height, yc + spacing));
                verticies.Add(new Vector3(xc + spacing, height, yc + far));
                verticies.Add(new Vector3(xc + far, height, yc + spacing));
                verticies.Add(new Vector3(xc + far, height, yc + far));
            }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetIndices(indicies, MeshTopology.Triangles, 0);
        mesh.SetNormals(verticies.Select(_ => new Vector3(0, 1, 0)).ToArray());
        return mesh;
    }
}
